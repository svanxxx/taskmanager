<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EditPlan.aspx.cs" Inherits="EditPlan" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Edit Plan</title>
	<script type="text/javascript" src="Scripts/jquery/jquery-1.11.2.js"></script>
	<script type="text/javascript" src="Scripts/jquery/jquery-ui.min.js"></script>
	<script type="text/javascript" src="Scripts/jquery/jquery.cookie.js"></script>
	<script type="text/javascript" src="Scripts/Common.js"></script>
	<link rel="stylesheet" type="text/css" href="Scripts/jquery/jquery-ui.min.css" />
	<link rel="stylesheet" type="text/css" href="Styles/Common.css" />
	<link rel="stylesheet" type="text/css" href="Styles/MonthSelector.css" />

	<script type="text/javascript" src="Scripts/EditPlan.js"></script>
	<link rel="stylesheet" type="text/css" href="Styles/EditPLan.css" />
</head>
<body>
	<form id="form1" runat="server">
		<div>
			<table id="toolsbar" style="width: 100%;display:none">
				<tr>
					<td id="droptdbin" style="width: 50%">
						<asp:Image ID="ImageBin" runat="server" Width="60px" Height="60px" ImageUrl="~/IMAGES/bin.png" Style="display: block; margin-left: auto; margin-right: auto;" />
					</td>
					<td id="droptdtt" style="width: 50%">
						<asp:Image ID="ImageTT" runat="server" Width="60px" Height="60px" ImageUrl="~/IMAGES/TT.png" Style="display: block; margin-left: auto; margin-right: auto;" />
					</td>
				</tr>
			</table>
			<asp:Table ID="UsersTbl" runat="server" Width="100%" EnableViewState="False">
			</asp:Table>
			<div style="width: 33%; float: left">
				<asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/dailyentry.aspx">Daily Entry</asp:HyperLink>
			</div>
			<div style="width: 33%; float: right">
				<asp:HyperLink ID="UserHL" runat="server" NavigateUrl="~/dailyentry.aspx">Activity</asp:HyperLink>
			</div>
			<div style="width: 33%; float: right">
				<asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/ttrep.aspx">Report</asp:HyperLink>
			</div>
			<br />
			<table style="width: 100%">
				<tr>
					<td style="width: 20%">
						<asp:DropDownList ID="UsersList" runat="server" Width="265px" AutoPostBack="True" OnSelectedIndexChanged="UsersList_SelectedIndexChanged" Font-Names="Arial" Font-Size="10pt">
						</asp:DropDownList>
					</td>
					<td style="width: 20%">
						<span>filter:</span>
						<input id="filter" type="text" />
						<span id="filtersum"></span>
					</td>
					<td style="width: 20%">
						<span>time limit:</span>
						<input id="timelimit" type="text" />
					</td>
					<td style="width: 20%; text-align: right">
						<span>selector:</span>
						<input id="selector" type="text" />
						<span id="selectorsum" style="text-align: right"></span>
					</td>
					<td style="width: 20%" align="right">
						<span>Show by</span>
						<select id="showbyCombo">
							<option value="150">150</option>
							<option value="200">200</option>
							<option value="250">250</option>
							<option value="300">300</option>
						</select>					
					</td>
				</tr>
			</table>
			<table style="float: right">
				<tr>
					<td class="reject">Reject</td>
					<td class="process">Process</td>
					<td class="onbst">On BST</td>
					<td><a id="export" title="Export visible rows to csv format file" href="#"><img src="IMAGES/excel_icon.gif""/></a></td>
					<td><img id="tools" title="Turn on/off tools panel" src="IMAGES/tools.png"/></td>
				</tr>
			</table>
			<input type="button" id="pagelink" class="btn-primary btn-xs" value="Get Link" style="float: right" />
			<input type="button" id="removeplan" class="btn-primary btn-xs" value="Remove from plan!" style="float: right" />
			<input type="button" id="updateplan" class="btn-primary btn-xs" value="Update Plan!" />
			<asp:Table ID="TTasks" runat="server" Font-Names="Arial" Font-Size="10pt" Width="100%" EnableViewState="False">
			</asp:Table>
			<span id="systemmessages">messages:</span>
			<div id="statusbar"></div>
		</div>
	</form>
</body>
</html>
