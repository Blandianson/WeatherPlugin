<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Weather.aspx.cs" Inherits="HaloBI.Prism.Plugin.Weather" validateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
	<link href="Styles/Weather.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div id="weatherUI">

            <h1><asp:Label runat="server" ID="city">City, Region, Country</asp:Label></h1>
            <h2 id="currWeather1"><asp:Label runat="server" ID="currWeather">Current Weather</asp:Label></h2>

            <div id="parentPanel">
                <div id="l_panel">
                    <asp:Image runat="server" ID="currIcon" alt="Current Weather" width="170px"/>
                </div>

                <div id="temp">
                    <h1><asp:Label runat="server" ID="currTemp">16 C</asp:Label></h1>
                    <label class="switch">
                      <asp:CheckBox id="tempToggle" runat="server" AutoPostBack="True" oncheckedchanged="uiUpdatePrism_Click"/>
                      <span class="slider round"></span>
                    </label>
                </div>

                <div id="r_panel">
                    <h4><asp:Label runat="server" ID="high">High</asp:Label></h4>
                    <hr />
                    <h4><asp:Label runat="server" ID="low">Low</asp:Label></h4>
                    <hr />
                    <h4><asp:Label runat="server" ID="titleTomor">Tomorrow</asp:Label></h4>
                    <div id="tomorrow">
                        <asp:Image runat="server" ID="nextIcon" alt="Tommorows Weather" width="50px" height="50px"/>
                        <h5><asp:Label runat="server" ID="nextHigh">Tomorrows High</asp:Label></h5>
                        <h5><asp:Label runat="server" ID="delimitier"> | </asp:Label></h5>
                        <h5><asp:Label runat="server" ID="nextLow">Tomorrows Low</asp:Label></h5>
                    </div>
                </div>

            </div>
        </div>

        <div id="hideAdmin">
			<br />
			<asp:DropDownList ID="uiMembersList" 
				runat="server" 
				OnSelectedIndexChanged="uiMembersList_SelectedIndexChanged"
				AutoPostBack="true">
			</asp:DropDownList>
			<asp:Label ID="uiSelectedMembers" 
				runat="server"
				cssClass="ui-plugin-template-prismSelection">
			</asp:Label>

        </div>
    </form>
</body>
</html>
