<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Template8.aspx.cs" Inherits="HaloBI.Prism.Plugin.Template8" validateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">


        <%--Keep the following --%>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
	<link href="Styles/Template8.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
			<br />
			<h4><asp:Label runat="server">Click the button to update the Prism view.</asp:Label></h4>
			<asp:DropDownList ID="uiMembersList" 
				runat="server" 
				OnSelectedIndexChanged="uiMembersList_SelectedIndexChanged"
				AutoPostBack="true">
			</asp:DropDownList>
			<asp:Button ID="uiUpdatePrism" 
				runat="server" 
				OnClick="uiUpdatePrism_Click"
				Text="Update Prism" 
			/>
        </div>
        <div>

		<h1><asp:Label ID="uiHeader" runat="server">Header Text</asp:Label></h1>
		<h3>Selected Item in Prism: 
			<asp:Label ID="uiSelectedMembers" 
				runat="server"
				cssClass="ui-plugin-template-prismSelection">
			</asp:Label>
		</h3>
        <asp:Label ID="weatherImage" 
			runat="server"
			cssClass="ui-plugin-template-prismSelection">
		</asp:Label>
        <asp:Image runat="server" ID="WeatherIcon" alt="Sample Image" width="300px" />
		
		

         <%-- Until Here --%>

		
<%--		<div>
			<h3><asp:Label runat="server">Shared Context Object</asp:Label></h3>
			<asp:TextBox 
				ID="uiContext" 
				TextMode="MultiLine"
				Height = "400px"
				Width = "100%"
				runat="server"
				Visible="false"
				CssClass="ui-plugin-template-context">
			</asp:TextBox>
		</div>--%>

    </div>
    </form>
</body>
</html>
