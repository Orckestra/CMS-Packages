<%@ Control Language="C#" Inherits="Composite.Plugins.Forms.WebChannel.UiControlFactories.DataReferenceTreeSelectorTemplateUserControlBase" %>
<%@ Import Namespace="Composite.Plugins.Forms.WebChannel.UiControlFactories" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Composite.Data" %>
<%@ Import Namespace="Composite.Data.Types" %>
<%@ Import Namespace="Composite.Plugins.Functions.WidgetFunctionProviders.StandardWidgetFunctionProvider.DataReference" %>
<%@ Import Namespace="Composite.Data.Validation.ClientValidationRules" %>
<script runat="server">

	protected override void BindStateToProperties()
	{
		this.Selected = DataReferenceSelector.SelectedValue;
	}

	protected override void InitializeViewState()
	{
		PopulateSelector();
	}

	private bool IsMediaReference()
	{
		return DataType.Equals(typeof(IImageFile)) || DataType.Equals(typeof(IMediaFile));
	}

	private bool IsRequired()
	{
		return ClientValidationRules != null && ClientValidationRules.Any(rule => rule is NotNullClientValidationRule);
	}

	private void PopulateSelector()
	{
		if (!IsPostBack)
		{
			List<object> options = new List<object>();
			if (!IsRequired())
				options.Add(new KeyLabelPair(string.Empty, "<NONE>"));

			if (IsMediaReference())
			{
				Composite.C1Console.Elements.SearchToken searchToken = null;
				if (string.IsNullOrEmpty(this.SearchToken) == false)
				{
					searchToken = Composite.C1Console.Elements.SearchToken.Deserialize(this.SearchToken);
				}
				var filter = Composite.Forms.Renderer.FormsRenderer.GetPredicate(searchToken);

				options.AddRange(
					DataFacade.GetData<IMediaFile>()
					.ToList()
					.Select(data => new { Data = data, Label = data.GetLabel(true) })
					.Where(data => filter(data.Data))
					.OrderBy(data => data.Label)
					.Select(data => new KeyLabelPair(data.Data.KeyPath, data.Label))
				);
			}
			else
			{
				foreach (var item in DataReferenceSelectorWidgetFunction<IPage>.GetOptions(Composite.Core.Types.TypeManager.SerializeType(this.DataType)))
				{
					options.Add(item);
				}
			}
			DataReferenceSelector.DataSource = options;
			DataReferenceSelector.DataTextField = "Label";
			DataReferenceSelector.DataValueField = "Key";
			DataReferenceSelector.DataBind();
		}
	}

	public override string GetDataFieldClientName()
	{
		return this.ClientID;
	}

</script>
<asp:DropDownList runat="server" ID="DataReferenceSelector">
</asp:DropDownList>
