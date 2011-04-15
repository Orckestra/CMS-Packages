using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;

namespace Composite.Tools.PackageCreator.Workflow
{
    partial class UploadConfigWorkflow
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
			this.wizardFormActivity3 = new Composite.C1Console.Workflow.Activities.WizardFormActivity();
			this.wizardFormActivity2 = new Composite.C1Console.Workflow.Activities.WizardFormActivity();
			this.ifElseBranchActivity2 = new System.Workflow.Activities.IfElseBranchActivity();
			this.ifElseBranchActivity1 = new System.Workflow.Activities.IfElseBranchActivity();
			this.setStateActivity4 = new System.Workflow.Activities.SetStateActivity();
			this.finishHandleExternalEventActivity1 = new Composite.C1Console.Workflow.Activities.FinishHandleExternalEventActivity();
			this.ifElseActivity1 = new System.Workflow.Activities.IfElseActivity();
			this.AddConfig_CodeActivity = new System.Workflow.Activities.CodeActivity();
			this.setStateActivity3 = new System.Workflow.Activities.SetStateActivity();
			this.cancelHandleExternalEventActivity2 = new Composite.C1Console.Workflow.Activities.CancelHandleExternalEventActivity();
			this.setStateActivity5 = new System.Workflow.Activities.SetStateActivity();
			this.nextHandleExternalEventActivity1 = new Composite.C1Console.Workflow.Activities.NextHandleExternalEventActivity();
			this.wizardFormActivity1 = new Composite.C1Console.Workflow.Activities.WizardFormActivity();
			this.Upload_CodeActivity = new System.Workflow.Activities.CodeActivity();
			this.setStateActivity2 = new System.Workflow.Activities.SetStateActivity();
			this.setStateActivity1 = new System.Workflow.Activities.SetStateActivity();
			this.cancelHandleExternalEventActivity1 = new Composite.C1Console.Workflow.Activities.CancelHandleExternalEventActivity();
			this.eventDrivenActivity_Finish = new System.Workflow.Activities.EventDrivenActivity();
			this.stateInitializationActivity3 = new System.Workflow.Activities.StateInitializationActivity();
			this.eventDrivenActivity_Cancel = new System.Workflow.Activities.EventDrivenActivity();
			this.eventDrivenActivity_Next = new System.Workflow.Activities.EventDrivenActivity();
			this.stateInitializationActivity2 = new System.Workflow.Activities.StateInitializationActivity();
			this.stateInitializationActivity1 = new System.Workflow.Activities.StateInitializationActivity();
			this.GlobalCancelEventDrivenActivity = new System.Workflow.Activities.EventDrivenActivity();
			this.AddConfigActivity = new System.Workflow.Activities.StateActivity();
			this.UploadActivity = new System.Workflow.Activities.StateActivity();
			this.FinishActivity = new System.Workflow.Activities.StateActivity();
			this.InitialState = new System.Workflow.Activities.StateActivity();
			// 
			// wizardFormActivity3
			// 
			this.wizardFormActivity3.ContainerLabel = null;
			this.wizardFormActivity3.FormDefinitionFileName = "\\InstalledPackages\\Composite.Tools.PackageCreator\\UploadConfigError.xml";
			this.wizardFormActivity3.Name = "wizardFormActivity3";
			// 
			// wizardFormActivity2
			// 
			this.wizardFormActivity2.ContainerLabel = null;
			this.wizardFormActivity2.FormDefinitionFileName = "\\InstalledPackages\\Composite.Tools.PackageCreator\\UploadConfigSuccessfull.xml";
			this.wizardFormActivity2.Name = "wizardFormActivity2";
			// 
			// ifElseBranchActivity2
			// 
			this.ifElseBranchActivity2.Activities.Add(this.wizardFormActivity3);
			this.ifElseBranchActivity2.Name = "ifElseBranchActivity2";
			// 
			// ifElseBranchActivity1
			// 
			this.ifElseBranchActivity1.Activities.Add(this.wizardFormActivity2);
			codecondition1.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.DidValidate);
			this.ifElseBranchActivity1.Condition = codecondition1;
			this.ifElseBranchActivity1.Name = "ifElseBranchActivity1";
			// 
			// setStateActivity4
			// 
			this.setStateActivity4.Name = "setStateActivity4";
			this.setStateActivity4.TargetStateName = "FinishActivity";
			// 
			// finishHandleExternalEventActivity1
			// 
			this.finishHandleExternalEventActivity1.EventName = "Finish";
			this.finishHandleExternalEventActivity1.InterfaceType = typeof(Composite.C1Console.Workflow.IFormsWorkflowEventService);
			this.finishHandleExternalEventActivity1.Name = "finishHandleExternalEventActivity1";
			// 
			// ifElseActivity1
			// 
			this.ifElseActivity1.Activities.Add(this.ifElseBranchActivity1);
			this.ifElseActivity1.Activities.Add(this.ifElseBranchActivity2);
			this.ifElseActivity1.Name = "ifElseActivity1";
			// 
			// AddConfig_CodeActivity
			// 
			this.AddConfig_CodeActivity.Name = "AddConfig_CodeActivity";
			this.AddConfig_CodeActivity.ExecuteCode += new System.EventHandler(this.AddConfig_CodeActivity_ExecuteCode);
			// 
			// setStateActivity3
			// 
			this.setStateActivity3.Name = "setStateActivity3";
			this.setStateActivity3.TargetStateName = "FinishActivity";
			// 
			// cancelHandleExternalEventActivity2
			// 
			this.cancelHandleExternalEventActivity2.EventName = "Cancel";
			this.cancelHandleExternalEventActivity2.InterfaceType = typeof(Composite.C1Console.Workflow.IFormsWorkflowEventService);
			this.cancelHandleExternalEventActivity2.Name = "cancelHandleExternalEventActivity2";
			// 
			// setStateActivity5
			// 
			this.setStateActivity5.Name = "setStateActivity5";
			this.setStateActivity5.TargetStateName = "AddConfigActivity";
			// 
			// nextHandleExternalEventActivity1
			// 
			this.nextHandleExternalEventActivity1.EventName = "Next";
			this.nextHandleExternalEventActivity1.InterfaceType = typeof(Composite.C1Console.Workflow.IFormsWorkflowEventService);
			this.nextHandleExternalEventActivity1.Name = "nextHandleExternalEventActivity1";
			// 
			// wizardFormActivity1
			// 
			this.wizardFormActivity1.ContainerLabel = null;
			this.wizardFormActivity1.FormDefinitionFileName = "\\InstalledPackages\\Composite.Tools.PackageCreator\\UploadConfig.xml";
			this.wizardFormActivity1.Name = "wizardFormActivity1";
			// 
			// Upload_CodeActivity
			// 
			this.Upload_CodeActivity.Name = "Upload_CodeActivity";
			this.Upload_CodeActivity.ExecuteCode += new System.EventHandler(this.Upload_CodeActivity_ExecuteCode);
			// 
			// setStateActivity2
			// 
			this.setStateActivity2.Name = "setStateActivity2";
			this.setStateActivity2.TargetStateName = "UploadActivity";
			// 
			// setStateActivity1
			// 
			this.setStateActivity1.Name = "setStateActivity1";
			this.setStateActivity1.TargetStateName = "FinishActivity";
			// 
			// cancelHandleExternalEventActivity1
			// 
			this.cancelHandleExternalEventActivity1.EventName = "Cancel";
			this.cancelHandleExternalEventActivity1.InterfaceType = typeof(Composite.C1Console.Workflow.IFormsWorkflowEventService);
			this.cancelHandleExternalEventActivity1.Name = "cancelHandleExternalEventActivity1";
			// 
			// eventDrivenActivity_Finish
			// 
			this.eventDrivenActivity_Finish.Activities.Add(this.finishHandleExternalEventActivity1);
			this.eventDrivenActivity_Finish.Activities.Add(this.setStateActivity4);
			this.eventDrivenActivity_Finish.Name = "eventDrivenActivity_Finish";
			// 
			// stateInitializationActivity3
			// 
			this.stateInitializationActivity3.Activities.Add(this.AddConfig_CodeActivity);
			this.stateInitializationActivity3.Activities.Add(this.ifElseActivity1);
			this.stateInitializationActivity3.Name = "stateInitializationActivity3";
			// 
			// eventDrivenActivity_Cancel
			// 
			this.eventDrivenActivity_Cancel.Activities.Add(this.cancelHandleExternalEventActivity2);
			this.eventDrivenActivity_Cancel.Activities.Add(this.setStateActivity3);
			this.eventDrivenActivity_Cancel.Name = "eventDrivenActivity_Cancel";
			// 
			// eventDrivenActivity_Next
			// 
			this.eventDrivenActivity_Next.Activities.Add(this.nextHandleExternalEventActivity1);
			this.eventDrivenActivity_Next.Activities.Add(this.setStateActivity5);
			this.eventDrivenActivity_Next.Name = "eventDrivenActivity_Next";
			// 
			// stateInitializationActivity2
			// 
			this.stateInitializationActivity2.Activities.Add(this.Upload_CodeActivity);
			this.stateInitializationActivity2.Activities.Add(this.wizardFormActivity1);
			this.stateInitializationActivity2.Name = "stateInitializationActivity2";
			// 
			// stateInitializationActivity1
			// 
			this.stateInitializationActivity1.Activities.Add(this.setStateActivity2);
			this.stateInitializationActivity1.Name = "stateInitializationActivity1";
			// 
			// GlobalCancelEventDrivenActivity
			// 
			this.GlobalCancelEventDrivenActivity.Activities.Add(this.cancelHandleExternalEventActivity1);
			this.GlobalCancelEventDrivenActivity.Activities.Add(this.setStateActivity1);
			this.GlobalCancelEventDrivenActivity.Name = "GlobalCancelEventDrivenActivity";
			// 
			// AddConfigActivity
			// 
			this.AddConfigActivity.Activities.Add(this.stateInitializationActivity3);
			this.AddConfigActivity.Activities.Add(this.eventDrivenActivity_Finish);
			this.AddConfigActivity.Name = "AddConfigActivity";
			// 
			// UploadActivity
			// 
			this.UploadActivity.Activities.Add(this.stateInitializationActivity2);
			this.UploadActivity.Activities.Add(this.eventDrivenActivity_Next);
			this.UploadActivity.Activities.Add(this.eventDrivenActivity_Cancel);
			this.UploadActivity.Name = "UploadActivity";
			// 
			// FinishActivity
			// 
			this.FinishActivity.Name = "FinishActivity";
			// 
			// InitialState
			// 
			this.InitialState.Activities.Add(this.stateInitializationActivity1);
			this.InitialState.Name = "InitialState";
			// 
			// UploadConfigWorkflow
			// 
			this.Activities.Add(this.InitialState);
			this.Activities.Add(this.FinishActivity);
			this.Activities.Add(this.UploadActivity);
			this.Activities.Add(this.AddConfigActivity);
			this.Activities.Add(this.GlobalCancelEventDrivenActivity);
			this.CompletedStateName = "FinishActivity";
			this.DynamicUpdateCondition = null;
			this.InitialStateName = "InitialState";
			this.Name = "UploadConfigWorkflow";
			this.CanModifyActivities = false;

        }

        #endregion

		private StateActivity FinishActivity;
		private SetStateActivity setStateActivity2;
		private SetStateActivity setStateActivity1;
		private Composite.C1Console.Workflow.Activities.CancelHandleExternalEventActivity cancelHandleExternalEventActivity1;
		private StateInitializationActivity stateInitializationActivity3;
		private StateInitializationActivity stateInitializationActivity2;
		private StateInitializationActivity stateInitializationActivity1;
		private EventDrivenActivity GlobalCancelEventDrivenActivity;
		private StateActivity AddConfigActivity;
		private StateActivity UploadActivity;
		private EventDrivenActivity eventDrivenActivity_Cancel;
		private EventDrivenActivity eventDrivenActivity_Next;
		private SetStateActivity setStateActivity3;
		private Composite.C1Console.Workflow.Activities.CancelHandleExternalEventActivity cancelHandleExternalEventActivity2;
		private Composite.C1Console.Workflow.Activities.WizardFormActivity wizardFormActivity1;
		private SetStateActivity setStateActivity4;
		private Composite.C1Console.Workflow.Activities.FinishHandleExternalEventActivity finishHandleExternalEventActivity1;
		private Composite.C1Console.Workflow.Activities.WizardFormActivity wizardFormActivity2;
		private EventDrivenActivity eventDrivenActivity_Finish;
		private CodeActivity Upload_CodeActivity;
		private IfElseBranchActivity ifElseBranchActivity2;
		private IfElseBranchActivity ifElseBranchActivity1;
		private IfElseActivity ifElseActivity1;
		private SetStateActivity setStateActivity5;
		private Composite.C1Console.Workflow.Activities.NextHandleExternalEventActivity nextHandleExternalEventActivity1;
		private Composite.C1Console.Workflow.Activities.WizardFormActivity wizardFormActivity3;
		private CodeActivity AddConfig_CodeActivity;
		private StateActivity InitialState;










	}
}
