<%@ Control Language="C#" Inherits="Composite.Plugins.Forms.WebChannel.UiControlFactories.SelectorTemplateUserControlBase" %>
<%@ Import Namespace="Composite.Plugins.Forms.WebChannel.UiControlFactories" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="System.Collections.Generic" %>

<script runat="server">
    string _selectedKey = null;

    protected void Page_Init(object sender, EventArgs e)
    {
        _selectedKey = HttpContext.Current.Request.Form[this.ClientName];
    }

    protected override void BindStateToProperties()
    {
        this.SelectedKeys = new List<string> { _selectedKey };
    }

    protected override void InitializeViewState()
    {
        List<KeyLabelPair> options = this.GetOptions();
        optionsRepeater.DataSource = options;
        optionsRepeater.DataBind();

        List<string> selectedKeys = new List<string>(this.SelectedKeys);

        if (selectedKeys.Count > 0)
        {
            if (selectedKeys.Count() > 1) throw new InvalidOperationException("Multiple elements selected. This was unexpected");
            if (options.Select(o => o.Key).Contains(selectedKeys[0]))
                _selectedKey = selectedKeys[0];
        }

        if (string.IsNullOrEmpty(_selectedKey) && options != null && options.Count > 1)
        {
            _selectedKey = this.GetOptions()[0].Key;
        }
    }

    public override string GetDataFieldClientName()
    {
        return this.ClientName;
    }

    string CustomUiSelectorTagParams(KeyLabelPair keyLabelPair)
    {
        if (this.SelectedKeys.Any() && keyLabelPair.Key == this.SelectedKeys.First())
        {
            return "selected=\"true\"";
        }

        return "";
    }

    private string ClientName
    {
        get
        {
            return this.UniqueID;
        }
    }
</script>
<select name="<%= this.ClientName %>" id="<%= this.ClientID %>">
    <asp:Repeater runat="server" ID="optionsRepeater">
        <ItemTemplate>
            <option
                value="<%# Server.HtmlEncode(((KeyLabelPair)Container.DataItem).Key) %>"
                <%# CustomUiSelectorTagParams( (KeyLabelPair)Container.DataItem ) %>>
                <%# Server.HtmlEncode(((KeyLabelPair)Container.DataItem).Label) %>
            </option>
        </ItemTemplate>
    </asp:Repeater>
</select>
