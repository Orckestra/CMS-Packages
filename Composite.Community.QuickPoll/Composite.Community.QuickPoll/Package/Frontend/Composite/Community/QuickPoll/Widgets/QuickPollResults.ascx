<%@ Control Language="C#" Inherits="Composite.Plugins.Forms.WebChannel.UiControlFactories.UserControlBasedUiControl"%>
<%@ Register TagPrefix="aspui" Namespace="Composite.Core.WebClient.UiControlLib" Assembly="Composite" %>
<%@ Import Namespace="Composite.C1Console.Forms" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Composite.Data" %>
<%@ Import Namespace="Composite.Core.Types" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="Composite.Community.QuickPoll" %>

<script runat="server">
	[BindableProperty()]
	[FormsProperty()]
	public Guid QuestionId { get; set; }

	public override void BindStateToControlProperties()
	{
	}

	public override void InitializeViewState()
	{
		if (QuestionId != Guid.Empty)
		{
			var answers = DataFacade.GetData<Answers>(d => d.QuestionRef == QuestionId);
			if (answers.Count() > 0)
			{
				repAnswers.DataSource = answers;
				repAnswers.DataBind();
			}
			else
				lblResultText.Visible = true;
		}
		else
			lblResultText.Visible = true;
	}

</script>

<table style="margin-top:7px;" width="100%">
	<asp:Repeater ID="repAnswers" runat="server">
		<ItemTemplate>
			<tr>
				<td style="width:90%;">
					<%# Server.HtmlEncode((DataBinder.Eval(Container.DataItem, "AnswerText")).ToString())%>
				</td>
				<td>
					:
				</td>
				<td>
					<%# DataBinder.Eval(Container.DataItem, "TotalVotes")%>
				</td>
			</tr>
		</ItemTemplate>
	</asp:Repeater>
	<tr>
		<td style="white-space:nowrap">
			<asp:Literal ID="lblResultText" runat="server" Visible="false" Text="No answers added"></asp:Literal>
		</td>
	</tr>
</table>
