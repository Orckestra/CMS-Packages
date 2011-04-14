using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using Composite.Data;
using Composite.Functions;

namespace Composite.Community.QuickPoll
{
	public class Functions
	{
		public static Expression<Func<Nullable<DateTime>, Boolean>> ActiveNowFilter()
		{
			Expression<Func<Nullable<DateTime>, Boolean>> activeNowFilter = f => f.HasValue && f.Value < DateTime.Today.AddDays(1);
			return activeNowFilter;
		}

		[FunctionParameterDescription("answerId", "AnswerId", "Answer Id")]
		public static void IncreaseAnswerCounter(string answerId)
		{
			var result = (from r in DataFacade.GetData<Answers>()
						  where r.Id == new Guid(answerId)
						  select r).FirstOrDefault();
			if (result != null)
			{
				result.TotalVotes++;
				DataFacade.Update(result);
			}
		}

		[FunctionParameterDescription("questionId", "QuestionId", "Question Id")]
		public static Control LoadPieChart(Guid questionId)
		{
			return new UserControlPieChart(questionId);
		}
	}

	internal class UserControlPieChart : WebControl
	{
		private Guid questionId;

		public UserControlPieChart(Guid questionId)
		{
			this.questionId = questionId;
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			Control control = null;
			try
			{
				control = Page.LoadControl(@"~\Frontend\Composite\Community\QuickPoll\QuickPollPieChart.ascx");
			}
			catch (Exception ex)
			{
				control = new Literal()
				{
					Text = ex.Message
				};
			}
			var questionControl = control as QuestionControl;
			if (questionControl != null)
			{
				questionControl.QuestionId = questionId;
			}
			this.Controls.Add(control);
		}
	}
}
