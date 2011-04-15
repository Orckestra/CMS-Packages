using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using Composite.Data.ProcessControlled.ProcessControllers.GenericPublishProcessController;
using Composite.C1Console.Elements;
using Composite.C1Console.Elements.Plugins.ElementProvider;
using Composite.Tools.PackageCreator.ElementProvider.Action;
using Composite.Tools.PackageCreator.Workflow;
using Composite.Core.ResourceSystem;
using Composite.C1Console.Security;
using Composite.Plugins.Elements.ElementProviders.PageElementProvider;
using Composite.C1Console.Workflow;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Composite.Tools.PackageCreator.Types;
using Composite.C1Console.Users;
using System.Globalization;


namespace Composite.Tools.PackageCreator.ElementProvider
{

	public sealed class PackageCreatorElementProvider : IHooklessElementProvider
	{
		private ElementProviderContext _context;
		private List<EntityTokenHook> _currentEntityTokenHooks = null;

		private static readonly PermissionType[] _permissionTypes = { PermissionType.Administrate };
		private static readonly ActionGroup PrimaryActionGroup = new ActionGroup(ActionGroupPriority.PrimaryHigh);

		public PackageCreatorElementProvider()
		{

		}

		public ElementProviderContext Context
		{
			set { _context = value; }
		}


		public IEnumerable<Element> GetRoots(SearchToken seachToken)
		{
			Element element = new Element(_context.CreateElementHandle(new PackageCreatorElementProviderEntityToken()))
			{
				VisualData = new ElementVisualizedData()
				{
					Label = "Packages",
					ToolTip = "Packages",
					HasChildren = true,
					Icon = new ResourceHandle("Composite.Icons", "package-element-closed-availableitem"),
					OpenedIcon = new ResourceHandle("Composite.Icons", "package-element-closed-availableitem")
				}
			};
			element.AddAction(new ElementAction(new ActionHandle(new WorkflowActionToken(typeof(CreatePackageWorkflow), new PermissionType[] { PermissionType.Administrate })))
			{
				VisualData = new ActionVisualizedData
				{
					Label = PackageCreatorFacade.GetLocalization("CreatePackage.Label"),
					ToolTip = PackageCreatorFacade.GetLocalization("CreatePackage.ToolTip"),
					Icon = new ResourceHandle("Composite.Icons", "package-element-closed-availableitem"),
					ActionLocation = new ActionLocation
					{
						ActionType = PackageCreatorFacade.ActionType,
						IsInFolder = false,
						IsInToolbar = false,
						ActionGroup = new ActionGroup("Develop", ActionGroupPriority.PrimaryLow)
					}
				}
			});
			

			element.AddAction(new ElementAction(new ActionHandle(new WorkflowActionToken(typeof(UploadConfigWorkflow), new PermissionType[] { PermissionType.Administrate })))
			{
				VisualData = new ActionVisualizedData
				{
					Label = PackageCreatorFacade.GetLocalization("UploadConfig.Label"),
					ToolTip = PackageCreatorFacade.GetLocalization("UploadConfig.ToolTip"),
					Icon = new ResourceHandle("Composite.Icons", "package-install-local-package"),
					ActionLocation = new ActionLocation
					{
						ActionType = PackageCreatorFacade.ActionType,
						IsInFolder = false,
						IsInToolbar = false,
						ActionGroup = new ActionGroup("Develop", ActionGroupPriority.PrimaryLow)
					}
				}
			});

			if (UserSettings.CultureInfo.Name != "en-US")
			{
				element.AddAction(new ElementAction(new ActionHandle(new CreatePackageWorkflowActionToken(string.Format("Composite.LanguagePack.{0}", new CultureInfo( UserSettings.CultureInfo.TwoLetterISOLanguageName).EnglishName), new PackageCreatorActionToken("Localizations", UserSettings.CultureInfo.Name))))
				{
					VisualData = new ActionVisualizedData
					{
						Label = string.Format(PackageCreatorFacade.GetLocalization("CreateLocalizationPackage.Label"), StringResourceSystemFacade.GetString("Composite.Cultures", UserSettings.CultureInfo.Name)),
						ToolTip = string.Format(PackageCreatorFacade.GetLocalization("CreateLocalizationPackage.ToolTip"), StringResourceSystemFacade.GetString("Composite.Cultures", UserSettings.CultureInfo.Name)),
						Icon = new ResourceHandle("Composite.Icons", "package-element-closed-availableitem"),
						ActionLocation = new ActionLocation
						{
							ActionType = PackageCreatorFacade.ActionType,
							IsInFolder = false,
							IsInToolbar = false,
							ActionGroup = new ActionGroup("Develop", ActionGroupPriority.PrimaryLow)
						}
					}
				});
			}

			yield return element;

			var configuration = PackageCreatorFacade.GetConfigurationDocument();
			var root = configuration.Root;

			element = GetXmlNodeElement("/"+root.Name.ToString(), root.Name.ToString());
			yield return element;

		}

