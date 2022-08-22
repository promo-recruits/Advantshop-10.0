#--------------------------------------------------
# Project: AdvantShop.NET
# Web site: https:\\www.advantshop.net
#--------------------------------------------------

#region Params
param(
#region SQL
# Name of sql server and instance
    [String]$SqlName = "localhost\SQLEXPRESS",
# Name of sql user
    [String]$SqlUserName = "sa",
# SQL password
    [String]$SqlPassword = "",
# SQL database name
    [String]$SqlDatabaseName = "",
#endregion SQL

#region Paths
# Path to dbs (folder where MSSQL contains db files)
    [String]$PathToDb = "",
# Path to chekout folder (ADVANTSHOP project src folder)
    [String]$CheckoutPath = "",
# Path to published site (folder with published ADVANTSHOP)
    [String]$SitePath = "",
#endregion Paths

#region IIS
# IIS pool name
    [String]$IISPoolName = "",
# IIS site name
    [String]$IISSiteName = "",
# IIS site url
    [String]$IISUrl = "localhost",
#endregion

#region Build settings
# Site type
    [String]$SiteType = "lic",
# Setup settings StoreActive
    [String]$ActiveStore = "false"
#endregion Build settings
)
#endregion Params

#region Local params
$PS_firstLevelLog = "   "
$PS_secondLevelLog = "      "
$SqlBackupFile = $CheckoutPath + "\DataBase\AdvantShop_10.0.0_empty.bak"
$SqlPatchesPath = $CheckoutPath + "\DataBase\patches"
$LicKey = ""
$ClientCode = ""
$CopyContent = $false
#endregion Local params

# Importing helpers scripts
$ScriptDir = Split-Path -parent $MyInvocation.MyCommand.Path
Import-Module $ScriptDir\helpers\PsHelper.psm1
Import-Module $ScriptDir\helpers\FilesHelper.psm1
Import-Module $ScriptDir\helpers\SqlHelper.psm1
Import-Module $ScriptDir\helpers\IISHelper.psm1


# Writing params to log (check the correctness of the received parameters)
#region Correctness
Write-Host "!Check the correctness"
Write-Host $PS_secondLevelLog "SQL"
Write-Host $PS_secondLevelLog "SqlName " $SqlName
Write-Host $PS_secondLevelLog "SqlUserName " $SqlUserName
Write-Host $PS_secondLevelLog "SqlPassword " $SqlPassword
Write-Host $PS_secondLevelLog "SqlDatabaseName " $SqlDatabaseName
Write-Host "---"
Write-Host $PS_secondLevelLog "Paths"
Write-Host $PS_secondLevelLog "PathToDb " $PathToDb
Write-Host $PS_secondLevelLog "CheckoutPath " $CheckoutPath
Write-Host $PS_secondLevelLog "SitePath " $SitePath
Write-Host "---"
Write-Host $PS_secondLevelLog "IIS"
Write-Host $PS_secondLevelLog "IISPoolName " $IISPoolName
Write-Host $PS_secondLevelLog "IISSiteName " $IISSiteName
Write-Host $PS_secondLevelLog "IISUrl " $IISUrl
Write-Host "---"
Write-Host $PS_secondLevelLog "Build settings"
Write-Host $PS_secondLevelLog "SiteType " $SiteType
Write-Host $PS_secondLevelLog "ActiveStore " $ActiveStore
Write-Host "!End of params"
#endregion Correctness

# Check if params is not set
#region Param checks
Write-Host "!Checking params"
if ($SqlName -eq $null -or $SqlName -eq "")
{
    Write-Host $PS_firstLevelLog "sqlName param is empty"
    exit 1
}
if ($SqlUserName -eq $null -or $SqlUserName -eq "")
{
    Write-Host $PS_firstLevelLog "sqlUserName param is empty"
    exit 1
}
if ($SqlPassword -eq $null -or $SqlPassword -eq "")
{
    Write-Host $PS_firstLevelLog "sqlPassword param is empty"
    exit 1
}
if ($SqlDatabaseName -eq $null -or $SqlDatabaseName -eq "")
{
    Write-Host $PS_firstLevelLog "sqlDatabaseName param is empty"
    exit 1
}
if ($PathToDb -eq $null -or $PathToDb -eq "")
{
    Write-Host $PS_firstLevelLog "pathToDb param is empty"
    exit 1
}
if ($CheckoutPath -eq $null -or $CheckoutPath -eq "")
{
    Write-Host $PS_firstLevelLog "CheckoutPath param is empty"
    exit 1
}
if ($SitePath -eq $null -or $SitePath -eq "")
{
    Write-Host $PS_firstLevelLog "sitePath param is empty"
    exit 1
}
if ($IISPoolName -eq $null -or $IISPoolName -eq "")
{
    Write-Host $PS_firstLevelLog "iisPoolName param is empty"
    exit 1
}
if ($IISSiteName -eq $null -or $IISSiteName -eq "")
{
    Write-Host $PS_firstLevelLog "iisSiteName param is empty"
    exit 1
}
if ($IISUrl -eq $null -or $IISUrl -eq "")
{
    Write-Host $PS_firstLevelLog "iisUrl param is empty"
    exit 1
}
if ($SiteType -eq $null -or $SiteType -eq "")
{
    Write-Host $PS_firstLevelLog "siteType param is empty"
    exit 1
}
Write-Host "!Params checked"
#endregion Param checks

