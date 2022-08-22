Param(
    $infile,
    $outfile,
    $etalonDb
  )
  try
  {
  if( -Not $outfile)
  {
    $outfile = $infile
  }
$stringEtalonDbApp = "Initial Catalog='etalon'"
$stringRepEtalonDbApp = "Initial Catalog='etalon_"+ $etalonDb + "'"
$temp_out_file = "$outfile.temp"

  Get-Content $infile | Foreach-Object {$_.Replace($stringEtalonDbApp, $stringRepEtalonDbApp)} | Set-Content $temp_out_file

  if( Test-Path $outfile)
  {
    Remove-Item $outfile
  }

  Move-Item $temp_out_file $outfile
  #Write-Host "done app.config"
  }
  
catch [Exception]
{
	#Write-Host $_.Exception.Message
	$errorsReported = $False
	CheckForErrors $Error
	exit 1
}
