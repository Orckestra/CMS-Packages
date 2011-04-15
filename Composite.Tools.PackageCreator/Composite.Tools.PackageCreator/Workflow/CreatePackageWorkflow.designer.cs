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

namespace Composite.Tools.PackageCreator
{
	partial class CreatePackageWorkflow
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
			this.faultHandlersActivity1 = new System.Workflow.ComponentModel.FaultHandlersActivity();
			this.setStateActivity6 = new System.Workflow.Activities.SetStateActivity();
			this.setStateActivity5 = new System.Workflow.Activities.SetStateActivity();
			this.savePackageCodeActivity = new System.Workflow.Activities.CodeActivity();
			this.ifElseBranchActivity2 = new System.Workflow.Activities.IfElseBranchActivity();
			this.ifElseBranchActivity1 = new System.Workflow.Activities.IfElseBranchActivity();
			this.ifElseActivity1 = new System.Workflow.Activities.IfElseActivity();
			this.nextHandleExternalEventActivity2 = new Composite.C1Console.Workflow.Activities.NextHandleExternalEventActivity();
			this.setStateActivity4 = new System.Workflow.Activities.SetStateActivity();
			this.cancelHandleExternalEventActivity1 = new Composite.C1Console.Workflow.Activities.CancelHandleExternalEventActivity();
			this.wizardFormActivity1 = new Composite.C1Console.Workflow.Activities.WizardFormActivity();
			this.packageInfoCodeActivity = new System.Workflow.Activities.CodeActivity();
			this.setStateActivity2 = new System.Workflow.Activities.SetStateActivity();
			this.NextDrivenActivity1 = new System.Workflow.Activities.EventDrivenActivity();
			this.CancelDrivenActivity2 = new System.Workflow.Activities.EventDrivenActivity();
			this.packageInfoInitialization = new System.Workflow.Activities.StateInitializationActivity();
			this.setStateActivity1 = new System.Workflow.Activities.SetStateActivity();
			this.cancelHandleExternalEventActivity2 = new Composite.C1Console.Workflow.Activities.CancelHandleExternalEventActivity();
			this.stateInitializationActivity = new System.Workflow.Activities.StateInitializationActivity();
			this.packageInfoState = new System.Workflow.Activities.StateActivity();
			this.globalCancelEventDrivenActivity = new System.Workflow.Activities.EventDrivenActivity();
			this.finalState = new System.Workflow.Activities.StateActivity();
			this.initializationState = new System.Workflow.Activities.StateActivity();
			// 
			// faultHandlersActivity1
			// 
			this.faultHandlersActivity1.Name = "faultHandlersActivity1";
			// 
			// setStateActivity6
			// 
			this.setStateActivity6.Name = "setStateActivity6";
			this.setStateActivity6.TargetStateName = "packageInfoState";
			// 
			// setStateActivity5
			// 
			this.setStateActivity5.Name = "setStateActivity5";
			this.setStateActivity5.TargetStateName = "finalState";
			// 
			// savePackageCodeActivity
			// 
			this.savePackageCodeActivity.Name = "savePackageCodeActivity";
			this.savePackageCodeActivity.ExecuteCode += new System.EventHandler(this.saveInfoCodeActivity_ExecuteCode);
			// 
			// ifElseBranchActivity2
			// 
			this.ifElseBranchActivity2.Activities.Add(this.setStateActivity6);
			this.ifElseBranchActivity2.Activities.Add(this.faultHandlersActivity1);
			this.ifElseBranchActivity2.Name = "ifElseBranchActivity2";
			// 
			// ifElseBranchActivity1
			// 
			this.ifElseBranchActivity1.Activities.Add(this.savePackageCodeActivity);
			this.ifElseBranchActivity1.Activities.Add(this.setStateActivity5);
			codecondition1.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.ValidateSave);
			this.ifElseBranchActivity1.Condition = codecondition1;
			this.ifElseBranchActivity1.Name = "ifElseBranchActivity1";
			// 
			// ifElseActivity1
			// 
			this.ifElseActivity1.Activities.Add(this.ifElseBranchActivity1);
			this.ifElseActivity1.Activities.Add(this.ifElseBranchActivity2);
			this.ifElseActivity1.Name = "ifElseActivity1";
			// 
			// nextHandleExternalEventActivity2
			// 
			this.nextHandleExternalEventActivity2.EventName = "Next";
			this.nextHandleExternalEventActivity2.InterfaceType = typeof(Composite.C1Console.Workflow.IFormsWorkflowEventService);
			this.nextHandleExternalEventActivity2.Name = "nextHandleExternalEventActivity2";
			// 
			// setStateActivity4
			// 
			this.setStateActivity4.Name = "setStateActivity4";
			this.setStateActivity4.TargetStateName = "finalState";
			// 
			// cancelHandleExternalEventActivity1
			// 
			this.cancelHandleExternalEventActivity1.EventName = "Cancel";
			this.cancelHandleExternalEventActivity1.InterfaceType = typeof(Composite.C1Console.Workflow.IFormsWorkflowEventService);
			this.cancelHandleExternalEventActivity1.Name = "cancelHandleExternalEventActivity1";
			// 
			// wizardFormActivity1
			// 
			this.wizardFormActivity1.ContainerLabel = null;
			this.wizardFormActivity1.FormDefinitionFileName = "\\InstalledPackages\\Composite.Tools.PackageCreator\\CreatePackage.xml";
			this.wizardFormActivity1.Name = "wizardFormActivity1";
			// 
			// packageInfoCodeActivity
			// 
			this.packageInfoCodeActivity.Name = "packageInfoCodeActivity";
			this.packageInfoCodeActivity.ExecuteCode += new System.EventHandler(this.packageInfoCodeActivity_ExecuteCode);
			// 
			// setStateActivity2
			// 
			this.setStateActivity2.Name = "setStateActivity2";
			this.setStateActivity2.TargetStateName = "packageInfoState";
			// 
			// NextDrivenActivity1
			// 
			this.NextDrivenActivity1.Activities.Add(this.nextHandleExternalEventActivity2);
			this.NextDrivenActivity1.Activities.Add(this.ifElseActivity1);
			this.NextDrivenActivity1.Name = "NextDrivenActivity1";
			// 
			// CancelDrivenActivity2
			// 
			this.CancelDrivenActivity2.Activities.Add(this.cancelHandleExternalEventActivity1);
			this.CancelDrivenActivity2.Activities.Add(this.setStateActivity4);
			this.CancelDrivenActivity2.Name = "CancelDrivenActivity2";
			// 
			// packageInfoInitialization
			// 
			this.packageInfoInitialization.Activities.Add(this.packageInfoCodeActivity);
			this.packageInfoInitialization.Activities.Add(this.wizardFormActivity1);
			this.packageInfoInitialization.Name = "packageInfoInitialization";
			// 
			// setStateActivity1
			// 
			this.setStateActivity1.Name = "setStateActivity1";
			this.setStateActivity1.TargetStateName = "finalState";
			// 
			// cancelHandleExternalEventActivity2
			// 
			this.cancelHandleExternalEventActivity2.EventName = "Cancel";
			this.cancelHandleExternalEventActivity2.InterfaceType = typeof(Composite.C1Console.Workflow.IFormsWorkflowEventService);
			this.cancelHandleExternalEventActivity2.Name = "cancelHandleExternalEventActivity2";
			// 
			// stateInitializationActivity
			// 
			this.stateInitializationActivity.Activities.Add(this.setStateActivity2);
			this.stateInitializationActivity.Name = "stateInitializationActivity";
			// 
			// packageInfoState
			// 
			this.packageInfoState.Activities.Add(this.packageInfoInitialization);
			this.packageInfoState.Activities.Add(this.CancelDrivenActivity2);
			this.packageInfoState.Activities.Add(this.NextDrivenActivity1);
			this.packageInfoState.Name = "packageInfoState";
			// 
			// globalCancelEventDrivenActivity
			// 
			this.globalCancelEventDrivenActivity.Activities.Add(this.cancelHandleExternalEventActivity2);
			this.globalCancelEventDrivenActivity.Activities.Add(this.setStateActivity1);
			this.globalCancelEventDrivenActivity.Name = "globalCancelEventDrivenActivity";
			// 
			// finalState
			// 
			this.finalState.Name = "finalState";
			// 
			// initializationState
			// 
			this.initializationState.Activities.Add(this.stateInitializationActivity);
			this.initializationState.Name = "initializationState";
			// 
			// CreatePackageWorkflow
			// 
			this.Activities.Add(this.initializationState);
			this.Activities.Add(this.finalState);
			this.Activities.Add(this.globalCancelEventDrivenActivity);
			this.Activities.Add(this.packageInfoState);
			this.CompletedStateName = "finalState";
			this.DynamicUpdateCondition = null;
			this.InitialStateName = "initializationState";
			this.Name = "CreatePackageWorkflow";
			this.CanModifyActivities = false;

		}

		#endregion

		private EventDrivenActivity globalCancelEventDrivenActivity;
		private StateActivity finalState;
		private Composite.C1Console.Workflow.Activities.WizardFormActivity wizardFormActivity1;
		private CodeActivity packageInfoCodeActivity;
		private SetStateActivity setStateActivity2;
		private StateInitializationActivity packageInfoInitialization;
		private SetStateActivity setStateActivity1;
		private StateInitializationActivity stateInitializationActivity;
		private StateActivity packageInfoState;
		private SetStateActivity setStateActivity4;
		private Composite.C1Console.Workflow.Activities.CancelHandleExternalEventActivity cancelHandleExternalEventActivity1;
		private Composite.C1Console.Workflow.Activities.NextHandleExternalEventActivity nextHandleExternalEventActivity2;
		private EventDrivenActivity CancelDrivenActivity2;
		private EventDrivenActivity NextDrivenActivity1;
		private Composite.C1Console.Workflow.Activities.CancelHandleExternalEventActivity cancelHandleExternalEventActivity2;
		private FaultHandlersActivity faultHandlersActivity1;
		private SetStateActivity setStateActivity6;
		private SetStateActivity setStateActivity5;
		private CodeActivity savePackageCodeActivity;
		private IfElseBranchActivity ifElseBranchActivity2;
		private IfElseBranchActivity ifElseBranchActivity1;
		private IfElseActivity ifElseActivity1;
		private StateActivity initializationState;














































	}
}
