param (
        [String]$sqlName = "WIN-T3DU134RTBH\SQLEXPRESS",        
        [String]$chekoutpath ="C:\teamcity\buildAgent\work\1262d54dd13f362a",
        [String]$pathtoDb ="d:\_DB",                
        [String]$sitepath = "d:\_SITE\etalon_testsrv1",
	[String]$testsitepath = "d:\_SITE",
        [String]$poolName ="test_Catalog",
	[String]$testName= "test_Catalog",
	[String]$etalonDb= "etalon_testsrv1",
	[String]$username= "sa",
	[String]$password= "qweQWE123!"

)

$ScriptDir = Split-Path -parent $MyInvocation.MyCommand.Path
Import-Module $ScriptDir\..\..\builds\Common\SqlHelper.psm1
Import-Module $ScriptDir\..\..\builds\Common\SiteHelper.psm1
Import-Module $ScriptDir\..\..\builds\Common\Helper.psm1
Import-Module WebAdministration
$ErrorActionPreference = "Stop"

$backupPath = $chekoutpath + "\DataBase\AdvantShop_10.0.0_empty.bak"
$scriptpath = $chekoutpath + "\DataBase\patches"

$database = $etalonDb
$databasetest = $testName
$site = $etalonDb
$sitetest = $testName
$testsitepath =  $testsitepath + "\" + $testName

Write-Host "sqlName" $sqlName
Write-Host "database" $database
Write-Host "backupPath" $backupPath
Write-Host "pathtoDb" $pathtoDb
Write-Host "scriptpath" $scriptpath
Write-Host "site" $site
Write-Host "sitetest" $sitetest
Write-Host "sitepath" $sitepath
Write-Host "testsitepath" $testsitepath
Write-Host "poolName" $poolName

try 
{
	Write-Host "restore database" $database
	RestoreDb $sqlName $database $pathtoDb $backupPath
	Write-Host "restore sitetest" $sitetest
	RestoreDb $sqlName $sitetest $pathtoDb $backupPath	

	foreach ($f in Get-ChildItem -path $scriptpath -Filter *.sql | sort-object { [regex]::Replace($_.Name, '\d+', { $args[0].Value.PadLeft(20) }) } ) 
	{ 
	    Write-Host "aplly file " $f.fullname $database
		SqlStringExec $sqlName $database $f.fullname
		
		#Write-Host "aplly file " $f.fullname $sitetest
		#SqlStringExec $sqlName $sitetest $f.fullname
		
	}
	
	SqlCommandExec $sqlName $database "UPDATE Settings.Settings set value = '8b40c4f4-322e-4926-ad39-d2f6d6cd10c1' where name='lickey'"
	#SqlCommandExec $sqlName $databasetest "UPDATE Settings.Settings set value = '8b40c4f4-322e-4926-ad39-d2f6d6cd10c1' where name='lickey'"
		
	
	SqlCommandExec $sqlName $database "insert Settings.Settings (name, value) values ('ClientCode','240')"
	#SqlCommandExec $sqlName $databasetest "insert Settings.Settings (name, value) values ('ClientCode','240')"
	
	
	$fromPath =$chekoutpath + "\AdvantShop.Web\pictures";
	$toPath= $sitepath  + "\pictures"; 
    copyFolderRetry $fromPath $toPath 

    $fromPath =$chekoutpath + "\AdvantShop.Web\userfiles";
	$toPath= $sitepath  + "\userfiles"; 
    copyFolderRetry  $fromPath $toPath

    $fromPath =$chekoutpath + "\AdvantShop.Web\design";
	$toPath= $sitepath  + "\design"; 
    copyFolderRetry $fromPath $toPath
	
	
	# creating test copy
	Write-Host "delete test site " $testsitepath
	Get-ChildItem -Path $testsitepath -Include * -File -Recurse | foreach { $_.Delete()} 
	
	Write-Host "crete test site folder" $testsitepath
	createFolder $testsitepath
	
	Write-Host "copy test site " $testsitepath
	copyFolderRetry $sitepath $testsitepath
	
	# set all connections
	Write-Host "set connection " $site
        Write-Host "set default connection " $site
	SetDefaultConnectionString $sitepath
	SetConnectionString $sitepath $sqlName $database $username $password
	
	Write-Host "set connection " $sitetest
	SetConnectionString $testsitepath $sqlName $databasetest $username $password
	
	Write-Host "create site " $site
	CreateSite $site $sitepath $poolName
	
	
	Write-Host "create test site " $sitetest
	CreateSite $sitetest $testsitepath $poolName
	
	# pinging
	Write-Host "do request to site " $site
	$url = "http://localhost/" + $site
	DoRequestRetry $url

	
	#Write-Host "do request to test site " $sitetest
	#$url = "http://localhost/" + $sitetest
	#DoRequestRetry $url

	Write-Host "done"
}

catch [Exception]
{
	#Write-Host $_.Exception.Message
	$errorsReported = $False
	CheckForErrors $Error
	exit 1
}