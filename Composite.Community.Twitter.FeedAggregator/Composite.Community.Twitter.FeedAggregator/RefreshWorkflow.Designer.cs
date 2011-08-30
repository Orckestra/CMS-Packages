using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Reflection;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;

namespace Composite.Community.Twitter.FeedAggregator
{
	partial class RefreshWorkflow
	{
		#region Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		[System.Diagnostics.DebuggerNonUserCode]
		[System.CodeDom.Compiler.GeneratedCode("", "")]
		private void InitializeComponent()
		{
			this.CanModifyActivities = true;
			this.setStateActivity2 = new System.Workflow.Activities.SetStateActivity();
			this.delayActivity = new System.Workflow.Activities.DelayActivity();
			this.refreshCodeActivity = new System.Workflow.Activities.CodeActivity();
			this.setStateActivity1 = new System.Workflow.Activities.SetStateActivity();
			this.delayBeforeStart = new System.Workflow.Activities.DelayActivity();
			this.stateInitializationCodeActivity = new System.Workflow.Activities.CodeActivity();
			this.timerEventDrivenActivity = new System.Workflow.Activities.EventDrivenActivity();
			this.refreshStateInitializationActivity = new System.Workflow.Activities.StateInitializationActivity();
			this.delayBeforeStartActivity = new System.Workflow.Activities.EventDrivenActivity();
			this.stateInitializationActivity = new System.Workflow.Activities.StateInitializationActivity();
			this.finishStateActivity = new System.Workflow.Activities.StateActivity();
			this.refreshStateActivity = new System.Workflow.Activities.StateActivity();
			this.refreshInitialState = new System.Workflow.Activities.StateActivity();
			// 
			// setStateActivity2
			// 
			this.setStateActivity2.Name = "setStateActivity2";
			this.setStateActivity2.TargetStateName = "refreshStateActivity";
			// 
			// delayActivity
			// 
			this.delayActivity.Name = "delayActivity";
			this.delayActivity.TimeoutDuration = System.TimeSpan.Parse("00:01:00");
			// 
			// refreshCodeActivity
			// 
			this.refreshCodeActivity.Name = "refreshCodeActivity";
			this.refreshCodeActivity.ExecuteCode += new System.EventHandler(this.refreshCodeActivity_ExecuteCode);
			// 
			// setStateActivity1
			// 
			this.setStateActivity1.Name = "setStateActivity1";
			this.setStateActivity1.TargetStateName = "refreshStateActivity";
			// 
			// delayBeforeStart
			// 
			this.delayBeforeStart.Name = "delayBeforeStart";
			this.delayBeforeStart.TimeoutDuration = System.TimeSpan.Parse("00:00:01");
			// 
			// stateInitializationCodeActivity
			// 
			this.stateInitializationCodeActivity.Name = "stateInitializationCodeActivity";
			this.stateInitializationCodeActivity.ExecuteCode += new System.EventHandler(this.stateInitializationCodeActivity_ExecuteCode);
			// 
			// timerEventDrivenActivity
			// 
			this.timerEventDrivenActivity.Activities.Add(this.delayActivity);
			this.timerEventDrivenActivity.Activities.Add(this.setStateActivity2);
			this.timerEventDrivenActivity.Name = "timerEventDrivenActivity";
			// 
			// refreshStateInitializationActivity
			// 
			this.refreshStateInitializationActivity.Activities.Add(this.refreshCodeActivity);
			this.refreshStateInitializationActivity.Name = "refreshStateInitializationActivity";
			// 
			// delayBeforeStartActivity
			// 
			this.delayBeforeStartActivity.Activities.Add(this.delayBeforeStart);
			this.delayBeforeStartActivity.Activities.Add(this.setStateActivity1);
			this.delayBeforeStartActivity.Name = "delayBeforeStartActivity";
			// 
			// stateInitializationActivity
			// 
			this.stateInitializationActivity.Activities.Add(this.stateInitializationCodeActivity);
			this.stateInitializationActivity.Name = "stateInitializationActivity";
			// 
			// finishStateActivity
			// 
			this.finishStateActivity.Name = "finishStateActivity";
			// 
			// refreshStateActivity
			// 
			this.refreshStateActivity.Activities.Add(this.refreshStateInitializationActivity);
			this.refreshStateActivity.Activities.Add(this.timerEventDrivenActivity);
			this.refreshStateActivity.Name = "refreshStateActivity";
			// 
			// refreshInitialState
			// 
			this.refreshInitialState.Activities.Add(this.stateInitializationActivity);
			this.refreshInitialState.Activities.Add(this.delayBeforeStartActivity);
			this.refreshInitialState.Name = "refreshInitialState";
			// 
			// RefreshWorkflow
			// 
			this.Activities.Add(this.refreshInitialState);
			this.Activities.Add(this.refreshStateActivity);
			this.Activities.Add(this.finishStateActivity);
			this.CompletedStateName = "finishStateActivity";
			this.DynamicUpdateCondition = null;
			this.InitialStateName = "refreshInitialState";
			this.Name = "RefreshWorkflow";
			this.CanModifyActivities = false;

		}

		#endregion

		private StateInitializationActivity stateInitializationActivity;

		private StateActivity finishStateActivity;

		private StateActivity refreshStateActivity;

		private CodeActivity stateInitializationCodeActivity;

		private StateInitializationActivity refreshStateInitializationActivity;

		private SetStateActivity setStateActivity2;

		private DelayActivity delayActivity;

		private CodeActivity refreshCodeActivity;

		private EventDrivenActivity timerEventDrivenActivity;

		private SetStateActivity setStateActivity1;

		private DelayActivity delayBeforeStart;

		private EventDrivenActivity delayBeforeStartActivity;

		private StateActivity refreshInitialState;
















	}
}
