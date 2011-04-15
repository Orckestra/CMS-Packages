using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Composite.Forms.Renderer
{
	class FormEmailHeader
	{
		public string From { get; private set; }
		public string To { get; private set; }
		public string Cc { get; private set; }
		public string Subject { get; private set; }

		public FormEmailHeader(object From, object To, object Cc, object Subject)
		{
			this.From = From == null ? string.Empty : From.ToString();
			this.To = To == null ? string.Empty : To.ToString();
			this.Cc = Cc == null ? string.Empty : Cc.ToString();
			this.Subject = Subject == null ? string.Empty : Subject.ToString();
		}
	}
}
