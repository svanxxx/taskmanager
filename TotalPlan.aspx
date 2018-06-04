<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TotalPlan.aspx.cs" Inherits="TotalPlan" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Total Plan</title>
	<link rel="stylesheet" type="text/css" href="Scripts/Jquery/jquery-ui.css" />
	<link rel="stylesheet" type="text/css" href="Scripts/jquery/layout-default-latest.css" />
	<link rel="stylesheet" type="text/css" href="Styles/jquery.ui.themes/redmond/jquery-ui.css" />
	<link rel="stylesheet" type="text/css" href="Styles/Common.css" />
	<link rel="Stylesheet" type="text/css" href="Styles/TotalPlan.css" />

	<script type="text/javascript" src="Scripts/Jquery/jquery-1.11.2.js"></script>
	<script type="text/javascript" src="Scripts/Jquery/jquery-ui.min.js"></script>
	<script type="text/javascript" src="Scripts/Jquery/jquery.ui-contextmenu.js"></script>
	<script src="http://mps.resnet.com/cdn/jquery/jquery.cookie.js"></script>
	<script type="text/javascript" src="Scripts/jquery/jquery.layout-latest.js"></script>
	<script type="text/javascript" src="Scripts/Common.js"></script>
	<script src="http://mps.resnet.com/cdn/mpshelper.js"></script>
	<script type="text/javascript" src="Scripts/TotalPlan.js"></script>
</head>
<body>
	<form id="form1" runat="server">
		<div>
			<nav class="navbar navbar-default">
				<div class="container-fluid">
					<div class="navbar-header">
						<a class="navbar-brand" href="totalplan.aspx">Live Calendar Plan</a>
					</div>
					<div>
						<ul class="nav navbar-nav">
							<li><a href="vacations.aspx">Vacations</a></li>
							<li><a href="http://mps.resnet.com/bst/web/index.aspx">BST</a></li>
							<li><a href="editplan.aspx">Personal Plan</a></li>
						</ul>
						<div class="pull-right">
							<label for="tttoggle">Show Plan</label>
							<input checked="checked" type="checkbox" class="form-control input-sm" id="tttoggle" />
						</div>
						<div class="pull-right">
							<label for="ttfilter">Highlight tasks by:</label>
							<input type="text" class="form-control input-sm" id="ttfilter" />
						</div>
					</div>
				</div>
			</nav>

			<div id="tblcontainer">
				<table id="maintbl">
					<thead />
					<tbody />
				</table>
				<div class="clselector col-md-2" style="background-color: purple">
					<select class="sevselector form-control" id="sev1"></select>
				</div>
				<div class="clselector col-md-2" style="background-color: green">
					<select class="sevselector form-control" id="sev2"></select>
				</div>
				<div class="clselector col-md-2" style="background-color: blue">
					<select class="sevselector form-control" id="sev3"></select>
				</div>
				<div class="clselector col-md-2" style="background-color: aqua">
					<select class="sevselector form-control" id="sev4"></select>
				</div>
			</div>
		</div>
	</form>
</body>
</html>
