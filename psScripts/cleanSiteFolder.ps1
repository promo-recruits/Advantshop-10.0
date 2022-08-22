#--------------------------------------------------
# Project: AdvantShop.NET
# Web site: https:\\www.advantshop.net
#--------------------------------------------------

#region Params
param(
# Path to published site (folder with published ADVANTSHOP)
    [String]$SitePath = "",
# Clean files after webpack build and files that dont need anymore
    [String]$CleanAfterWebpack = "false"
)
#endregion Params

#region Local params
$PS_firstLevelLog = "   "
$PS_secondLevelLog = "      "
#endregion Local params

# Importing helpers scripts
$ScriptDir = Split-Path -parent $MyInvocation.MyCommand.Path
Import-Module $ScriptDir\helpers\PsHelper.psm1
Import-Module $ScriptDir\helpers\FilesHelper.psm1

# Writing params to log (check the correctness of the received parameters)
#region Correctness
Write-Host "!Check the correctness"
Write-Host $PS_firstLevelLog "SitePath --> " $SitePath
Write-Host $PS_firstLevelLog "CleanAfterWebpack --> " $CleanAfterWebpack
Write-Host "!End of params"
#endregion Correctness

# Check if params is not set
#region Param checks
Write-Host "!Checking params"
if ($SitePath -eq $null -or $SitePath -eq "")
{
    Write-Host $PS_firstLevelLog "sitePath param is empty"
    exit 1
}
Write-Host "!Params checked"
#endregion Param checks

# Converting params because in case of TeamCity params converts to strings
#region Converting
try
{
    $IsWebpack = [System.Convert]::ToBoolean($CleanAfterWebpack)
    Write-Host $PS_firstLevelLog "CleanAfterWebpack converted to boolean --> " $IsWebpack
}
catch [FormatException]
{
    Whrite-Host $PS_firstLevelLog "!Error on convertion CleanAfterWebpack param to boolean - settings will be false"
    $IsWebpack = $false
}
#endregion Converting

