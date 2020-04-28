## Technical details
This package has no dependencies. But if your site uses specific styles filetypes, be sure you have installed the appropriate CSS compilers of the latest versions:
- to support **LESS** styles filetypes install [**Orckestra.Web.Css.Less**](https://github.com/Orckestra/CMS-Packages/tree/master/Composite.Web.Css.Less) package of the version **1.3.3 or more**.
- to support **SCSS** styles filetypes install [**Orckestra.Web.Css.Sass**](https://github.com/Orckestra/CMS-Packages/tree/master/Composite.Web.Css.Sass) package of the version **2.0.2 or more**.

In case your site has specific styles filetypes, but you have not installed demanded compilers, such files not to be used on the page.

## Description
The package provides scripts and styles bundling and minification. It allows to reduce the number of requests that the browser needs to get all of the resource files, and also to reduce the size of such files and, as a result, to load pages faster. This package does not minify inline scripts, inline styles, or HTML code itself.
A bundle with styles adding to the original page to the end of the HEAD part of the page. While a bundle with scripts adding to the end of the BODY part.

To exclude specific style or script from the bundling and minification process, add to such element c1-not-bundleminify attribute with "true" value.

To turn on or to turn off **styles** bundling and minification:
  - open the **Web.config** file in the root directory of a web site;
  - locate to the **configuration/appSettings** path;
  - find the key `Orckestra.Web.BundlingAndMinification.BundleAndMinifyStyles`;
  - set up `true` or `false` value.
  
  To turn on or to turn off **scripts** bundling and minification:
  - open the **Web.config** file in the root directory of a web site;
  - locate to the **configuration/appSettings** path;
  - find the key `Orckestra.Web.BundlingAndMinification.BundleAndMinifyScripts`;
  - set up `true` or `false` value.
  
  To turn on/off the package at all, turn on/off both styles and scripts settings.
  
  There is a list of predefined cases when the package does not bundle and minify scripts and styles of a page:
  - A request appears when the current user logged in to the admin console panel.
  - A request is related to the admin console panel.
  - A web site works in debugging mode. To check the debug mode status open the **Web.config** file in the root of a website and locate to the **configuration/system.web/compilation** path.

If none of these conditions is true, but there are no bundling and minification, be sure it is not a cached version of a page and check logs.
