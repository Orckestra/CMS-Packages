using System;
using System.Web;
using System.Web.UI;

namespace Composite.Forms.Renderer
{
	public class ErrorSummary : IValidator
	{
		string _message;

		public static void AddError(string message)
		{
			Page currentPage = HttpContext.Current.Handler as Page;
			if (currentPage == null) throw new InvalidOperationException("The Current HttpContext Handler must be a System.Web.Ui.Page");

			ErrorSummary error = new ErrorSummary(message);
			currentPage.Validators.Add(error);
		}

		private ErrorSummary(string message)
		{
			_message = message;
		}

		public string ErrorMessage
		{
			get { return _message; }
			set { }
		}

		public bool IsValid
		{
			get { return false; }
			set { }
		}

		public void Validate()
		{ }
	}
}
