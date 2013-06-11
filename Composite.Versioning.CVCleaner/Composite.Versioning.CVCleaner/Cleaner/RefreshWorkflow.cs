using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Workflow.Activities;
using Composite.C1Console.Workflow;
using Composite.Core;
using Composite.Core.Threading;

namespace Composite.Versioning.ContentVersioning.Cleaner
{

	public delegate void RefreshFunctionDelegate();

	public sealed partial class RefreshWorkflow : StateMachineWorkflowActivity
	{
		public static void Run(int delay, RefreshFunctionDelegate refreshFunction)
		{
			var refreshWorkflow = WorkflowFacade.CreateNewWorkflow(
							typeof(RefreshWorkflow),
							new Dictionary<string, object> {
							{ "Delay", delay },
							{ "RefreshFunction", refreshFunction }
						}
						);

			refreshWorkflow.Start();

			WorkflowFacade.RunWorkflow(refreshWorkflow);
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Delay
		{
			get;
			set;
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RefreshFunctionDelegate RefreshFunction
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
			Log.LogVerbose("RefreshWorkflow", "Start refresh workflow for '{0}' function", RefreshFunction.Method.Name);
			DelayActivity delayActivity = (DelayActivity)this.GetActivityByName("delayActivity");
			delayActivity.TimeoutDuration = TimeSpan.FromSeconds(Delay);
		}

		private void refreshCodeActivity_ExecuteCode(object sender, EventArgs e)
		{
			using (ThreadDataManager.EnsureInitialize())
			{
				Log.LogVerbose("RefreshWorkflow", "Run '{0}' function", RefreshFunction.Method.Name);
				RefreshFunction();
			}
		}
	}
}
