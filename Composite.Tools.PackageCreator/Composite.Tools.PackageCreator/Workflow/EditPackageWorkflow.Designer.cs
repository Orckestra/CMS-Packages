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
    partial class EditPackageWorkflow
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
			this.SaveCodeActivity = new System.Workflow.Activities.CodeActivity();
			this.elseBranchActivity = new System.Workflow.Activities.IfElseBranchActivity();
			this.ifValidateActivity = new System.Workflow.Activities.IfElseBranchActivity();
			this.setStateActivity2 = new System.Workflow.Activities.SetStateActivity();
			this.ifElseActivity1 = new System.Workflow.Activities.IfElseActivity();
			this.saveHandleExternalEventActivity1 = new Composite.C1Console.Workflow.Activities.SaveHandleExternalEventActivity();
			this.documentFormActivity1 = new Composite.C1Console.Workflow.Activities.DocumentFormActivity();
			this.InitCodeActivity = new System.Workflow.Activities.CodeActivity();
			this.setStateActivity1 = new System.Workflow.Activities.SetStateActivity();
			this.cancelHandleExternalEventActivity1 = new Composite.C1Console.Workflow.Activities.CancelHandleExternalEventActivity();
			this.eventDrivenActivity_Save = new System.Workflow.Activities.EventDrivenActivity();
			this.EditStateInitializationActivity = new System.Workflow.Activities.StateInitializationActivity();
			this.GlobalEventDrivenActivity = new System.Workflow.Activities.EventDrivenActivity();
			this.FinishActivity = new System.Workflow.Activities.StateActivity();
			this.EditPackageWorkflowInitialState = new System.Workflow.Activities.StateActivity();
			// 
			// SaveCodeActivity
			// 
			this.SaveCodeActivity.Name = "SaveCodeActivity";
			this.SaveCodeActivity.ExecuteCode += new System.EventHandler(this.SaveCodeActivity_ExecuteCode);
			// 
			// elseBranchActivity
			// 
			this.elseBranchActivity.Name = "elseBranchActivity";
			// 
			// ifValidateActivity
			// 
			this.ifValidateActivity.Activities.Add(this.SaveCodeActivity);
			codecondition1.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.ValidateSave);
			this.ifValidateActivity.Condition = codecondition1;
			this.ifValidateActivity.Name = "ifValidateActivity";
			// 
			// setStateActivity2
			// 
			this.setStateActivity2.Name = "setStateActivity2";
			this.setStateActivity2.TargetStateName = "EditPackageWorkflowInitialState";
			// 
			// ifElseActivity1
			// 
			this.ifElseActivity1.Activities.Add(this.ifValidateActivity);
			this.ifElseActivity1.Activities.Add(this.elseBranchActivity);
			this.ifElseActivity1.Name = "ifElseActivity1";
			// 
			// saveHandleExternalEventActivity1
			// 
			this.saveHandleExternalEventActivity1.EventName = "Save";
			this.saveHandleExternalEventActivity1.InterfaceType = typeof(Composite.C1Console.Workflow.IFormsWorkflowEventService);
			this.saveHandleExternalEventActivity1.Name = "saveHandleExternalEventActivity1";
			// 
			// documentFormActivity1
			// 
			this.documentFormActivity1.ContainerLabel = null;
			this.documentFormActivity1.CustomToolbarDefinitionFileName = "\\InstalledPackages\\Composite.Tools.PackageCreator\\PackageInfoToolbar.xml";
			this.documentFormActivity1.FormDefinitionFileName = "\\InstalledPackages\\Composite.Tools.PackageCreator\\PackageInfo.xml";
			this.documentFormActivity1.Name = "documentFormActivity1";
			// 
			// InitCodeActivity
			// 
			this.InitCodeActivity.Name = "InitCodeActivity";
			this.InitCodeActivity.ExecuteCode += new System.EventHandler(this.InitCodeActivity_ExecuteCode);
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
			// eventDrivenActivity_Save
			// 
			this.eventDrivenActivity_Save.Activities.Add(this.saveHandleExternalEventActivity1);
			this.eventDrivenActivity_Save.Activities.Add(this.ifElseActivity1);
			this.eventDrivenActivity_Save.Activities.Add(this.setStateActivity2);
			this.eventDrivenActivity_Save.Name = "eventDrivenActivity_Save";
			// 
			// EditStateInitializationActivity
			// 
			this.EditStateInitializationActivity.Activities.Add(this.InitCodeActivity);
			this.EditStateInitializationActivity.Activities.Add(this.documentFormActivity1);
			this.EditStateInitializationActivity.Name = "EditStateInitializationActivity";
			// 
			// GlobalEventDrivenActivity
			// 
			this.GlobalEventDrivenActivity.Activities.Add(this.cancelHandleExternalEventActivity1);
			this.GlobalEventDrivenActivity.Activities.Add(this.setStateActivity1);
			this.GlobalEventDrivenActivity.Name = "GlobalEventDrivenActivity";
			// 
			// FinishActivity
			// 
			this.FinishActivity.Name = "FinishActivity";
			// 
			// EditPackageWorkflowInitialState
			// 
			this.EditPackageWorkflowInitialState.Activities.Add(this.EditStateInitializationActivity);
			this.EditPackageWorkflowInitialState.Activities.Add(this.eventDrivenActivity_Save);
			this.EditPackageWorkflowInitialState.Name = "EditPackageWorkflowInitialState";
			// 
			// EditPackageWorkflow
			// 
			this.Activities.Add(this.EditPackageWorkflowInitialState);
			this.Activities.Add(this.FinishActivity);
			this.Activities.Add(this.GlobalEventDrivenActivity);
			this.CompletedStateName = "FinishActivity";
			this.DynamicUpdateCondition = null;
			this.InitialStateName = "EditPackageWorkflowInitialState";
			this.Name = "EditPackageWorkflow";
			this.CanModifyActivities = false;

        }

        #endregion

		private EventDrivenActivity GlobalEventDrivenActivity;
		private StateActivity FinishActivity;
		private CodeActivity InitCodeActivity;
		private SetStateActivity setStateActivity1;
		private Composite.C1Console.Workflow.Activities.CancelHandleExternalEventActivity cancelHandleExternalEventActivity1;
		private StateInitializationActivity EditStateInitializationActivity;
		private IfElseBranchActivity elseBranchActivity;
		private IfElseBranchActivity ifValidateActivity;
		private IfElseActivity ifElseActivity1;
		private Composite.C1Console.Workflow.Activities.SaveHandleExternalEventActivity saveHandleExternalEventActivity1;
		private EventDrivenActivity eventDrivenActivity_Save;
		private SetStateActivity setStateActivity2;
		private Composite.C1Console.Workflow.Activities.DocumentFormActivity documentFormActivity1;
		private CodeActivity SaveCodeActivity;
		private StateActivity EditPackageWorkflowInitialState;














	}
}
