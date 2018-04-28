<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ViewObjective.aspx.cs" Inherits="ViewObjective" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>View Objective</title>
	<script type="text/javascript" src="scripts/jquery/jquery-1.11.2.js"></script>
	<script type="text/javascript" src="scripts/jquery/jquery-ui.min.js"></script>
	<script type="text/javascript" src="scripts/jquery/jquery.cookie.js"></script>
	<script type="text/javascript" src="scripts/Common.js"></script>
	<link rel="stylesheet" type="text/css" href="scripts/jquery/jquery-ui.min.css" />
	<link rel="stylesheet" type="text/css" href="Styles/Common.css" />

	<script type="text/javascript" src="scripts/ObjectiviesBase.js"></script>
	<script type="text/javascript" src="scripts/ViewObjective.js"></script>
	<link rel="stylesheet" type="text/css" href="Styles/Objectivies.css" />
</head>
<body>
	<form id="form1" runat="server">
		<div>
			<div class="container">
				<a href="#" id="trigger" class="trigger">Select</a>
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
</body>
</html>