function CleanCommonFilesInFolde
{
    param(
        [string]$Path,
        [bool]$WithWebpack,
        [bool]$LeavePartialScripts,
        [string]$LogPadding
    )

    Write-Host $LogPadding "!Removing common developing files"

    RemoveFile("$Path\Web.ConnectionString.config.etalon")
    RemoveFile("$Path\package.json.etalon")
    RemoveFile("$Path\karma.conf.js")
    RemoveFile("$Path\job_scheduling_data_2_0.xsd")
    RemoveFile("$Path\gulpfile.js")
    RemoveFile("$Path\AdvantShop.Web.Site.csproj.teamcity.msbuild.tcargs")
    RemoveFile("$Path\AdvantShop.Web.Site.csproj.teamcity")
    RemoveFile("$Path\.babelrc")
    RemoveFile("$Path\.browserslistrc")
    RemoveFile("$Path\.npmrc")
    RemoveFile("$Path\postcss.config.js")
    RemoveFile("$Path\webpack.config.js")
    RemoveFile("$Path\webpack.config.dev.js")
    RemoveFile("$Path\webpack.config.prod.js")
    RemoveFile("$Path\webpack.config.rules.js")
    RemoveFile("$Path\package.json")
    RemoveFile("$Path\package-lock.json")
    RemoveFile("$Path\AdvantShop.Web.Site.nuspec")

    Write-Host $LogPadding "!Removing node scripts"
    RemoveFolder("$Path\bundle_config\");
    RemoveFolder("$Path\node_modules\");
    RemoveFolder("$Path\node_scripts\");

    Write-Host $LogPadding "!Removing design folder with exlude => .css, images, .config"
    CleanFolder -root "$Path\design" -exclude ('\.css', 'images', '\.config')

    $pathStyles = "$Path\styles\"
    $pathScripts = "$Path\scripts\"

    if ($WithWebpack)
    {
        Write-Host $LogPadding "!Removing styles folder"
        RemoveFolder($pathStyles)
        if ($LeavePartialScripts)
        {
            # Admin pages uses _common and _partials scripts
            Write-Host $LogPadding "!Cleaning scripts folder (exclude '_common', '_partials', '_mobile', *.html)"
            CleanFolder -root $pathScripts -exclude ('_common', '_partials', '_mobile', '\.html')

            $pathScriptsCommon = "$Path\scripts\_common\"
            $pathScriptsPartials = "$Path\scripts\_partials\"
            $pathScriptsMobile = "$Path\scripts\_mobile\"
            Write-Host $LogPadding "!Cleaning scripts _common folder (exclude '\.css', '\.js', '\.html')"
            CleanFolder -root $pathScriptsCommon -exclude ('\.css', '\.js', '\.html')
            Write-Host $LogPadding "!Cleaning scripts _partials folder (exclude '\.css', '\.js', '\.html')"
            CleanFolder -root $pathScriptsPartials -exclude ('\.css', '\.js', '\.html')
            Write-Host $LogPadding "!Cleaning scripts _mobile folder (exclude '\.css', '\.js', '\.html')"
            CleanFolder -root $pathScriptsMobile -exclude ('\.css', '\.js', '\.html')
        }
        else
        {
            Write-Host $LogPadding "!Cleaning scripts folder (exclude *.html)"
            CleanFolder -root $pathScripts -exclude ('\.html')
        }
    }
    else
    {
        Write-Host $LogPadding "!Cleaning styles folder (exclude *.css)"
        CleanFolder -root $pathStyles -exclude ('\.css')
        Write-Host $LogPadding "!Cleaning scripts folder (exclude '_common', '_partials', '_mobile\\full-height-mobile', '_mobile\\mobileOverlap\.html', '\.css', 'extendScripts', '\.html')"
        CleanFolder -root $pathScripts -exclude ('_common', '_partials', '_mobile\\full-height-mobile', '_mobile\\mobileOverlap\.html', '\.css', 'extendScripts', '\.html')
    }
}


try
{
    Write-Host "!Removing .pdb files from bin folder"
    RemoveFilesWithExtension -path "$SitePath\bin\" -extension ".pdb"

    Write-Host "!Removing all .scss files"
    RemoveFilesWithExtension -path "$SitePath" -extension ".scss"

    Write-Host "!Cleaning root folder"
    CleanCommonFilesInFolde -Path $SitePath -WithWebpack $True -LeavePartialScripts $True -LogPadding $PS_firstLevelLog

    Write-Host "!Cleaning Areas/Mobile folder"
    CleanCommonFilesInFolde -Path "$SitePath\Areas\Mobile\" -WithWebpack $True -LeavePartialScripts $False -LogPadding $PS_firstLevelLog

    $templatesRoot = $SitePath + '\Templates'
    if (Test-Path $templatesRoot)
    {
        $templates = Get-ChildItem $templatesRoot
        if ($templates.Count -gt 0)
        {
            Write-Host "!Cleaning Templates folders"
            foreach ($templatesItem in $templates)
            {
                Write-Host $PS_firstLevelLog "Removing files for $templatesItem"
                $templeteRoot = "$templatesRoot\$templatesItem"
                CleanCommonFilesInFolde -Path $templeteRoot -WithWebpack $True -LeavePartialScripts $False -LogPadding $PS_secondLevelLog
                Write-Host $PS_firstLevelLog "Removing files for $templatesItem\Areas\Mobile\"
                CleanCommonFilesInFolde -Path "$templeteRoot\Areas\Mobile\" -WithWebpack $True -LeavePartialScripts $False -LogPadding $PS_secondLevelLog
            }
        }
        else
        {
            Write-Host $PS_firstLevelLog "!Not found templates for clean"
        }
    }
}
catch
{
    FormatErrors $Error
    exit 1
}

Write-Host "!Done!"
