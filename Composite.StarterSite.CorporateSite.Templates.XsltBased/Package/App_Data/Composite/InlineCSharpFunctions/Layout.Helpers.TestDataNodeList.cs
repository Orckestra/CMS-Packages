using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Composite.Data;
using Composite.Data.Types;

namespace Layout.Helpers
{
  public static class InlineMethodFunction
  {
    public static IEnumerable<XNode> TestDataNodeList()
    {
      XNamespace xhtml = "http://www.w3.org/1999/xhtml";
      yield return new XComment("Test data start");
      yield return new XElement(xhtml + "h1", "This is test data");
      yield return new XElement(xhtml + "p", "This is just test data for preview purposes...");
       yield return new XComment("Test data end");

    }
  }
}
