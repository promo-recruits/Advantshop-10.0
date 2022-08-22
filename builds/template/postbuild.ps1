param (
    [String]$sitepath = "E:\sites\template"
)


$ScriptDir = Split-Path -parent $MyInvocation.MyCommand.Path
Import-Module $ScriptDir\..\Common\Helper.psm1

$templatesRoot = $sitepath + '\Templates'
$templates = Get-ChildItem $templatesRoot

if($templates.Count -gt 0){
    Write-Host "--- remove files for templates ---"

    foreach($templatesItem in $templates){
        $templeteRoot = "$templatesRoot\$templatesItem"
        $webpackBundleFolder = "$templeteRoot\bundle_config\_pages.js"
        if(Test-Path $webpackBundleFolder){
            Write-Host "remove files for $templatesItem"
            deleteCommonFiles -rootPath "$templeteRoot\" -isTemplateWithWebpack $true
        }

        $webpackBundleFolderMobile = "$templeteRoot\Areas\Mobile\bundle_config\_pages.js"
        if(Test-Path $webpackBundleFolderMobile){
            Write-Host "remove files for $templatesItem\Areas\Mobile\"
            deleteCommonFiles -rootPath "$templeteRoot\Areas\Mobile\" -isTemplateWithWebpack $true
        }
    }

    Write-Host "--- done ---"
}else{
    Write-Host "Not found templates for clean"
}
