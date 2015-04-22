using System;
using System.Workflow.Activities;

namespace Composite.Community.Blog.Workflows.Helpers
{
	/// <summary>
	/// Helper v0.1
	/// </summary>
	public abstract partial class OneStepsWizardWorkflow : Composite.C1Console.Workflow.Activities.FormsWorkflow
	{


		protected virtual void Step1Initialization_ExecuteCode(object sender, EventArgs e)
		{

		}

		protected virtual void Finish_ExecuteCode(object sender, EventArgs e)
		{

		}

		public OneStepsWizardWorkflow()
		{
			InitializeComponent();
		}

		public abstract string Step1_FormDefinitionFileName { get; }


		protected virtual void Step1_Validate(object sender, ConditionalEventArgs e)
		{

		}


	}
}
