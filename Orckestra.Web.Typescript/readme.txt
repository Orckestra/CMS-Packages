      Before installation be sure you have turned off debug mode of your web site. To check debug mode status of your 
      web site open Web.config in the root of you site and locate to configuration/system.web/compilation to debug attribute.
      Package provides typescripts compilation into javascripts using Microsoft Typescript Compiler of version 3.1.5.
      Typescripts changes can critically influence on alive web site, therefore pretty recommended to provide any tests,
      changes or configurations both with this package and with your typescripts in test enviroment, not on alive web site.

      Package is preconfigured to be ready to use Typescripts of default Orckestra RefApplication application on C1 engine.
      By default, typescripts will be looking in the folder ~/UI.Package/Typescript and will be compiled into 
      a file ~/UI.Package/Javascript/orckestra.composer.js. But you can change this behavior and any settings.

      Package has 3 level of settings:
      - General settings;
      - Package settings;
      - Compilation settings.

      Lev.1. General settings. Located in the root Web.config of your site and allows you to turn on or to turn off this package.
      To turn on or to turn off this package open Web.config in the root of your site, locate to configuration/appSettings
      and find a key "Orckestra.Web.Typescripb.Enable". Here you can set up "true" or "false" value depending what you need.
      Also notice, that any errors of this package will be thrown if you set up debug mode in Web.config. To check or to set 
      up debug mode open a Web.config file and locate to configuration/system.web/compilation path. Set up "true" or "false"
      depending what do you need.

      Lev.2. Package (assembly) settings. Located in Bin folder of your site and allows you to change this package settings and 
      to create tasks. To manage your package settings, open Bin folder and locate to the Orckestra.Web.Typescript.dll.cmp.xml file. 
      Explanation about purpose of every setting you can find in this file in comments. In general, here you can see typescripts
      compilation tasks. Every typescript task amoung other settings contains a path to a compilation settings file tsconfig.json. 
      This file is originally used by Microsoft Typescript Compiler for typescripts compilation.

      Lev.3. Compilation (tsconfig.json) settings. Originally this file is used by TypescriptCompiler for compilation. 
      The path to the tsconfig.json file is configuring in tasks in package settings (lev.2). Predefinied tsconfig.json file has 
      minimun settings, but you can specify any other compilation settings. The only restriction, you must set up outFile value int 
      tsconfig.json. More about tsconfig.json project file and possible attributes and options you can read at 
      https://www.typescriptlang.org/docs/handbook/tsconfig-json.html