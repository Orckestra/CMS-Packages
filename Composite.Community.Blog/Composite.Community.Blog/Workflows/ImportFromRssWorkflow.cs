using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Workflow.Activities;
using System.Xml;
using Composite.C1Console.Trees;
using Composite.Community.Blog.Workflows.Helpers;
using Composite.Core;
using Composite.Data;
using Composite.Data.Types;

namespace Composite.Community.Blog.Workflows
{
	public sealed class ImportFromRssWorkflow: OneStepsWizardWorkflow
	{
		protected override void Step1Initialization_ExecuteCode(object sender, EventArgs e)
		{
			this.Bindings.Add("Path", string.Empty);
		}

		protected override void Step1_Validate(object sender, ConditionalEventArgs e)
		{

			try
			{
				var pageId = Guid.Empty;
				var path = GetBinding<string>("Path");


				var treeToken = EntityToken as TreeSimpleElementEntityToken;
				if (treeToken != null)
				{
					var pageToken = treeToken.ParentEntityToken as DataEntityToken;
					if (pageToken != null)
					{
						var page = pageToken.Data as IPage;
						if (page != null)
						{
							pageId = page.Id;
						}
					}
				}


				Verify.That(pageId != Guid.Empty, "Page not found");

				new BlogImportHelper().Import(path, pageId);

				e.Result = true;
			}
			catch (Exception ex)
			{
				ShowFieldMessage("Path", ex.Message);
				e.Result = false;
				Log.LogError("Blog Import From RSS", ex);
			}
		}

		protected override void Finish_ExecuteCode(object sender, EventArgs e)
		{
			var addNewTreeRefresher = this.CreateSpecificTreeRefresher();
			addNewTreeRefresher.PostRefreshMesseges(this.EntityToken);
		}

		public override string Step1_FormDefinitionFileName
		{
			get { return "/Composite.Community.Blog/ImportFromRss.xml"; }
		}
	}
}
