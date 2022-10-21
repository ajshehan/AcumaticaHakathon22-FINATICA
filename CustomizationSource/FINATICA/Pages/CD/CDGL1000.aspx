<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="CDGL1000.aspx.cs" Inherits="Page_CDGL1000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="CD.FINATICA.FiscalYearMapMaint"
        PrimaryView="CalendarConversionMap"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="CalendarConversionMap" Width="100%" Height="100px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector3" DataField="CurrentStartPeriodID" ></px:PXSelector></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="CalendarConversionMapDetails">
			    <Columns>
				<px:PXGridColumn DataField="CurrentFinYear" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CurrentFinPeriodID" Width="72" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CurrentStartDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CurrentEndDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn CommitChanges="True" DataField="NewFinYear" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn CommitChanges="True" DataField="NewFinPeriodNbr" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn CommitChanges="True" DataField="NewStartDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn CommitChanges="True" DataField="NewEndDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn Type="CheckBox" CommitChanges="True" DataField="IsAdjustment" Width="60" ></px:PXGridColumn></Columns>
			
				<RowTemplate>
					<px:PXNumberEdit runat="server" ID="CstPXNumberEdit5" DataField="NewFinPeriodNbr" ></px:PXNumberEdit></RowTemplate></px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
		<ActionBar >
		</ActionBar>
	</px:PXGrid>
</asp:Content>