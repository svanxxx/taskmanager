<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ttrep.aspx.cs" Inherits="ttrep" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Severity Report</title>
	<script type="text/javascript" src="Scripts/jquery/jquery-1.11.2.js"></script>
	<script type="text/javascript" src="Scripts/jquery/jquery-ui.min.js"></script>
	<script type="text/javascript" src="Scripts/jquery/jquery.cookie.js"></script>
	<script type="text/javascript" src="Scripts/Common.js"></script>
	<link rel="stylesheet" type="text/css" href="Scripts/jquery/jquery-ui.min.css" />
	<link rel="stylesheet" type="text/css" href="Styles/Common.css" />
	<link rel="stylesheet" type="text/css" href="Styles/MonthSelector.css" />

	<script type="text/javascript" src="Scripts/ttrep.js"></script>
	<link rel="stylesheet" type="text/css" href="Styles/ttrep.css" />
</head>
<body>
	<form id="form1" runat="server">
		<div>
			<asp:DropDownList ID="TTSeverity" runat="server" Height="20px" Width="235px">
			</asp:DropDownList>
			<label>Date:</label>
			<input id="startdate" aria-haspopup="False" height="20px" /><asp:CheckBox ID="startdatecheck" ToolTip="Use Date Filter" runat="server" />
			<asp:DropDownList ID="TTDisposition" runat="server" Height="20px" Width="235px">
			</asp:DropDownList>
			<asp:DropDownList ID="TTUser" runat="server" Height="20px" Width="235px">
			</asp:DropDownList>
			<input id="stext"/>
			<br />
			<button id="more" type="button">more...</button>
			<table id="bstlegend" style="float: right">
				<tr>
					<td class="reject">Reject</td>
					<td class="process">Process</td>
					<td class="onbst">On BST</td>
					<td class="requested">Requested</td>
					<td class="finished">Finished</td>
					<td class="tested">Tested</td>
					<td class="scheduled">Scheduled</td>
					<td><a id="export" title="Export visible rows to csv format file" href="#"><img src="IMAGES/excel_icon.gif""/></a></td>
				</tr>
			</table>
			<div id="summary">
				<div id="totalhours"></div>
			</div>
			<asp:Table class ="table table-hover table-bordered table-condensed" ID="TTTable" runat="server" GridLines="Both" ViewStateMode="Disabled">
			</asp:Table>
			<div id="statusbarspacer"></div>
			<div id="statusbar"></div>
		</div>
	</form>
</body>
</html>
