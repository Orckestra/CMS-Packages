using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using Composite.C1Console.Elements;
using Composite.C1Console.Elements.Plugins.ElementProvider;
using Composite.C1Console.Security;
using Composite.C1Console.Users;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;
using Composite.Data.ProcessControlled.ProcessControllers.GenericPublishProcessController;
using Composite.Tools.PackageCreator.Actions;
using Composite.Tools.PackageCreator.ElementProvider.Actions;
using Composite.Tools.PackageCreator.ElementProvider.EntityTokens;
using Composite.Tools.PackageCreator.Types;
using Composite.Tools.PackageCreator.Workflow;


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
                    Label = PackageCreatorFacade.GetLocalization("PackageCreatorElementProviderEntityToken.Label"),
                    ToolTip = PackageCreatorFacade.GetLocalization("PackageCreatorElementProviderEntityToken.ToolTip"),
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

            //if (UserSettings.CultureInfo.Name != "en-US")
            if (UserSettings.C1ConsoleUiLanguage.Name != "en-US")
            {
                element.AddAction(new ElementAction(new ActionHandle(new CreatePackageWorkflowActionToken(string.Format("Composite.LanguagePack.{0}", new CultureInfo(UserSettings.C1ConsoleUiLanguage.TwoLetterISOLanguageName).EnglishName), new AddLocalizationActionToken(UserSettings.C1ConsoleUiLanguage.Name))))
                {
                    VisualData = new ActionVisualizedData
                    {
                        Label = string.Format(PackageCreatorFacade.GetLocalization("CreateLocalizationPackage.Label"), StringResourceSystemFacade.GetString("Composite.Cultures", UserSettings.C1ConsoleUiLanguage.Name)),
                        ToolTip = string.Format(PackageCreatorFacade.GetLocalization("CreateLocalizationPackage.ToolTip"), StringResourceSystemFacade.GetString("Composite.Cultures", UserSettings.C1ConsoleUiLanguage.Name)),
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

            yield return GetXmlNodeElement(PCCompositeConfig.Source, string.Empty, PCCompositeConfig.Source);
            yield return GetXmlNodeElement(PCWebConfig.Source, string.Empty, PCWebConfig.Source);
        }

        private Element GetXmlNodeElement(string source, string xpath, string name)
        {

            return new Element(_context.CreateElementHandle(new XmlNodeElementProviderEntityToken(xpath, source)))
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

        private Element GetXmlAttributeElement(string source, string xpath, string name)
        {

            var element = new Element(_context.CreateElementHandle(new XmlNodeAttributeProviderEntityToken(xpath, source)))
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
            return element;
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
                            Label = PackageCreatorFacade.GetLocalization("EditPackage.Label"),
                            ToolTip = PackageCreatorFacade.GetLocalization("EditPackage.ToolTip"),
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
                            Label = PackageCreatorFacade.GetLocalization("SetActivePackage.Label"),
                            ToolTip = PackageCreatorFacade.GetLocalization("SetActivePackage.ToolTip"),
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
                            Label = PackageCreatorFacade.GetLocalization("DeleteConfigPackageCreator.Label"),
                            ToolTip = PackageCreatorFacade.GetLocalization("DeleteConfigPackageCreator.ToolTip"),
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
                            Label = item.GetLabel(),
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
                            Label = PackageCreatorFacade.GetLocalization("DeleteItemPackageCreator.Label"),
                            ToolTip = PackageCreatorFacade.GetLocalization("DeleteItemPackageCreator.ToolTip"),
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
                var configuration = PackageCreatorFacade.GetConfigurationDocument(entityToken.Source);
                var xpath = (entityToken as XmlNodeElementProviderEntityToken).XPath;
                XContainer container = xpath == string.Empty ? configuration as XContainer : configuration.XPathSelectElement(xpath) as XContainer;
                var element = container as XElement;
                var counter = new Dictionary<string, int>();


                if (container != null)
                {
                    var xmlElements = new List<Element>();
                    foreach (XElement item in container.Elements())
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

                        xmlElements.Add(GetXmlNodeElement(entityToken.Source, string.Format("{0}/{1}{2}", xpath, name, position), name + (string.IsNullOrEmpty(description) ? string.Empty : string.Format("({0})", description))));
                    }

                    xmlElements.Sort((a, b) => string.Compare(a.VisualData.Label,
                                                              b.VisualData.Label,
                                                              StringComparison.InvariantCulture));

                    foreach (var e in xmlElements) yield return e;
                }

                if (element != null)
                {
                    var elementPath = Regex.Replace(xpath, @"\[[^\]]*\]$", "");
                    foreach (var attribute in element.Attributes())
                    {
                        string name = attribute.Name.ToString();
                        yield return GetXmlAttributeElement(entityToken.Source, string.Format("{0}[@{1}='{2}']", elementPath, name, attribute.Value), string.Format("{0}={1}", name, attribute.Value));
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
