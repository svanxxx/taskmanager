<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DailyReport.aspx.cs" Inherits="DailyReport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Daily Report</title>
	<link rel="stylesheet" type="text/css" href="Scripts/Jquery/jquery-ui.css" />
	<link rel="stylesheet" type="text/css" href="Scripts/jquery/layout-default-latest.css" />
	<link rel="stylesheet" type="text/css" href="Styles/jquery.ui.themes/redmond/jquery-ui.css" />
	<link rel="stylesheet" type="text/css" href="Styles/Common.css" />
	<link rel="Stylesheet" type="text/css" href="Styles/DailyReport.css" />

	<script type="text/javascript" src="Scripts/Jquery/jquery-1.11.2.js"></script>
	<script type="text/javascript" src="Scripts/Jquery/jquery-ui.min.js"></script>
	<script type="text/javascript" src="Scripts/Jquery/jquery.ui-contextmenu.js"></script>
	<script type="text/javascript" src="Scripts/jquery/jquery.cookie.js"></script>
	<script type="text/javascript" src="Scripts/jquery/jquery.layout-latest.js"></script>
	<script type="text/javascript" src="Scripts/Common.js"></script>
	<script type="text/javascript" src="Scripts/DailyReport.js"></script>
</head>
<body>
	<form id="form1" runat="server">
		<div>
			<input id="date" type="date" class=".input-sm" name="repdate" />
			<table id="maintbl">
				<thead>
					<tr>
						<td>Name</td>
						<td>Yesterday</td>
						<td>Today</td>
						<td>Plan</td>
					</tr>
				</thead>
				<tbody></tbody>
			</table>
		</div>
	</form>
</body>
</html>
