  
  ## Technical details

  
The package provides typescripts compilation into javascript using [Microsoft Typescript Compiler of version 3.1.5](https://www.nuget.org/packages/Microsoft.TypeScript.Compiler/3.1.5). Any typescript changes recompiles dynamically into a javascript file. Typescripts changes can critically influence on a live web site. Therefore it pretty recommended providing any tests, changes, or configurations both with this package and with typescripts in a test environment, not on a live web site.
  Before installing, be sure to turn off the debug mode of a web site. To check the debug mode status open the **Web.config** file in the root of a site and locate to the **configuration/system.web/compilation** path.
  
  ## Description
  
  The package initially configured to be ready to compile typescripts of default **Orckestra Reference Application on the C1 engine**. By default, typescripts looking in the folder **{website_root_path}/UI.Package/Typescript** and compiling into a file **{website_root_path}/UI.Package/Javascript/orckestra.composer.js**. But this behavior can be changed in settings.
  
  The package has 3 levels of settings:
  - general settings;
  - package settings;
  - compilation settings.
  
  ### Level 1. General settings.
  They located in the root directory of a web site in the Web.config file. Settings allow enabling or disabling of this package. If this package is not using, recommended to turn it off.
  To turn on or to turn off this package:
  - open the **Web.config** file in the root directory of a web site;
  - locate to the **configuration/appSettings** path;
  - find the key `Orckestra.Web.Typescript.Enable`;
  - set up `true` or `false` value.
  
  Any errors of this package thrown if the debug mode in the Web.config file is enabled.
  To check or to set up the debug mode:
  - open the **Web.config** file;
  - locate to the **configuration/system.web/compilation** path;
  - set up `true` or `false` value to the `debug` attribute.
  
  ### Level 2. Package (assembly) settings.
  They located in the bin folder of a web site. These settings allow creating typescript compilation tasks. To manage package settings, open the **bin** folder and locate the **Orckestra.Web.Typescript.dll.cmp.xml** file. Explanation about settings usage it is possible to find in this file in comments. Every typescript task between other settings contains a path to a compilation settings file **tsconfig.json**. Microsoft Typescript Compiler initially uses this file for typescripts compilation.
  
  ### Level 3. Compilation settings.
  These are settings, defined in the tsconfig.json file(s). Initially, this file is used by Microsoft Typescript Compiler for compilation. The path to the **tsconfig.json** file is configuring in tasks in [package (assembly) settings](#level-2-package-assembly-settings). Default tsconfig.json file has a minimal set of settings, but any other compilation settings can be specified. It is compulsory to set up the `outFile` value in the tsconfig.json file.
  More about the tsconfig.json project files, possible attributes and options at [https://www.typescriptlang.org/docs/handbook/tsconfig-json.html](https://www.typescriptlang.org/docs/handbook/tsconfig-json.html)
