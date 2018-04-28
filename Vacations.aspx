<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Vacations.aspx.cs" Inherits="Vacations" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Vacations</title>
	<script type="text/javascript" src="Scripts/jquery/jquery-1.11.2.js"></script>
	<script type="text/javascript" src="Scripts/jquery/jquery-ui.min.js"></script>
	<script type="text/javascript" src="Scripts/jquery/jquery.cookie.js"></script>
	<script type="text/javascript" src="Scripts/Common.js"></script>
	<link rel="stylesheet" type="text/css" href="Scripts/jquery/jquery-ui.min.css" />
	<link rel="stylesheet" type="text/css" href="Styles/Common.css" />
	<script type="text/javascript" src="Scripts/Vacations.js"></script>
	<link rel="stylesheet" type="text/css" href="Styles/Vacations.css" />
</head>
<body>
	<form id="form1" runat="server">
		<asp:HiddenField ID="LastUpdatedTR" runat="server"/>
		<div>
			<div id="sidebar" style="position: absolute; width: 170px; height: 100%;">
				<asp:Table ID="SNames" runat="server" BorderStyle="None" BorderWidth="1px" CellPadding="0" CellSpacing="1" GridLines="Both" BorderColor="White" EnableViewState="False">
				</asp:Table>
			</div>
			<div id="centered" style="display: block; background-color: blue; margin-left: 170px; overflow-x: scroll; top: 0px; right: 0px; bottom: 0px; left: 0px; overflow: scroll;">
				<asp:Table ID="STable" runat="server" BorderStyle="None" BorderWidth="1px" CellPadding="0" CellSpacing="1" GridLines="Both" EnableViewState="False">
				</asp:Table>
			</div>
			<table id="SLegend">
				<tr>
					<td class="unpaid">Unpaid</td>
					<td class="sick">Sick</td>
					<td class="weekend">Weekend</td>
					<td class="empty">Empty</td>
					<td class="problem">Problem</td>
					<td class="work">Work</td>
					<td class="vacation">Vacation</td>
				</tr>
			</table>
			<select id="syear">
				<option>2013</option>
				<option>2014</option>
				<option>2015</option>
				<option>2016</option>
				<option>2017</option>
				<option>2018</option>
				<option>2019</option>
			</select>
		</div>
		<div id="rowselector" style='visibility: hidden; position: absolute; background: rgba(200, 54, 54, 0.2); top: 0px; right: 0px; left: 0px;'>&nbsp</div> 
	</form>
</body>
</html>
