#--------------------------------------------------
# Project: AdvantShop.NET
# Web site: https:\\www.advantshop.net
#--------------------------------------------------

function RemoveFile($Path)
{
    if (Test-Path $Path)
    {
        Remove-Item $Path
    }
}

function RemoveFilesWithExtension
{
    param(
        [string]$Path,
        [string]$Extension
    )

    if (Test-Path $Path)
    {
        Get-ChildItem $Path -recurse | Where-Object { $_.Extension -eq $Extension } | Remove-Item -Force -Recurse -Confirm:$false
    }
}

function RemoveFolder($Path)
{
    if (Test-Path $Path)
    {
        Get-ChildItem -Path $Path -Recurse | Remove-Item -Force -Recurse -Confirm:$false
        Remove-Item $Path -Force -Recurse
    }
}

function CleanFolder
{
    param(
        [string]$Root,
        [string[]]$Exclude
    )

    if (!(Test-Path $Root))
    {
        return
    }

    $Root = Resolve-Path $Root

    Push-Location $Root

    $objListToDel = Get-ChildItem $Root -Recurse

    $filesToDel = $objListToDel | Where-Object { $_.Extension -ne "" }
    $directoriesToDel = $objListToDel | Where-Object { $_.Extension -eq "" }

    foreach ($exclusion in $Exclude)
    {
        $filesToDel = $filesToDel | Where-Object { $_.fullname -notmatch $exclusion }
        $directoriesToDel = $directoriesToDel | Where-Object { $_.fullname -notmatch $exclusion }
    }

    foreach ($file in $filesToDel)
    {
        $file | Remove-Item
    }

    foreach ($dir in $directoriesToDel)
    {
        if (Test-Path $dir)
        {
            $filesInDirectoty = Get-ChildItem -Path $dir -Recurse -File

            if ($null -eq $filesInDirectoty)
            {
                $dir | Remove-Item -Recurse
            }
        }
    }

    Pop-Location

    if (@(Get-ChildItem $Root ).Count -eq 0)
    {
        $Root | Remove-Item
    }
}

function CopyFolder()
{
    param (
        [string] $from,
        [string] $to,
        $exclude = @("ttt.ttx"),
        $excludeMatch = @("qwqwqwq")
    )

    Write-Host $PS_firstLevelLog "copy " $from " to " $to
    [regex]$excludeMatchRegEx = '(?i)' + (($excludeMatch |ForEach-Object { [regex]::escape($_) }) -join "|") + ''
    Get-ChildItem -Path $from -Recurse -Exclude $exclude |
            Where-Object { $null -eq $excludeMatch -or $_.FullName.Replace($from, "") -notmatch $excludeMatchRegEx } |
            Copy-Item -Destination {
                $len = $from.length
                if ($_.PSIsContainer)
                {
                    Join-Path $to $_.Parent.FullName.Substring($len)
                }
                else
                {
                    Join-Path $to $_.FullName.Substring($len)
                }
            } -Force -Exclude $exclude
}

function CopyFolderRetry()
{
    param (
        [string] $from,
        [string] $to,
        $exclude = @("ttt.ttx"),
        $excludeMatch = @("qwqwqwq")
    )

    $retryCount = 5
    For ($i = 0; $i -lt $retryCount; $i++) {
        try
        {
            copyFolder $from $to $exclude $excludeMatch
            return
        }
        catch [Exception]
        {
            Write-Host $PS_firstLevelLog "copy err, trying again"
        }
    }
    throw $_.Exception
}


Export-ModuleMember -Function  *
