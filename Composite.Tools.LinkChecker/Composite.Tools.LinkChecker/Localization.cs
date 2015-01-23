 
 
 
 
 
 
 
 
 
 


using Composite.Core.ResourceSystem;

namespace Composite.Tools.LinkChecker
{
	/// <summary>    
	/// Class generated from localization files  
    /// </summary>
    /// <exclude />
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)] 
	public static class Localization
	{
		 /// <summary>&quot;Link Checker&quot;</summary> 
 public static string LinkCheckerActionToken_Label { get { return T("LinkCheckerActionToken.Label"); } } 
 /// <summary>&quot;List Pages with invalid links to internal pages, media and external links.&quot;</summary> 
 public static string LinkCheckerActionToken_ToolTip { get { return T("LinkCheckerActionToken.ToolTip"); } } 
 /// <summary>&quot;&quot;</summary> 
 public static string BrokenLink_External { get { return T("BrokenLink.External"); } } 
 /// <summary>&quot;media file does not exist&quot;</summary> 
 public static string BrokenLink_MediaLibrary { get { return T("BrokenLink.MediaLibrary"); } } 
 /// <summary>&quot;page does not exist&quot;</summary> 
 public static string BrokenLink_Page { get { return T("BrokenLink.Page"); } } 
 /// <summary>&quot;page is not published&quot;</summary> 
 public static string BrokenLink_PageNotPublished { get { return T("BrokenLink.PageNotPublished"); } } 
 /// <summary>&quot;internal link is broken&quot;</summary> 
 public static string BrokenLink_Relative { get { return T("BrokenLink.Relative"); } } 
 /// <summary>&quot;Link Checker&quot;</summary> 
 public static string BrokenLinkReport_Title { get { return T("BrokenLinkReport.Title"); } } 
 /// <summary>&quot;The page is not a valid Xhtml document, failed to extract links.&quot;</summary> 
 public static string BrokenLinkReport_NotValidXhml { get { return T("BrokenLinkReport.NotValidXhml"); } } 
 /// <summary>&quot;Response code is {0}&quot;</summary> 
 public static string BrokenLinkReport_HttpStatus(object parameter0) { return string.Format(T("BrokenLinkReport.HttpStatus"), parameter0); } 
 /// <summary>&quot;No broken links found&quot;</summary> 
 public static string BrokenLinkReport_NoBrokenLinksFound { get { return T("BrokenLinkReport.NoBrokenLinksFound"); } } 
     private static string T(string key) 
       { 
            return StringResourceSystemFacade.GetString("Composite.Tools.LinkChecker", key);
        }
     public static class Constants 
		{
 /// <summary>&quot;Link Checker&quot;</summary> 
public const string LinkCheckerActionToken_Label = "${Composite.Tools.LinkChecker,LinkCheckerActionToken.Label}"; 
 /// <summary>&quot;List Pages with invalid links to internal pages, media and external links.&quot;</summary> 
public const string LinkCheckerActionToken_ToolTip = "${Composite.Tools.LinkChecker,LinkCheckerActionToken.ToolTip}"; 
 /// <summary>&quot;&quot;</summary> 
public const string BrokenLink_External = "${Composite.Tools.LinkChecker,BrokenLink.External}"; 
 /// <summary>&quot;media file does not exist&quot;</summary> 
public const string BrokenLink_MediaLibrary = "${Composite.Tools.LinkChecker,BrokenLink.MediaLibrary}"; 
 /// <summary>&quot;page does not exist&quot;</summary> 
public const string BrokenLink_Page = "${Composite.Tools.LinkChecker,BrokenLink.Page}"; 
 /// <summary>&quot;page is not published&quot;</summary> 
public const string BrokenLink_PageNotPublished = "${Composite.Tools.LinkChecker,BrokenLink.PageNotPublished}"; 
 /// <summary>&quot;internal link is broken&quot;</summary> 
public const string BrokenLink_Relative = "${Composite.Tools.LinkChecker,BrokenLink.Relative}"; 
 /// <summary>&quot;Link Checker&quot;</summary> 
public const string BrokenLinkReport_Title = "${Composite.Tools.LinkChecker,BrokenLinkReport.Title}"; 
 /// <summary>&quot;The page is not a valid Xhtml document, failed to extract links.&quot;</summary> 
public const string BrokenLinkReport_NotValidXhml = "${Composite.Tools.LinkChecker,BrokenLinkReport.NotValidXhml}"; 
 /// <summary>&quot;Response code is {0}&quot;</summary> 
public const string BrokenLinkReport_HttpStatus = "${Composite.Tools.LinkChecker,BrokenLinkReport.HttpStatus}"; 
 /// <summary>&quot;No broken links found&quot;</summary> 
public const string BrokenLinkReport_NoBrokenLinksFound = "${Composite.Tools.LinkChecker,BrokenLinkReport.NoBrokenLinksFound}"; 

		}

	}
}


