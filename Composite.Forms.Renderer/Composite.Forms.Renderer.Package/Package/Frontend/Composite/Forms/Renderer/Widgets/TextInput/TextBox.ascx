<%@ Control Language="C#" Inherits="Composite.Plugins.Forms.WebChannel.UiControlFactories.TextInputTemplateUserControlBase"  %>
<%@ Import Namespace="Composite.Plugins.Forms.WebChannel.UiControlFactories" %>
<%@ Import Namespace="Composite.C1Console.Forms.CoreUiControls" %>
<%@ Import Namespace="Composite.Data.Validation.ClientValidationRules" %>

<script runat="server">
	private string _currentStringValue = null;

	protected void Page_Init(object sender, EventArgs e)
	{
		if (_currentStringValue == null)
		{
			_currentStringValue = Request.Form[this.UniqueID];
		}
	}

	protected override void BindStateToProperties()
	{
		this.Text = _currentStringValue;
	}

	protected override void InitializeViewState()
	{
		_currentStringValue = this.Text;
	}

	public override string GetDataFieldClientName()
	{
		return this.ClientID.Replace("_","$");
	}

	private string TypeParam()
	{
		var classes = new HashSet<string>();
		var type = string.Empty;
		var @readonly = false;
		switch (this.Type)
		{
			case Composite.C1Console.Forms.CoreUiControls.TextBoxType.Password:
				type = "password";
				break;
			case Composite.C1Console.Forms.CoreUiControls.TextBoxType.Integer:
				classes.Add("integer");
				break;
			case Composite.C1Console.Forms.CoreUiControls.TextBoxType.Decimal:
				classes.Add("number");
				break;
			case Composite.C1Console.Forms.CoreUiControls.TextBoxType.ProgrammingIdentifier:
				classes.Add("programmingidentifier");
				break;
			case Composite.C1Console.Forms.CoreUiControls.TextBoxType.ProgrammingNamespace:
				classes.Add("programmingnamespace");
				break;
			case Composite.C1Console.Forms.CoreUiControls.TextBoxType.ReadOnly:
				@readonly = true;
				break;
			case Composite.C1Console.Forms.CoreUiControls.TextBoxType.String:
			default:
				break;
		}

		return (classes.Count > 0 ? string.Format(@" class=""{0}""", string.Join(" ", classes)) : string.Empty)
			+ (type.Equals(string.Empty) ? string.Empty : string.Format(@" type=""{0}""", type))
			+ (@readonly ?  @" readonly=""true""" : string.Empty);
	}

</script>
<input name="<%= this.UniqueID %>" id="<%= this.ClientID %>"  value="<%= Server.HtmlEncode(_currentStringValue) %>" <%= TypeParam() %> />