		private Element GetXmlNodeElement(string xpath, string name)
		{
			
			return new Element(_context.CreateElementHandle(new XmlNodeElementProviderEntityToken(xpath)))
			{
				VisualData = new ElementVisualizedData()
				{
					Label = name,
					ToolTip = name,
					HasChildren = true,
					Icon = new ResourceHandle("Composite.Icons", "folder"),
					OpenedIcon = new ResourceHandle("Composite.Icons", "folder-open")
				}
			};
		}

		private Element GetXmlAttributeElement(string xpath, string name)
		{

			return new Element(_context.CreateElementHandle(new XmlNodeAttributeProviderEntityToken(xpath)))
			{
				VisualData = new ElementVisualizedData()
				{
					Label = name,
					ToolTip = name,
					HasChildren = false,
					Icon = new ResourceHandle("Composite.Icons", "data"),
					OpenedIcon = new ResourceHandle("Composite.Icons", "data")
				}
			};
		}

		


		public IEnumerable<Element> GetChildren(EntityToken entityToken, SearchToken seachToken)
		{
			if (entityToken is PackageCreatorElementProviderEntityToken)
			{
				IEnumerable<string> packages = PackageCreatorFacade.GetPackageNames();
				foreach (var package in packages)
				{
					var icon = (PackageCreatorFacade.ActivePackageName == package) ? GenericPublishProcessController.Publish : new ResourceHandle("Composite.Icons", "page-publication");
					var element = new Element(_context.CreateElementHandle(new PackageCreatorPackageElementProviderEntityToken(package)))
					{
						VisualData = new ElementVisualizedData()
						{
							Label = package,
							ToolTip = package,
							HasChildren = true,
							Icon = icon,
							OpenedIcon = icon
						}
					};
					element.AddAction(new ElementAction(new ActionHandle(new WorkflowActionToken(typeof(EditPackageWorkflow), new PermissionType[] { PermissionType.Administrate })))
					{
						VisualData = new ActionVisualizedData
						{
							Label = "Edit",
							ToolTip = "Edit",
							Icon = new ResourceHandle("Composite.Icons", "page-edit-page"),
							ActionLocation = new ActionLocation
							{
								ActionType = ActionType.Other,
								IsInFolder = false,
								IsInToolbar = false,
								ActionGroup = new ActionGroup("Develop", ActionGroupPriority.PrimaryLow)
							}
						}
					});
					element.AddAction(new ElementAction(new ActionHandle(new SetActivePackageActionToken()))
					{
						VisualData = new ActionVisualizedData
						{
							Label = "Set Active",
							ToolTip = "Set Active",
							Icon = new ResourceHandle("Composite.Icons", "accept"),
							ActionLocation = new ActionLocation
							{
								ActionType = ActionType.Other,
								IsInFolder = false,
								IsInToolbar = false,
								ActionGroup = new ActionGroup("Develop", ActionGroupPriority.PrimaryLow)
							}
						}
					});

					element.AddAction(new ElementAction(new ActionHandle(new ConfirmWorkflowActionToken("Are you sure?", typeof(DeleteConfigPackageCreatorActionToken))))
					{
						VisualData = new ActionVisualizedData
						{
							Label = "Delete",
							ToolTip = "Delete",
							Icon = new ResourceHandle("Composite.Icons", "generated-type-delete"),
							ActionLocation = new ActionLocation
							{
								ActionType = PackageCreatorFacade.ActionType,
								IsInFolder = false,
								IsInToolbar = false,
								ActionGroup = new ActionGroup("Other", ActionGroupPriority.PrimaryLow)
							}
						}
					});


					element.AddAction(new ElementAction(new ActionHandle(new DownloadPackageActionToken("package")))
					{
						VisualData = new ActionVisualizedData
						{
							Label = PackageCreatorFacade.GetLocalization("DownloadPackageButton.Label"),
							ToolTip = PackageCreatorFacade.GetLocalization("DownloadPackageButton.ToolTip"),
							Icon = new ResourceHandle("Composite.Icons", "package-install-local-package"),
							ActionLocation = new ActionLocation
							{
								ActionType = PackageCreatorFacade.ActionType,
								IsInFolder = false,
								IsInToolbar = false,
								ActionGroup = new ActionGroup("Download", ActionGroupPriority.PrimaryLow)
							}
						}
					});
					element.AddAction(new ElementAction(new ActionHandle(new DownloadPackageActionToken("config")))
					{
						VisualData = new ActionVisualizedData
						{
							Label = PackageCreatorFacade.GetLocalization("DownloadConfigButton.Label"),
							ToolTip = PackageCreatorFacade.GetLocalization("DownloadConfigButton.ToolTip"),
							Icon = new ResourceHandle("Composite.Icons", "package-install-local-package"),
							ActionLocation = new ActionLocation
							{
								ActionType = PackageCreatorFacade.ActionType,
								IsInFolder = false,
								IsInToolbar = false,
								ActionGroup = new ActionGroup("Download", ActionGroupPriority.PrimaryLow)
							}
						}
					});
					
					yield return element;
				}
			}
			else if (entityToken is PackageCreatorPackageElementProviderEntityToken)
			{
				var categories = PackageCreatorFacade.GetCategories(entityToken.Source);
				foreach (var category in categories)
				{
					var element = new Element(_context.CreateElementHandle(new PackageCreatorCategoryElementProviderEntityToken(entityToken.Source, category.Name)))
					{
						VisualData = new ElementVisualizedData()
						{
							Label = category.GetLabel(),
							ToolTip = category.GetLabel(),
							HasChildren = true,
							Icon = new ResourceHandle("Composite.Icons", "folder"),
							OpenedIcon = new ResourceHandle("Composite.Icons", "folder-open")
						}
					};
					yield return element;
				}

			}
			else if (entityToken is PackageCreatorCategoryElementProviderEntityToken)
			{
				var items = PackageCreatorFacade.GetItems(entityToken.Type, entityToken.Source);
				foreach (var item in items.OrderBy(d => d.Name))
				{
					var element = new Element(_context.CreateElementHandle(new PackageCreatorItemElementProviderEntityToken(item.Name, entityToken.Source, entityToken.Type)))
					{
						VisualData = new ElementVisualizedData()
						{
							Label = item.Name,
							ToolTip = item.Name,
							HasChildren = false,
							Icon = item.ItemIcon,
							OpenedIcon = item.ItemIcon
						}
					};
					element.AddAction(new ElementAction(new ActionHandle(new DeleteItemPackageCreatorActionToken()))
					{
						VisualData = new ActionVisualizedData
						{
							Label = "Delete",
							ToolTip = "Delete",
							Icon = new ResourceHandle("Composite.Icons", "page-delete-page"),
							ActionLocation = new ActionLocation
							{
								ActionType = ActionType.Other,
								IsInFolder = false,
								IsInToolbar = false,
								ActionGroup = new ActionGroup("Develop", ActionGroupPriority.PrimaryLow)
							}
						}
					});
					yield return element;
				}
			}
			else if (entityToken is XmlNodeElementProviderEntityToken)
			{
				var configuration = PackageCreatorFacade.GetConfigurationDocument();
				var xpath = (entityToken as XmlNodeElementProviderEntityToken).XPath;
				var element = configuration.XPathSelectElement(xpath);
				var counter = new Dictionary<string, int>();


				if (element != null)
				{

					foreach (XElement item in element.Elements())
					{
						string position = string.Empty;
						string name = item.Name.ToString();
						if (counter.ContainsKey(name))
						{
							position = string.Format("[{0}]", ++counter[name]);
						}
						else
						{
							counter.Add(name, 1);
						}
						if (item.Attribute("name") != null)
						{
							position = string.Format("[@name='{0}']", item.Attribute("name").Value);
						}

						string description = item.IndexAttributeValue();
						yield return GetXmlNodeElement(string.Format("{0}/{1}{2}", xpath, name, position), name + (string.IsNullOrEmpty(description) ? string.Empty : string.Format("({0})", description)));
					}
					var elementPath = Regex.Replace(xpath, @"\[[^\]]*\]$","" );
					foreach (var attribute in element.Attributes())
					{
						string name = attribute.Name.ToString();
						yield return GetXmlAttributeElement(string.Format("{0}[@{1}='{2}']", elementPath, name, attribute.Value), string.Format("{0}={1}", name, attribute.Value));
					}
				}
			}
		}

		public IEnumerable<LabeledProperty> GetLabeledProperties(EntityToken entityToken)
		{
			yield break;
		}

		public List<EntityTokenHook> GetHooks()
		{
			return this.CurrentEntityTokenHooks;
		}

		private List<EntityTokenHook> CurrentEntityTokenHooks
		{
			get
			{
				if (_currentEntityTokenHooks == null)
				{
					_currentEntityTokenHooks = CreateHooks();
				}
				return _currentEntityTokenHooks;
			}
		}

		private List<EntityTokenHook> CreateHooks()
		{
			EntityTokenHook entityTokenHook = new EntityTokenHook(new PackageCreatorElementProviderEntityToken());
			return new List<EntityTokenHook> { entityTokenHook };
		}
	}
}
