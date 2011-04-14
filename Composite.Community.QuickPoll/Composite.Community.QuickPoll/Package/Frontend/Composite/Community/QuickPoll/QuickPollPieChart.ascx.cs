using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;
using Composite.Community.QuickPoll;
using Composite.Data;

public partial class Frontend_Composite_Community_QuickPoll_QuestionPieChart : QuestionControl
{
	protected void Page_Load(object sender, EventArgs e)
	{
		var answers = DataFacade.GetData<Answers>(d => d.QuestionRef == QuestionId);
		int totalVotes = answers.Sum(d => d.TotalVotes);
		if (totalVotes > 0)
		{
			QuickPollResultChart.Titles.Add("Voters: " + totalVotes.ToString());
			foreach (var answer in answers)
			{
				DataPoint point = new DataPoint();
				point.SetValueY(new object[] { (double)answer.TotalVotes / (double)totalVotes });
				point.LegendText = answer.AnswerText;
				QuickPollResultChart.Series["Default"].Points.Add(point);
			}
			// Enable 3D
			QuickPollResultChart.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
			QuickPollResultChart.Series["Default"]["CollectedThresholdUsePercent"] = "true";
		}
	}
}