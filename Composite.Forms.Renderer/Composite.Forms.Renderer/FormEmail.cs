using Composite.Core.Xml;

namespace Composite.Forms.Renderer
{
    internal class FormEmail
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Subject { get; set; }
        public XhtmlDocument Body { get; set; }
        public bool AppendFormData { get; set; }
    }
}
