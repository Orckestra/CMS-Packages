using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Workflow.Activities;
using Composite.Core.Logging;
using Composite.Core.Threading;

namespace Composite.Community.Twitter.FeedAggregator
{
	public sealed partial class RefreshWorkflow : StateMachineWorkflowActivity
	{

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Delay
		{
			get;
			set;
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<string> Queries
		{
			get;
			set;
		}

		public RefreshWorkflow()
		{
			InitializeComponent();
		}

		private void stateInitializationCodeActivity_ExecuteCode(object sender, EventArgs e)
		{
			DelayActivity delayActivity = (DelayActivity)this.GetActivityByName("delayActivity");
			delayActivity.TimeoutDuration = TimeSpan.FromSeconds(Delay);
			LoggingService.LogVerbose("FeedAggregator", string.Format("Start RefressWorkfwow"));
		}

		private void refreshCodeActivity_ExecuteCode(object sender, EventArgs e)
		{
			using (ThreadDataManager.EnsureInitialize())
			{
				FeedAggregatorFacade.UpdateFeeds(Queries);
				LoggingService.LogVerbose("FeedAggregator", string.Format("Update feeds: {0}", string.Join(", ", Queries)));
			}
		}
	}
}
