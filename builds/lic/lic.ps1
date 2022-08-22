param (
        [String]$sqlName = "ADV-SAND-SRV\SQLEXPRESS",        
        [String]$chekoutpath ="E:\teamcity\buildAgent\work\8691eb17a5f15243",
        [String]$pathtoDb ="E:\db",                
        [String]$sitepath = "E:\sites\lic\current",
        [String]$poolName ="dev",
        [String]$outputpath ="E:\advantshopOut\lic",
		[String]$username= "sa",
		[String]$password= "qweasdefgw123!"
)

$ScriptDir = Split-Path -parent $MyInvocation.MyCommand.Path
Import-Module $ScriptDir\..\Common\SqlHelper.psm1
Import-Module $ScriptDir\..\Common\SiteHelper.psm1
Import-Module $ScriptDir\..\Common\Helper.psm1
$ErrorActionPreference = "Stop"

if (!(Test-Path -Path $sitepath))
{
    exit 0;
}

$backupPath = $chekoutpath + "\DataBase\AdvantShop_10.0.0_empty.bak"
$scriptpath = $chekoutpath + "\DataBase\patches"

$version = getVersion $sitepath

$database = "lic" + $version
$site ="lic" + $version

renameFolder $sitepath $site

$sitepath=$sitepath.Replace('current',$site)

Write-Host "sqlName" $sqlName
Write-Host "database" $database
Write-Host "backupPath" $backupPath
Write-Host "pathtoDb" $pathtoDb
Write-Host "scriptpath" $scriptpath
Write-Host "site" $site
Write-Host "sitepath" $sitepath
Write-Host "poolName" $poolName
Write-Host "outputpath" $outputpath

try 
{

	Write-Host "Lic"
	Write-Host "restore database"
	RestoreDb $sqlName $database $pathtoDb $backupPath
	
	foreach ($f in Get-ChildItem -path $scriptpath -Filter *.sql | sort-object { [regex]::Replace($_.Name, '\d+', { $args[0].Value.PadLeft(20) }) } ) 
	{ 
	    Write-Host "aplly file " $f.fullname
		SqlStringExec $sqlName $database $f.fullname
	}
	
	SqlCommandExec $sqlName $database "update [Settings].[Settings] set Value = 'False' where Name = 'StoreActive'"
	
    #$fromPath =$chekoutpath + "\AdvantShop.Web\pictures";
	#$toPath= $sitepath  + "\pictures"; 
    #copyFolderRetry $fromPath $toPath 

    $fromPath =$chekoutpath + "\AdvantShop.Web\userfiles";
	$toPath= $sitepath  + "\userfiles"; 
    copyFolderRetry  $fromPath $toPath
	    
    $fromPath =$chekoutpath + "\AdvantShop.Web\design";
	$toPath= $sitepath  + "\design"; 
    copyFolderRetry $fromPath $toPath

	Write-Host "set connection " $site
	SetConnectionString $sitepath $sqlName $database $username $password
	
	Write-Host "create site " $site
	CreateSite $site $sitepath $poolName
	
	#Write-Host "do request to site " $site
	#$url = "http://localhost/" + $site
	#DoRequestRetry $url

    $outPath = $outputpath + "\"+$site
    $backupfile = $outPath + "\" + $site + ".bak"
    $zipdb = $outPath + "\" + $site + "_db.zip"

    deleteFolder $outPath
    createFolder $outPath
	
	Write-Host "SHRINKDATABASE"
	$sqlShrinkCommand = "ALTER DATABASE [" + $database + "] SET RECOVERY SIMPLE; DBCC SHRINKDATABASE ('" + $database + "', 10);"
	Write-Host $sqlShrinkCommand
	SqlCommandExec $sqlName $database $sqlShrinkCommand
	
    Write-Host "backupDb"
	backupDb $sqlName $database $backupfile
	
	$exclude = @('AdvantShop.sln.metaproj','AdvantShop.sln.metaproj.tmp','Advantshop.Web.Site.csproj.teamcity','Advantshop.Web.Site.csproj.teamcity.msbuild.tcargs', 'gulpfile.js','job_scheduling_data_2_0.xsd','karma.conf.js','robots.txt', 'Web.ConnectionString.config.etalon', 'package.json.etalon', 'karma.conf.js', 'job_scheduling_data_2_0.xsd', 'gulpfile.js', 'AdvantShop.Web.Site.csproj.teamcity.msbuild.tcargs', 'AdvantShop.Web.Site.csproj.teamcity','.babelrc', '.browserslistrc', '.npmrc', 'postcss.config.js', 'webpack.config.js', 'webpack.config.dev.js', 'webpack.config.prod.js', 'webpack.config.rules.js', 'package.json', 'package-lock.json')
	$excludeMatch = @('obj','errlog', 'bundle_config', 'node_modules', 'node_scripts')
		
    $publishedPath = $outPath +"\published"
	copyFolderRetry $sitepath $publishedPath $exclude $excludeMatch

    $pathStyles = $publishedPath + "\styles\"
    $pathScripts = $publishedPath + "\scripts\"

    Clean-Folder -rootfolder $pathStyles -excluded ('\.css')
    Clean-Folder -rootfolder $pathScripts -excluded ('_common', '_partials', '_mobile\\full-height-mobile', '_mobile\\mobileOverlap\.html', '\.css')

	SetDefaultConnectionString $publishedPath

	$sourcepath = $outPath +"\source"
	
	$exclude = @('AdvantShop.sln.metaproj','AdvantShop.sln.metaproj.tmp')
	$excludeMatch = @('\bin\','\obj\','\errlog\')
	copyFolderRetry $chekoutpath $sourcepath $exclude $excludeMatch	
	$t = $sourcepath + '\AdvantShop.Web'
    SetDefaultConnectionString $t

	Write-Host "remove PDB"

	del ($publishedPath + "\bin\*.pdb")	

	$zipSite = $outPath + "\" + $site + ".zip"
    $zipFolderPath = $outPath + "\*"
	zip $zipSite $zipFolderPath
	
	SetConnectionString $sitepath $sqlName $database $username $password
	#uploadtocap

    deleteFile $backupfile
	deleteFolderRetry $publishedPath
	deleteFolderRetry $sourcepath
	
	Write-Host "done"
}
catch [Exception]
{
	#Write-Host $_.Exception.Message
	$errorsReported = $False
	CheckForErrors $Error
	exit 1
}