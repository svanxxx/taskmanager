<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Machines.aspx.cs" Inherits="Machines" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>MPS Machines</title>
	<meta charset="utf-8">
	<meta name="viewport" content="width=device-width, initial-scale=1">
	<script type="text/javascript" src="Scripts/jquery/jquery-1.11.2.js"></script>
	<script type="text/javascript" src="Scripts/jquery/jquery-ui.min.js"></script>
	<script src="http://mps.resnet.com/cdn/jquery/jquery.cookie.js"></script>
	<script type="text/javascript" src="Scripts/Common.js"></script>
	<script src="http://mps.resnet.com/cdn/mpshelper.js"></script>
	<script type="text/javascript" src="Scripts/Machines.js"></script>
	<link rel="stylesheet" type="text/css" href="Scripts/jquery/jquery-ui.min.css" />
	<link rel="stylesheet" type="text/css" href="Styles/Common.css" />
</head>
<body>
	<form id="form1" runat="server">
		<div class="container">
			<div class="row">
				<h1>Manage machines</h1>
				<p id="hint" runat="server">Select machine:</p>
			</div>
				<div class="col-sm-12" id="maindiv" runat="server">
				</div>
		</div>
	</form>
</body>
</html>
