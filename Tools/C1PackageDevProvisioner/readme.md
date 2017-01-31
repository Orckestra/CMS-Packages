This tool is intended to help you with package development, starter site testing and package installation testing.

The tool can do the following:
 - easily set up a test site / reset a test site
 - create a mock package server / mock setup server (let you test starter site configurations)
 - mirror changes to package files from your project to your test site

Using this tool:

1.	Have one or more package repos on your machine
2.	Build the solution C1PackageDevProvisioner
3.	In runner build dir output dir, edit Runner.exe.config
  1.	Set C1SitePath to a path – it can be a temp / empty folder, if you want this tool to initialize your C1 site
  2.	Set PackageProjectsPaths to one or more paths (separated by comma) to the root of repositories containing packages
  3.	Set C1BaseImageZipUrl to a URL giving ZIP containing the C1 build you desire to test on.
4.	If you want to update the starter site options, see SetupDescriptionPath in the config and the SetupDescription.xml file – the file define what starter site options you have and which packages goes into it.
5.	Run the tool (runner.exe) 
  1.	Say yes to resetting C1 site (this may take a long while first time) 
  2.	Tool will find all packages in the repos you specified and build them – first run take time
  3.	Launch IIS / IIS Express for the web app path C1SitePath from config
  4.	Launch browser showing your website – you will see the setup wizard (mock edition)
  5.	During setup you will see starter site options as defined in SetupDescription.xml
  6.	Changes to static files / assembly recompiles done in package projects (in one of your package repos) will be auto copied to the test site – so you can edit in project and immediately see on website
  7.	If you want to start over, stop the runner.exe tool (press any key) and run it again, saying yes to reset your website. Second run a lot faster.

The tool locate packages by searching for "install.xml" files (package install descriptions) and locating .sln files in nearest poarent directory. 
Packages are build using "nuget restore" and "msbuild" in the sollution directory of each package. Packages that cannot be build this way, will not work with this tool.
