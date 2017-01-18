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

echo "Ready to commit.."
if($SourceRepo)
{
	echo "Source: $SourceRepo"
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
		$IncludeItems += $OriginalTexts -replace ".resx", ".$Target.resx"
	}
copy-Item -Path .\$WebsitePath\$LocalSourcePath\*.* .\$SourceFolder\$SourcePath -Include $IncludeItems -Recurse -Force
cd $SourceFolder
$a,$b = $SourceRepo.Split(@('//'),'None')
$SourceRepoCred = "$a`//$SourceRepoUserName`:$SourceRepoPassword@$b"
git add .
git commit -m "localization Tool commit"
git push $SourceRepoCred
cd..
}
if($TargetRepo)
{
	echo "Target: $TargetRepo"
$strFolderName=$TargetFolder
If (-NOT(Test-Path $strFolderName)){
	exit 1
}
$IncludeItems = @()
$OriginalTexts = Get-ChildItem .\$SourceFolder\$SourcePath
$IncludeItems += $OriginalTexts -replace $Source, $Target
$IncludeItems += Get-ChildItem .\$TargetFolder\$TargetPath
copy-Item -Path .\$WebsitePath\$LocalTargetPath\*.* .\$TargetFolder\$TargetPath -Include $IncludeItems -Recurse -Force
cd $TargetFolder
$a,$b = $TargetRepo.Split(@('//'),'None')
$TargetRepoCred = "$a`//$TargetRepoUserName`:$TargetRepoPassword@$b"
git add .
git commit -m "localization Tool commit"
git push $TargetRepoCred
cd..
}
