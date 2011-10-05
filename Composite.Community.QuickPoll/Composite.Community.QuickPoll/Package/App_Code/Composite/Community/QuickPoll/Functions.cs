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

	}

}
