$SourceRepoUserName = $args[0]
$SourceRepoPassword = $args[1]
$TargetRepoUserName = $args[2]
$TargetRepoPassword = $args[3]

$xml = [xml] (Get-Content .\LocalizationTool.exe.config)
$WebsitePath = $xml.SelectNodes('//add[@key="websitePath"]/@value')[0].'#text'
$Source = $xml.SelectNodes('//add[@key="sourceCultureName"]/@value')[0].'#text'
$Target = $xml.SelectNodes('//add[@key="targetCultureName"]/@value')[0].'#text'
$SourceBranch = $xml.SelectNodes('//add[@key="sourceBranch"]/@value')[0].'#text'
$TargetBranch = $xml.SelectNodes('//add[@key="targetBranch"]/@value')[0].'#text'
$SourceRepo = $xml.SelectNodes('//add[@key="sourceRepo"]/@value')[0].'#text'
$TargetRepo = $xml.SelectNodes('//add[@key="targetRepo"]/@value')[0].'#text'
$SourcePath = $xml.SelectNodes('//add[@key="sourcePath"]/@value')[0].'#text'
$TargetPath = $xml.SelectNodes('//add[@key="targetPath"]/@value')[0].'#text'
$LocalSourcePath = $xml.SelectNodes('//add[@key="localSourcePath"]/@value')[0].'#text'
$LocalTargetPath = $xml.SelectNodes('//add[@key="localTargetPath"]/@value')[0].'#text'
$SourceFolder = "source"
$TargetFolder = "target"
$PackageFolder = "package"
$RemotServerName = "XPCTEAM01"
if($SourceRepo)
{
$strFolderName=$SourceFolder
If (-NOT(Test-Path $strFolderName)){
	exit 1
}

	if($TargetRepo)
	{
		$IncludeItems = Get-ChildItem .\$SourceFolder\$SourcePath
	}
	else
	{
		$IncludeItems = @()
		$OriginalTexts = Get-ChildItem .\$SourceFolder\$SourcePath
		$IncludeItems += $OriginalTexts
		$IncludeItems += $OriginalTexts -replace $Source, $Target
	}
$remoteServerDir = "\\$RemotServerName\www\C1Build-em-$SourceBranch\$LocalSourcePath\"
if (!(test-path($remoteServerDir))) { mkdir $remoteServerDir }
copy-Item -PassThru -Path .\$WebsitePath\$LocalSourcePath\*.* $remoteServerDir -Include $IncludeItems -Recurse -Force
}
if($TargetRepo)
{
	$strFolderName=$TargetFolder
If (-NOT(Test-Path $strFolderName)){
	exit 1
}
$IncludeItems = @()
$OriginalTexts = Get-ChildItem .\$SourceFolder\$SourcePath
$IncludeItems += $OriginalTexts -replace $Source, $Target
$IncludeItems += Get-ChildItem .\$TargetFolder\$TargetPath
$remoteServerDir = "\\$RemotServerName\www\C1Build-em-$SourceBranch\$LocalTargetPath\"
if (!(test-path($remoteServerDir))) { mkdir $remoteServerDir }
copy-Item -PassThru -Path .\$WebsitePath\$LocalTargetPath\*.* $remoteServerDir -Include $IncludeItems -Recurse -Force
}

.\touch.ps1 "\\$RemotServerName\www\C1Build-em-$SourceBranch\Web.config"