# Converting params because in case of TeamCity params converts to strings
#region Converting
try
{
    $StoreActive = [System.Convert]::ToBoolean($ActiveStore)
}
catch [FormatException]
{
    Write-Host "!Error on convertion ActiveStore param to boolean - settings will be false"
    $StoreActive = $false
}

if ($SiteType -eq "lic")
{
    $LicKey = "8b40c4f4-322e-4926-ad39-d2f6d6cd10c1"
    $ClientCode = "240"
    $CopyContent = $true
}
if ($SiteType -eq "saas")
{
    $LicKey = "e6dc57ee-931e-4ac1-9137-5a47e56f977a"
    $ClientCode = "76001"
}
#endregion Converting

try
{
    Write-Host "!Restore database"
    RestoreDb $SqlName $SqlDatabaseName $PathToDb $SqlBackupFile

    Write-Host "!Apply sql patches"
    foreach ($f in Get-ChildItem -path $SqlPatchesPath -Filter *.sql | sort-object { [regex]::Replace($_.Name, '\d+', { $args[0].Value.PadLeft(20) }) })
    {
        Write-Host $PS_firstLevelLog "aplly file " $f.fullname $SqlDatabaseName
        SqlFileExec $SqlName $SqlDatabaseName $f.fullname
    }

    Write-Host "!Updating [Settings].[Settings]"
    Write-Host $PS_firstLevelLog "!Updating ShopUrl"
    SqlCommandExec $SqlName $SqlDatabaseName "UPDATE Settings.Settings set value = '$IISUrl' where name='ShopUrl'"

    if ($LicKey -ne "")
    {
        Write-Host $PS_firstLevelLog "!Updating lickey"
        SqlCommandExec $SqlName $SqlDatabaseName "UPDATE Settings.Settings set value = '$LicKey' where name='lickey'; UPDATE Settings.Settings set value = 'True' where name='ActiveLic'"
    }
    if ($ClientCode -ne "")
    {
        Write-Host $PS_firstLevelLog "!Updating ClientCode"
        SqlCommandExec $SqlName $SqlDatabaseName "insert Settings.Settings (name, value) values ('ClientCode','$ClientCode')"
    }
    if ($StoreActive)
    {
        Write-Host $PS_firstLevelLog "!Updating StoreActive"
        SqlCommandExec $SqlName $SqlDatabaseName "update [Settings].[Settings] set Value = 'True' where Name = 'StoreActive'"
    }

    if ($CopyContent)
    {
        Write-Host "!Copy content"
        Write-Host $PS_firstLevelLog "!pictures folder"
        $fromPath = $CheckoutPath + "\AdvantShop.Web\pictures";
        $toPath = $SitePath + "\pictures";
        CopyFolderRetry $fromPath $toPath

        Write-Host $PS_firstLevelLog "!userfiles folder"
        $fromPath = $CheckoutPath + "\AdvantShop.Web\userfiles";
        $toPath = $SitePath + "\userfiles";
        CopyFolderRetry  $fromPath $toPath

        Write-Host $PS_firstLevelLog "!design folder"
        $fromPath = $CheckoutPath + "\AdvantShop.Web\design";
        $toPath = $SitePath + "\design";
        CopyFolderRetry $fromPath $toPath
    }

    Write-Host "!Set connection"
    SetConnectionString $SitePath $SqlName $SqlDatabaseName $SqlUserName $SqlPassword
    Write-Host "!Set public version"
    SetPublicVersion $SitePath

    Write-Host "!Create IIS site"
    CreateSite $IISSiteName $SitePath $IISPoolName

    Write-Host "!Ping to admin panel $IISUrl/admin"
    DoRequestRetry "$IISUrl/admin"
}
catch
{
    $errorsReported = $False
    FormatErrors $Error
    exit 1
}