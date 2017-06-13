using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Composite.Core.Xml;
using DocumentFormat.OpenXml.Packaging;
using System.Text.RegularExpressions;

namespace Composite.Media.WordDocumentViewer
{
    public class DocumentStyles
    {
        private static readonly XNamespace w = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
        private readonly Dictionary<Pair, string> _listFormats = new Dictionary<Pair, string>();
        private readonly Dictionary<string, string> _styleFormats = new Dictionary<string, string>();

        private readonly XElement _document;
        private readonly XElement _styles;
        private readonly Dictionary<string, int> _outlineLevel = new Dictionary<string, int>();
        private readonly IXmlNamespaceResolver _resolver;
        private readonly IXmlNamespaceResolver _stylesResolver;

        private readonly Stack<int> _lastLevelList = new Stack<int>();
        private int _lastLevel = -1;
        private readonly Dictionary<string, string> _levelNames = new Dictionary<string, string>();


        public DocumentStyles(WordprocessingDocument wpdocument)
        {
            _document = wpdocument.MainDocumentPart.NumberingDefinitionsPart.GetXElement();
            _styles = wpdocument.MainDocumentPart.StyleDefinitionsPart.GetXElement();
            _resolver = _document.CreateNavigator();
            _stylesResolver = _styles.CreateNavigator();
            foreach (var num in _document.XPathSelectElements("//w:num", _resolver))
            {
                var numId = num.XPathSelectAttributeValue("@w:numId", _resolver);
                var abstractNumId = num.XPathSelectAttributeValue("w:abstractNumId/@w:val", _resolver);

                var abstractNum = GetDescriptionNum(abstractNumId);
                var multiLevelType = abstractNum.XPathSelectAttributeValue(@"w:multiLevelType/@w:val", _resolver);

                foreach (var lvl in abstractNum.XPathSelectElements("w:lvl", _resolver))
                {
                    var ilvl = lvl.XPathSelectAttributeValue("@w:ilvl", _resolver);
                    var numFmt = lvl.XPathSelectAttributeValue("w:numFmt/@w:val", _resolver);

                    _listFormats[new Pair(numId, ilvl)] = numFmt;
                }
            }

            foreach (var style in _styles.XPathSelectElements("//w:style", _stylesResolver))
            {
                var numId = style.XPathSelectAttributeValue("w:pPr/w:numPr/w:numId/@w:val", _stylesResolver);
                if (string.IsNullOrEmpty(numId))
                    continue;
                //skip heading
                var outlineLvl = style.XPathSelectAttributeValue("w:pPr/w:outlineLvl/@w:val", _stylesResolver);

                var styleId = style.XPathSelectAttributeValue("@w:styleId", _stylesResolver);

                if (!string.IsNullOrEmpty(outlineLvl))
                {
                    if (int.TryParse(outlineLvl, out int outlineLvlValue))
                        _outlineLevel[styleId] = outlineLvlValue;
                    continue;
                }

                _styleFormats[styleId] = numId;
            }
        }



        private XElement GetDescriptionNum(string abstractNumId)
        {
            var abstractNum = _document.XPathSelectElement($"//w:abstractNum[@w:abstractNumId = '{abstractNumId}']", _resolver);
            var numStyleLink = abstractNum.XPathSelectAttributeValue("w:numStyleLink/@w:val", _resolver);
            if (numStyleLink != null)
            {
                var newNumId = _styles.XPathSelectAttributeValue(
                    $"//w:style[@w:styleId = '{numStyleLink}']/w:pPr/w:numPr/w:numId/@w:val", _stylesResolver);
                if (string.IsNullOrEmpty(newNumId))
                    return abstractNum;

                var newDescriptionNumId = _document.XPathSelectAttributeValue(
                    $"//w:num[@w:numId = '{newNumId}']/w:abstractNumId/@w:val", _resolver);
                if (string.IsNullOrEmpty(newDescriptionNumId))
                    return abstractNum;

                return GetDescriptionNum(newDescriptionNumId);
            }
            return abstractNum;
        }

