param (
	[String]$sqlName = "WIN-T3DU134RTBH\SQLEXPRESS",        
	[String]$chekoutpath ="C:\teamcity\buildAgent\work\1262d54dd13f362a",
	[String]$pathtoDb ="d:\_DB",                
	[String]$sitepath = "d:\_SITE\etalon_testsrv1",
	[String]$sitepathdefault = "d:\_SITE\etalon",
	[String]$testsitepath = "d:\_SITE",
	[String]$poolName ="test_Catalog",
	[String]$testName= "test_Catalog",
	[String]$etalonDb= "etalon_testsrv1",
	[String]$username= "sa", 
	[String]$password= "qweQWE123!",
	[String]$defaultFilesPath = "d:\_SITE\files_default"

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
Write-Host "sitepathdefault" $sitepathdefault
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
	
	# creating test copy
	Write-Host "delete test site " $testsitepath
	Get-ChildItem -Path $testsitepath -Include * -File -Recurse | foreach { $_.Delete()} 
	
	Write-Host "crete test site folder" $testsitepath
	createFolder $testsitepath
	
	Write-Host "copy test site " $testsitepath
	copyFolderRetry $sitepathdefault $testsitepath
		
	#return default pictures and userfiles for test_site
	$fromPath = $defaultFilesPath + "\pictures";
	$toPath = $testsitepath + "\pictures"; 
    copyFolderRetry $fromPath $toPath 

    $fromPath = $defaultFilesPath + "\userfiles"; 
	$toPath = $testsitepath + "\userfiles";
    copyFolderRetry  $fromPath $toPath
	
	# set all connections
	#trying to access a shared site resource
	#Write-Host "set connection " $site
	#Write-Host "set default connection " $site
	#SetDefaultConnectionString $sitepath
	#SetConnectionString $sitepath $sqlName $database $username $password
	
	#set site connection for testsite to check etalon_db at step 'request...'
	Write-Host "set connection " $sitetest
	SetConnectionString $testsitepath $sqlName $database $username $password
	
	Write-Host "create site " $site
	CreateSite $site $sitepathdefault $poolName
	
	Write-Host "create test site " $sitetest
	CreateSite $sitetest $testsitepath $poolName
	
	# pinging
	# site is only used for restore the database, and sitetest is used for run tests

	#Write-Host "do request to test site " $site
	#$url = "http://localhost/" + $site
	#DoRequestRetry $url

	#request to check default database cause default testDB has not expected version
	Write-Host "do request to test site " $sitetest
	$url = "http://localhost/" + $sitetest
	DoRequestRetry $url

	#set the correct test connection
	Write-Host "set default connection " $sitetest
	SetDefaultConnectionString $testsitepath
	
	Write-Host "set connection " $sitetest
	SetConnectionString $testsitepath $sqlName $databasetest $username $password

	Write-Host "done"
}

catch [Exception]
{
	#Write-Host $_.Exception.Message
	$errorsReported = $False
	CheckForErrors $Error
	exit 1
}