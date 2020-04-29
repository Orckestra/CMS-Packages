## Technical details
This package has no dependencies. But if your website uses specific styles filetypes, be sure you have installed the appropriate CSS compilers of the latest versions:
- to support **LESS** styles filetypes install [**Orckestra.Web.Css.Less**](https://github.com/Orckestra/CMS-Packages/tree/master/Composite.Web.Css.Less) package of the version **1.3.3 or more**.
- to support **SCSS** styles filetypes install [**Orckestra.Web.Css.Sass**](https://github.com/Orckestra/CMS-Packages/tree/master/Composite.Web.Css.Sass) package of the version **2.0.2 or more**.

In case your website has specific styles filetypes, but you have not installed demanded compilers, such files will be excluded during processing.

## Description
The package provides scripts and/or styles bundling and minification. It allows to reduce the number of requests that the browser needs to get all of the resource files, to reduce the size of such files and, as a result, to load pages faster. This package does not minify inline scripts, inline styles, or HTML code itself.
A bundle with styles adding to the original page to the end of the HEAD part. While a bundle with scripts adding to the end of the BODY part.

To exclude specific style or script from the bundling and minification process, add to such element c1-not-bundleminify attribute with "true" value.

To turn on or to turn off bundling and minification of all **styles**:
  - open the **Web.config** file in the root directory of a website;
  - locate to the **configuration/appSettings** path;
  - find the key `Orckestra.Web.BundlingAndMinification.BundleAndMinifyStyles`;
  - set up `true` or `false` value.
  
  To turn on or to turn off bundling and minification of all **scripts**:
  - open the **Web.config** file in the root directory of a website;
  - locate to the **configuration/appSettings** path;
  - find the key `Orckestra.Web.BundlingAndMinification.BundleAndMinifyScripts`;
  - set up `true` or `false` value.
  
  To turn on/off the package at all, turn on/off both styles and scripts in the settings as described above.
  
  There is a list of predefined cases when the package does not bundle and minify scripts and styles of a page:
  - A request appears when the current user logged in to the admin console panel.
  - A request is related to the admin console panel.
  - A website works in debugging mode. To check the debug mode status open the **Web.config** file in the root of a website and locate to the **configuration/system.web/compilation** path.

If none of these conditions is true, but scripts/styles are not bundled and minified - be sure it is not a cached version of a page, and check logs.
