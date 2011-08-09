<%@ WebHandler Language="C#" Class="BlogCommentsRssFeedHttpHandler" %>
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Web;
using System.Xml.Linq;

using Composite.Core;
using Composite.Data;
using Composite.Functions;

public class BlogCommentsRssFeedHttpHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        // Get the name of the function to execute by copying the current file name (without the .ashx extension)
        string functionName = "Composite.Community.Blog.CommentsRss";
        // Locate the data culture to use - like en-US or nl-NL
        CultureInfo dataCulture = GetCurrentDataCulture(context);

        using (DataScope dataScope = new DataScope(DataScopeIdentifier.Public, dataCulture))
        {
            // Grab a function object to execute
            IFunction function = FunctionFacade.GetFunction(functionName);

            // Execute the function, passing all query string parameters as input parameters
            object functionResult = FunctionFacade.Execute<object>(function, context.Request.QueryString);

            // output result
            if (functionResult != null)
            {
                context.Response.Write(functionResult.ToString());
                if (functionResult is XNode && function.ReturnType != typeof(Composite.Core.Xml.XhtmlDocument)) 
        			context.Response.ContentType = "text/xml";
            }
        }
    }

    
    public bool IsReusable
    {
        get
        {
            return true;
        }
    }


    /// <summary>
    /// Locates the data scope culture that the function should be scoped to.
    /// I.e. the 'language' of the data to work on. 
    /// The Query String variable 'cultureScope' will be used, or the systems 
    /// default culure is this parameter is not specified.
    /// </summary>
    private CultureInfo GetCurrentDataCulture(HttpContext context)
    {
        string cultureScope = context.Request.QueryString["cultureScope"];
        if (string.IsNullOrEmpty(cultureScope)==false)
        {
            return new CultureInfo(cultureScope);
        }
        
        return DataLocalizationFacade.DefaultLocalizationCulture;
    }
}