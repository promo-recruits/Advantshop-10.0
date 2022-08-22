#--------------------------------------------------
# Project: AdvantShop.NET
# Web site: https:\\www.advantshop.net
#--------------------------------------------------
Import-Module "WebAdministration" -DisableNameChecking

function SetConnectionString($SitePath, $SqlName, $SqlDatabaseName, $SqlUserName, $SqlPassword)
{
    $config = "$SitePath\Web.ConnectionString.config"

    if (!(Test-Path $config))
    {
        $configEtalon = "$SitePath\Web.ConnectionString.config.etalon"
        Rename-Item $configEtalon $config
    }

    $doc = (Get-Content $config) -as [Xml]
    $root = $doc.get_DocumentElement();

    Write-Host $PS_firstLevelLog "old connection:", $root.add.connectionString

    #Replace common
    $newCon = $root.add.connectionString.Replace("Data Source='MyServerName'; Connect Timeout='60'; Initial Catalog='MyDBName'; Persist Security Info='True'; User ID='MyUserName'; Password='MyPass';",
            "Data Source='$SqlName'; Connect Timeout='60'; Initial Catalog='$SqlDatabaseName'; Persist Security Info='True'; User ID='$SqlUserName'; Password='$SqlPassword';");
    $root.add.connectionString = $newCon

    #Replace from dev
    $newCon = $root.add.connectionString.Replace("Data Source='SERVER\SQL2008R2EXPRESS'; Connect Timeout='60'; Initial Catalog='AdvantShop_6.5_etalon'; Persist Security Info='True'; User ID='sa'; Password='ewqEWQ321#@!';",
            "Data Source='$SqlName'; Connect Timeout='60'; Initial Catalog='$SqlDatabaseName'; Persist Security Info='True'; User ID='$SqlUserName'; Password='$SqlPassword';");
    $root.add.connectionString = $newCon

    Write-Host $PS_firstLevelLog "new connection:", $root.add.connectionString

    $doc.Save($config)
}

function SetPublicVersion($SitePath)
{
    $config = "$SitePath\Web.config"
    $doc = (Get-Content $config) -as [Xml]

    $node = $doc.configuration.appSettings.add | ? { $_.key -eq "PublicVersion" };
    $node.value = $node.value + " " + (Get-Date).ToString();

    $doc.Save($config)
}

function GetWebConfigAppSetting
{
    param
    (
        [string]$SitePath,
        [string]$KeyName
    )

    $config = "$SitePath\Web.config"
    $doc = (Get-Content $config) -as [Xml]

    $node = $doc.configuration.appSettings.add | ? { $_.key -eq $KeyName };
    return $node.value
}

function CreateSite($SiteName, $SitePath, $PoolName)
{
    $iisAppPoolName = $PoolName
    $iisAppPoolDotNetVersion = "v4.0"

    Set-Location IIS:\AppPools\
    #check if the app pool exists
    if (!(Test-Path $iisAppPoolName -pathType container))
    {
        #create the app pool
        $appPool = New-Item $iisAppPoolName
        $appPool | Set-ItemProperty -Name "managedRuntimeVersion" -Value $iisAppPoolDotNetVersion
    }
    Set-Location IIS:\Sites\DefaultWebSite
    if (Test-Path $SiteName -pathType container)
    {
        return
    }
    Set-ItemProperty IIS:\sites\DefaultWebSite\$SiteName -name applicationPool -value $iisAppPoolName
}

function DoRequest($Url)
{
    $webclient = New-Object Net.WebClient
    $stringSite = $webclient.DownloadString($Url);
    $webclient.sta
    if (-not $stringSite.Contains('<meta name="generator" content="AdVantShop.NET">'))
    {
        $errMsg = "Error on do request \n\r" + $stringSite
        throw [Exception] $errMsg
    }
}

function DoRequestRetry($Url)
{
    $retryCount =3
    For ($i=0; $i -lt $retryCount; $i++) {
        try
        {
            DoRequest($Url)
            return
        }
        catch [Exception]
        {
        }
    }
    throw $_.Exception
}

Export-ModuleMember -Function  *