        public XElement GetListElement(string numId, string ilvl)
        {
            var x = _listFormats.Where(d => d.Key.NumId == "5");

            if (_listFormats.ContainsKey(new Pair(numId, ilvl)))
            {
                var format = _listFormats[new Pair(numId, ilvl)];
                switch (format)
                {
                    case "decimal":
                    case "cardinalText":
                    case "ordinal":
                    case "ordinalText":
                    case "hex":
                        return new XElement(Namespaces.Xhtml + "ol");

                    case "upperLetter":
                        return new XElement(Namespaces.Xhtml + "ol",
                            new XAttribute("type", "A"));

                    case "lowerLetter":
                        return new XElement(Namespaces.Xhtml + "ol",
                            new XAttribute("type", "a"));

                    case "upperRoman":
                        return new XElement(Namespaces.Xhtml + "ol",
                            new XAttribute("type", "I"));

                    case "lowerRoman":
                        return new XElement(Namespaces.Xhtml + "ol",
                            new XAttribute("type", "i"));

                    case "chicago":
                    case "bullet":
                    default:
                        return new XElement(Namespaces.Xhtml + "ul");

                }
            }
            return new XElement(Namespaces.Xhtml + "ul");
        }

        public bool IsNumerable(string numId, string ilvl)
        {
            if (_listFormats.ContainsKey(new Pair(numId, ilvl)))
            {
                var format = _listFormats[new Pair(numId, ilvl)];
                switch (format)
                {
                    case "decimal":
                    case "cardinalText":
                    case "ordinal":
                    case "ordinalText":
                    case "hex":
                    case "upperLetter":
                    case "lowerLetter":
                    case "upperRoman":
                    case "lowerRoman":
                        return true;
                    default:
                        return false;
                }
            }
            return false;
        }

        public struct Pair
        {
            public Pair(string numId, string ilvl)
            {
                NumId = numId;
                Ilvl = ilvl;
            }

            public readonly string NumId;
            public readonly string Ilvl;
        }


        public bool IsListItem(XNode node)
        {
            if (node is XElement element)
            {
                if (element.Name.LocalName == "p")
                {
                    var numIdAttr = element.Attribute("numId");
                    if (numIdAttr != null)
                    {
                        return numIdAttr.Value != "0";
                    }

                    if (_styleFormats.ContainsKey(element.AttributeValue("class")))
                        return true;
                }
            }
            return false;
        }

        public bool IsListItem(XNode node, string numId, string ilvl)
        {
            if (IsListItem(node))
            {
                var element = node as XElement;
                var pair = GetListPair(element);
                return pair.NumId == numId && pair.Ilvl == ilvl;
            }
            return false;
        }

        public Pair GetListPair(XElement element)
        {
            if (element.Attribute("numId") != null)
                return new Pair(element.AttributeValue("numId"), element.AttributeValue("ilvl"));
            if (_styleFormats.ContainsKey(element.AttributeValue("class")))
                return new Pair(_styleFormats[element.AttributeValue("class")], "0");
            return new Pair();
        }

        public bool IsLevel(XNode node)
        {
            if (node is XElement element)
            {
                if (element.Name.LocalName == "h4")
                    return true;

                if (element.Name.LocalName == "p")
                {
                    var className = element.AttributeValue("class");
                    if (string.IsNullOrEmpty(className))
                        return false;

                    if (_outlineLevel.ContainsKey(className))
                    {
                        if (_outlineLevel[className] == 0)
                            return true;
                    }
                }
            }
            return false;
        }

        internal string GetNumeration(XElement element)
        {
            var className = element.AttributeValue("class"); ;
            var level = _outlineLevel[className];
            while (_lastLevel < level)
            {
                _lastLevelList.Push(0);
                _lastLevel++;
            }
            while (_lastLevel > level)
            {
                _lastLevelList.Pop();
                _lastLevel--;
            }
            if (_lastLevelList.Count > 0)
            {
                var index = _lastLevelList.Peek() + 1;
                _lastLevelList.Pop();
                _lastLevelList.Push(index);
                return string.Join(".", _lastLevelList.Select(i => i.ToString()).Reverse().ToArray());
            }
            return string.Empty;
        }

        internal string GetLevelToc(XElement level)
        {
            var tocId = level.Descendants(Namespaces.Xhtml + "a").Where(a => (a.AttributeValue("name") ?? string.Empty).StartsWith("_Toc")).Select(a => a.AttributeValue("name")).FirstOrDefault() ?? string.Empty;
            if (!_levelNames.ContainsKey(tocId))
            {
                var value = level.Value.Trim();
                value = Regex.Replace(value, @"[^\w\d\s]", "");
                value = Regex.Replace(value, @" ", "-");
                var index = 0;
                var indexvalue = value;
                while (_levelNames.ContainsValue(indexvalue))
                {
                    indexvalue = value + (++index);
                }
                _levelNames[tocId] = indexvalue;
            }
            return _levelNames[tocId];
        }

        internal string GetLevel(XElement element)
        {
            var className = element.AttributeValue("class");
            if (_outlineLevel.ContainsKey(className))
                return _outlineLevel[className].ToString();

            return string.Empty;
        }
    }
}
