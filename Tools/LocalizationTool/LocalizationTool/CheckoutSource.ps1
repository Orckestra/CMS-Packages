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

if($SourceRepo)
{
echo "Source: $SourceRepo"
$strFolderName=$SourceFolder
If (Test-Path $strFolderName){
	Remove-Item $strFolderName -Force -Recurse
}
git init $SourceFolder
cd $SourceFolder/
$a,$b = $SourceRepo.Split(@('//'),'None')
$SourceRepoCred = "$a`//$SourceRepoUserName`:$SourceRepoPassword@$b"
git remote add -f origin $SourceRepoCred
git config core.sparseCheckout true
echo $SourcePath.replace(@('\'),@('/')) | out-file -encoding ascii .git/info/sparse-checkout
git pull $SourceRepoCred
$Branches = git branch -r
$result = $Branches | Select-String $SourceBranch
	if(!$result)	
	{exit 1}
	else
	{git checkout $SourceBranch
	git pull}
cd..
echo "Copying.."
copy-Item -PassThru -Force -Path .\$SourceFolder\$SourcePath\*.* .\$WebsitePath\$LocalSourcePath\
}
if($TargetRepo)
{
echo "Target: $TargetRepo"
$strFolderName=$TargetFolder
If (Test-Path $strFolderName){
	Remove-Item $strFolderName -Force -Recurse
}

git init $TargetFolder
cd $TargetFolder/
$a,$b = $TargetRepo.Split(@('//'),'None')
$TargetRepoCred = "$a`//$TargetRepoUserName`:$TargetRepoPassword@$b"
git remote add -f origin $TargetRepoCred
git config core.sparseCheckout true
echo $TargetPath.replace(@('\'),@('/')) | out-file -encoding ascii .git/info/sparse-checkout
git pull $TargetRepoCred
$Branches = git branch -r
$result = $Branches | Select-String $TargetBranch
	if(!$result)	
	{git checkout -b $TargetBranch}
	else
	{git checkout $TargetBranch
	git pull}
cd..
echo "Copying.."
copy-Item -PassThru -Force -Path .\$TargetFolder\$TargetPath\*.* .\$WebsitePath\$LocalTargetPath\
}
