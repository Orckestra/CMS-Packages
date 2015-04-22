using System.Workflow.Activities;

namespace Composite.Community.Blog.Workflows.Helpers
{
	partial class OneStepsWizardWorkflow
	{
		#region Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		[System.Diagnostics.DebuggerNonUserCode]
		private void InitializeComponent()
		{
			this.CanModifyActivities = true;
			System.Workflow.Activities.CodeCondition codecondition1 = new System.Workflow.Activities.CodeCondition();
			System.Workflow.ComponentModel.ActivityBind activitybind1 = new System.Workflow.ComponentModel.ActivityBind();
			this.setStateActivity8 = new System.Workflow.Activities.SetStateActivity();
			this.setStateActivity3 = new System.Workflow.Activities.SetStateActivity();
			this.FinishCodeActivity = new System.Workflow.Activities.CodeActivity();
			this.closeCurrentViewActivity1 = new Composite.C1Console.Workflow.Activities.CloseCurrentViewActivity();
			this.ifElseBranchActivity4 = new System.Workflow.Activities.IfElseBranchActivity();
			this.ifElseBranchActivity3 = new System.Workflow.Activities.IfElseBranchActivity();
			this.setStateActivity2 = new System.Workflow.Activities.SetStateActivity();
			this.cancelHandleExternalEventActivity2 = new Composite.C1Console.Workflow.Activities.CancelHandleExternalEventActivity();
			this.ifElseActivity2 = new System.Workflow.Activities.IfElseActivity();
			this.finishHandleExternalEventActivity1 = new Composite.C1Console.Workflow.Activities.FinishHandleExternalEventActivity();
			this.wizardSecondStepFormActivity = new Composite.C1Console.Workflow.Activities.WizardFormActivity();
			this.setStateActivity1 = new System.Workflow.Activities.SetStateActivity();
			this.codeActivity1 = new System.Workflow.Activities.CodeActivity();
			this.setStateActivity4 = new System.Workflow.Activities.SetStateActivity();
			this.cancelHandleExternalEventActivity1 = new Composite.C1Console.Workflow.Activities.CancelHandleExternalEventActivity();
			this.Step2_Cancel = new System.Workflow.Activities.EventDrivenActivity();
			this.Step2_Finish = new System.Workflow.Activities.EventDrivenActivity();
			this.stateInitializationActivity3 = new System.Workflow.Activities.StateInitializationActivity();
			this.stateInitializationActivity1 = new System.Workflow.Activities.StateInitializationActivity();
			this.GlobalEventDrivenActivity = new System.Workflow.Activities.EventDrivenActivity();
			this.Step1 = new System.Workflow.Activities.StateActivity();
			this.FinishState = new System.Workflow.Activities.StateActivity();
			this.InitializationState = new System.Workflow.Activities.StateActivity();
			// 
			// setStateActivity8
			// 
			this.setStateActivity8.Name = "setStateActivity8";
			this.setStateActivity8.TargetStateName = "Step1";
			// 
			// setStateActivity3
			// 
			this.setStateActivity3.Name = "setStateActivity3";
			this.setStateActivity3.TargetStateName = "FinishState";
			// 
			// FinishCodeActivity
			// 
			this.FinishCodeActivity.Name = "FinishCodeActivity";
			this.FinishCodeActivity.ExecuteCode += new System.EventHandler(this.Finish_ExecuteCode);
			// 
			// closeCurrentViewActivity1
			// 
			this.closeCurrentViewActivity1.Name = "closeCurrentViewActivity1";
			// 
			// ifElseBranchActivity4
			// 
			this.ifElseBranchActivity4.Activities.Add(this.setStateActivity8);
			this.ifElseBranchActivity4.Name = "ifElseBranchActivity4";
			// 
			// ifElseBranchActivity3
			// 
			this.ifElseBranchActivity3.Activities.Add(this.closeCurrentViewActivity1);
			this.ifElseBranchActivity3.Activities.Add(this.FinishCodeActivity);
			this.ifElseBranchActivity3.Activities.Add(this.setStateActivity3);
			codecondition1.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.Step1_Validate);
			this.ifElseBranchActivity3.Condition = codecondition1;
			this.ifElseBranchActivity3.Name = "ifElseBranchActivity3";
			// 
			// setStateActivity2
			// 
			this.setStateActivity2.Name = "setStateActivity2";
			this.setStateActivity2.TargetStateName = "FinishState";
			// 
			// cancelHandleExternalEventActivity2
			// 
			this.cancelHandleExternalEventActivity2.EventName = "Cancel";
			this.cancelHandleExternalEventActivity2.InterfaceType = typeof(Composite.C1Console.Workflow.IFormsWorkflowEventService);
			this.cancelHandleExternalEventActivity2.Name = "cancelHandleExternalEventActivity2";
			// 
			// ifElseActivity2
			// 
			this.ifElseActivity2.Activities.Add(this.ifElseBranchActivity3);
			this.ifElseActivity2.Activities.Add(this.ifElseBranchActivity4);
			this.ifElseActivity2.Name = "ifElseActivity2";
			// 
			// finishHandleExternalEventActivity1
			// 
			this.finishHandleExternalEventActivity1.EventName = "Finish";
			this.finishHandleExternalEventActivity1.InterfaceType = typeof(Composite.C1Console.Workflow.IFormsWorkflowEventService);
			this.finishHandleExternalEventActivity1.Name = "finishHandleExternalEventActivity1";
			// 
			// wizardSecondStepFormActivity
			// 
			this.wizardSecondStepFormActivity.ContainerLabel = "";
			activitybind1.Name = "OneStepsWizardWorkflow";
			activitybind1.Path = "Step1_FormDefinitionFileName";
			this.wizardSecondStepFormActivity.Name = "wizardSecondStepFormActivity";
			this.wizardSecondStepFormActivity.SetBinding(Composite.C1Console.Workflow.Activities.WizardFormActivity.FormDefinitionFileNameProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind1)));
			// 
			// setStateActivity1
			// 
			this.setStateActivity1.Name = "setStateActivity1";
			this.setStateActivity1.TargetStateName = "Step1";
			// 
			// codeActivity1
			// 
			this.codeActivity1.Name = "codeActivity1";
			this.codeActivity1.ExecuteCode += new System.EventHandler(this.Step1Initialization_ExecuteCode);
			// 
			// setStateActivity4
			// 
			this.setStateActivity4.Name = "setStateActivity4";
			this.setStateActivity4.TargetStateName = "FinishState";
			// 
			// cancelHandleExternalEventActivity1
			// 
			this.cancelHandleExternalEventActivity1.EventName = "Cancel";
			this.cancelHandleExternalEventActivity1.InterfaceType = typeof(Composite.C1Console.Workflow.IFormsWorkflowEventService);
			this.cancelHandleExternalEventActivity1.Name = "cancelHandleExternalEventActivity1";
			// 
			// Step2_Cancel
			// 
			this.Step2_Cancel.Activities.Add(this.cancelHandleExternalEventActivity2);
			this.Step2_Cancel.Activities.Add(this.setStateActivity2);
			this.Step2_Cancel.Name = "Step2_Cancel";
			// 
			// Step2_Finish
			// 
			this.Step2_Finish.Activities.Add(this.finishHandleExternalEventActivity1);
			this.Step2_Finish.Activities.Add(this.ifElseActivity2);
			this.Step2_Finish.Name = "Step2_Finish";
			// 
			// stateInitializationActivity3
			// 
			this.stateInitializationActivity3.Activities.Add(this.wizardSecondStepFormActivity);
			this.stateInitializationActivity3.Name = "stateInitializationActivity3";
			// 
			// stateInitializationActivity1
			// 
			this.stateInitializationActivity1.Activities.Add(this.codeActivity1);
			this.stateInitializationActivity1.Activities.Add(this.setStateActivity1);
			this.stateInitializationActivity1.Name = "stateInitializationActivity1";
			// 
			// GlobalEventDrivenActivity
			// 
			this.GlobalEventDrivenActivity.Activities.Add(this.cancelHandleExternalEventActivity1);
			this.GlobalEventDrivenActivity.Activities.Add(this.setStateActivity4);
			this.GlobalEventDrivenActivity.Name = "GlobalEventDrivenActivity";
			// 
			// Step1
			// 
			this.Step1.Activities.Add(this.stateInitializationActivity3);
			this.Step1.Activities.Add(this.Step2_Finish);
			this.Step1.Activities.Add(this.Step2_Cancel);
			this.Step1.Name = "Step1";
			// 
			// FinishState
			// 
			this.FinishState.Name = "FinishState";
			// 
			// InitializationState
			// 
			this.InitializationState.Activities.Add(this.stateInitializationActivity1);
			this.InitializationState.Name = "InitializationState";
			// 
			// OneStepsWizardWorkflow
			// 
			this.Activities.Add(this.InitializationState);
			this.Activities.Add(this.FinishState);
			this.Activities.Add(this.Step1);
			this.Activities.Add(this.GlobalEventDrivenActivity);
			this.CompletedStateName = "FinishState";
			this.DynamicUpdateCondition = null;
			this.InitialStateName = "InitializationState";
			this.Name = "OneStepsWizardWorkflow";
			this.CanModifyActivities = false;

		}

		#endregion

		private StateInitializationActivity stateInitializationActivity1;

		private StateActivity Step1;

		private StateActivity FinishState;

		private C1Console.Workflow.Activities.CancelHandleExternalEventActivity cancelHandleExternalEventActivity1;

		private EventDrivenActivity Step2_Cancel;

		private EventDrivenActivity Step2_Finish;

		private StateInitializationActivity stateInitializationActivity3;

		private EventDrivenActivity GlobalEventDrivenActivity;

		private SetStateActivity setStateActivity2;

		private CodeActivity FinishCodeActivity;

		private SetStateActivity setStateActivity3;

		private C1Console.Workflow.Activities.CancelHandleExternalEventActivity cancelHandleExternalEventActivity2;

		private SetStateActivity setStateActivity4;

		private C1Console.Workflow.Activities.CloseCurrentViewActivity closeCurrentViewActivity1;

		private C1Console.Workflow.Activities.FinishHandleExternalEventActivity finishHandleExternalEventActivity1;

		private CodeActivity codeActivity1;

		private C1Console.Workflow.Activities.WizardFormActivity wizardSecondStepFormActivity;

		private SetStateActivity setStateActivity8;

		private IfElseBranchActivity ifElseBranchActivity4;

		private IfElseBranchActivity ifElseBranchActivity3;

		private IfElseActivity ifElseActivity2;

		private SetStateActivity setStateActivity1;

		private StateActivity InitializationState;











































































	}
}
