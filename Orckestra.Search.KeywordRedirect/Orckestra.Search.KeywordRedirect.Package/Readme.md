# Install
1. Install package
2. Add Function 


# Develop
1. Copy/Link Source folder to root folder
2. Copy Link Package/Composite/InstalledPackages/Orckestra.Search.KeywordRedirect/view to Website


# Develop new package
1. "npm init"  - setup package
2. "npm install -D jspm@beta" (jspm 0.17 or higher, 0.17 in beta now)
> (optional) "npm install -g jspm@beta"
3. "jspm init" (shoud be used jspm 0.17 or higher)
>set relative path
4. jspm install react react-dom
5. jspm install npm:wampy
6. jspm install npm:c1-cms
7. jspm install --dev npm:babel-plugin-transform-react-jsx core-js
>  set configuration like here (in jspm.config.js) 
> https://jspm.io/0.17-beta-guide/installing-the-jsx-babel-plugin.html
8. jspm install npm:styled-components
9. jspm install react-redux
10. jspm install npm:redux-thunk
11. jspm install immutable
12. add polyfill for IE - "jspm install npm:es6-promise" and "jspm install npm:url-polyfill"


