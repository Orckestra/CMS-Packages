1. Require Composite.C1Console.Forms.NestedHtmlForm 1.0.5 and higher
2. Reqiure Config  MVC 

web.config
<configuration>
        <system.web>
                <compilation>
                        <assemblies>
                                <add assembly="System.Web.Descriptionions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35" />
                                <add assembly="System.Web.Routing, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35" />
                                <add assembly="System.Data.DataSetExtensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089" />
                                <add assembly="System.Data.Linq, Version=3.5.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089" />
                                <add assembly="System.Web.Mvc, Version=2.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35" />
                        <assemblies>
               </compilation>
               <pages>
                    <namespaces>
                            <add namespace="System.Web.Mvc" />
                            <add namespace="System.Web.Mvc.Ajax" />
                            <add namespace="System.Web.Mvc.Html" />
                            <add namespace="System.Web.Routing" />
                            <add namespace="System.Linq" />
                            <add namespace="System.Collections.Generic" />
                    </namespaces>
                    
3. Routes must be registered in \App_Code\Composite\AspNet\MvcPlayer\Route.cs  instead Global Asax

	