<%@ Control Language="C#" AutoEventWireup="true" CodeFile="QuickPollPieChart.ascx.cs"
	Inherits="Frontend_Composite_Community_QuickPoll_QuestionPieChart" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
	Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%@ Import Namespace="Composite.Data" %>
<%@ Import Namespace="Composite.Community.QuickPoll" %>
<div>
	<asp:Chart ID="QuickPollResultChart" runat="server" Width="300" Height="200">
		<Legends>
			<asp:Legend BackColor="Transparent" Alignment="Center" Docking="Right" Font="Trebuchet MS, 8.25pt, style=Bold"
				IsTextAutoFit="False" Name="Default" LegendStyle="Column">
			</asp:Legend>
		</Legends>
		<BorderSkin SkinStyle="Raised"></BorderSkin>
		<Series>
			<asp:Series Name="Default" ChartType="Pie" Label="#PERCENT{P1}" BorderColor="180, 26, 59, 105"
				Color="220, 65, 140, 240">
			</asp:Series>
		</Series>
		<ChartAreas>
			<asp:ChartArea Name="ChartArea1" BorderColor="64, 64, 64, 64" BackSecondaryColor="Transparent"
				BackColor="Transparent" ShadowColor="Transparent" BorderWidth="0">
				<Area3DStyle Rotation="0" />
				<AxisY LineColor="64, 64, 64, 64">
					<LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
					<MajorGrid LineColor="64, 64, 64, 64" />
				</AxisY>
				<AxisX LineColor="64, 64, 64, 64">
					<LabelStyle Font="Trebuchet MS, 8.25pt, style=Bold" />
					<MajorGrid LineColor="64, 64, 64, 64" />
				</AxisX>
			</asp:ChartArea>
		</ChartAreas>
	</asp:Chart>
</div>
