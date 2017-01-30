<%@ WebHandler Language="C#" Class="Handler" %>

using System;
using System.Web;

public class Handler : IHttpHandler {

    public void ProcessRequest (HttpContext context)
    {
        // so ... you obviously never want this code on any production machine. Pure dev tooling for localhost dev.
        if (context.Request.Url.Host != "localhost")
        {
            throw new InvalidOperationException("Only served for localhost");
        }
        string path = context.Request["path"];
        context.Response.ContentType = "application/zip";
        context.Response.WriteFile(path);
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}