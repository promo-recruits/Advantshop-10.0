param (
        [String]$sqlName = "ADV-SAND-SRV\SQLEXPRESS",        
        [String]$chekoutpath ="E:\teamcity\buildAgent\work\8691eb17a5f15243",
        [String]$pathtoDb ="E:\db",                
        [String]$sitepath = "E:\sites\template",
        [String]$poolName ="template",
		[String]$username= "sa",
		[String]$password= "qweasdefgw123!",
		[String]$siteUrl= "",
		[String]$database= "template",
		[String]$site= "template"
)

$ScriptDir = Split-Path -parent $MyInvocation.MyCommand.Path
Import-Module $ScriptDir\..\Common\SqlHelper.psm1
Import-Module $ScriptDir\..\Common\SiteHelper.psm1
Import-Module $ScriptDir\..\Common\Helper.psm1
$ErrorActionPreference = "Stop"

$backupPath = $chekoutpath + "\DataBase\AdvantShop_10.0.0_empty.bak"
$scriptpath = $chekoutpath + "\DataBase\patches"

Write-Host "sqlName" $sqlName
Write-Host "database" $database
Write-Host "backupPath" $backupPath
Write-Host "pathtoDb" $pathtoDb
Write-Host "scriptpath" $scriptpath
Write-Host "site" $site
Write-Host "sitepath" $sitepath
Write-Host "poolName" $poolName

try 
{
	Write-Host "remove PDB"

	del ($sitepath + "\bin\*.pdb")

	Write-Host "restore database"

	RestoreDb $sqlName $database $pathtoDb $backupPath
	
	foreach ($f in Get-ChildItem -path $scriptpath -Filter *.sql | sort-object { [regex]::Replace($_.Name, '\d+', { $args[0].Value.PadLeft(20) }) } ) 
	{ 
		Write-Host "aplly file " $f.fullname $database
		SqlStringExec $sqlName $database $f.fullname
	}
	
	if($siteUrl -ne ""){
		$siteUrlCommand = "UPDATE Settings.Settings set value = '"+ $siteUrl + "' where name='ShopUrl'"
		SqlCommandExec $sqlName $database $siteUrlCommand
	}

	SqlCommandExec $sqlName $database "UPDATE Settings.Settings set value = '8b40c4f4-322e-4926-ad39-d2f6d6cd10c1' where name='lickey'; UPDATE Settings.Settings set value = 'True' where name='ActiveLic'"	
	SqlCommandExec $sqlName $database "insert Settings.Settings (name, value) values ('ClientCode','240')"
	
	$fromPath =$chekoutpath + "\AdvantShop.Web\pictures";
	$toPath= $sitepath  + "\pictures"; 
	copyFolderRetry $fromPath $toPath 

	$fromPath =$chekoutpath + "\AdvantShop.Web\userfiles";
	$toPath= $sitepath  + "\userfiles"; 
	copyFolderRetry  $fromPath $toPath

	$fromPath =$chekoutpath + "\AdvantShop.Web\design";
	$toPath= $sitepath  + "\design"; 
	copyFolderRetry $fromPath $toPath

	Write-Host "set connection " $site
	SetConnectionString $sitepath $sqlName $database $username $password

	Write-Host "set public version " $site
	SetPublicVersion $sitepath


	Write-Host "create site " $site
	CreateSite $site $sitepath $poolName

	# pinging
	Write-Host "do request to site " $site
	$url = "http://localhost/" + $site
	DoRequestRetry $url

	Write-Host "done"
}
catch [Exception]
{
	#Write-Host $_.Exception.Message
	$errorsReported = $False
	CheckForErrors $Error
	exit 1
}