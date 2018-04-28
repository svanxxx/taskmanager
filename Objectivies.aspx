<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Objectivies.aspx.cs" Inherits="Objectivies" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Objectivies</title>
	<script type="text/javascript" src="Scripts/jquery/jquery-1.11.2.js"></script>
	<script type="text/javascript" src="Scripts/jquery/jquery-ui.min.js"></script>
	<script type="text/javascript" src="Scripts/jquery/jquery.cookie.js"></script>
	<script type="text/javascript" src="Scripts/Common.js"></script>
	<link rel="stylesheet" type="text/css" href="Scripts/jquery/jquery-ui.min.css" />
	<link rel="stylesheet" type="text/css" href="Styles/Common.css" />

	<script type="text/javascript" src="Scripts/ObjectiviesBase.js"></script>
	<script type="text/javascript" src="Scripts/Objectivies.js"></script>
	<link rel="stylesheet" type="text/css" href="Styles/Objectivies.css">
</head>
<body>
	<form id="form1" runat="server">
		<div>
			<div class="container">
				<a href="#" id="trigger" class="trigger">Add</a>
				<div id="drop" class="drop">
				</div>
			</div>
			<table id="bstlegend" style="float: right">
				<tr>
					<td class="reject">Reject</td>
					<td class="process">Process</td>
					<td class="onbst">On BST</td>
					<td class="requested">Requested</td>
					<td class="finished">Finished</td>
					<td class="tested">Tested</td>
					<td class="scheduled">Scheduled</td>
				</tr>
			</table>
		</div>
	</form>
	<div id="statusbar"></div>
</body>
</html>
