<%@ Control Language="C#" Inherits="Composite.Plugins.Forms.WebChannel.UiControlFactories.SelectorTemplateUserControlBase" %>
<%@ Import Namespace="Composite.Plugins.Forms.WebChannel.UiControlFactories" %>
<%@ Import Namespace="Composite.Core.Types" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>

<script runat="server">
    protected void Page_Init(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            MultiKeyCheckBoxList.DataSource = this.GetOptions();
            /*	.Select(
                    d => new ListItem()
                        {
                            Text = d.Label,
                            Value = d.Key,
                            Selected = this.SelectedKeys.Contains(d.Key)
                        }
                );		 */
            MultiKeyCheckBoxList.DataBind();
            foreach (ListItem item in MultiKeyCheckBoxList.Items)
            {
                if (this.SelectedKeys.Contains(item.Value))
                {
                    item.Selected = true;
                }
            }

        }
    }




    public override void BindStateToProperties()
    {
	    this.SelectedKeys = MultiKeyCheckBoxList.Items
			.Cast<ListItem>()
			.Where(item => item.Selected)
			.Select(item => item.Value).ToList();
    }

    public override string GetDataFieldClientName()
    {
        return this.ClientID.Replace("_", "$");
    }

    public override void InitializeViewState()
    {
    }

</script>

<asp:CheckBoxList ID="MultiKeyCheckBoxList" runat="server" RepeatLayout="Flow" CssClass="checkbox" DataTextField="Label" DataValueField="Key">
</asp:CheckBoxList>
