<%@ Page Language="C#" AutoEventWireup="true" CodeFile="dailyentry.aspx.cs" Inherits="dailyentry"
	ValidateRequest="false" %>

<%@ Register Assembly="System.Web.DynamicData, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
	Namespace="System.Web.DynamicData" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Daily Entry</title>

	<link rel="stylesheet" type="text/css" href="Scripts/Jquery/jquery-ui.css" />
	<link rel="stylesheet" type="text/css" href="Scripts/jquery/layout-default-latest.css" />
	<link rel="stylesheet" type="text/css" href="Styles/jquery.ui.themes/redmond/jquery-ui.css" />
	<link rel="stylesheet" type="text/css" href="Styles/Common.css" />
	<link rel="stylesheet" type="text/css" href="TR.css" />
	<link rel="Stylesheet" type="text/css" href="Styles/DailyEntry.css" />

	<script type="text/javascript" src="Scripts/Jquery/jquery-1.11.2.js"></script>
	<script type="text/javascript" src="Scripts/Jquery/jquery-ui.min.js"></script>
	<script type="text/javascript" src="Scripts/Jquery/jquery.ui-contextmenu.js"></script>
	<script type="text/javascript" src="Scripts/jquery/jquery.cookie.js"></script>
	<script type="text/javascript" src="Scripts/jquery/jquery.layout-latest.js"></script>
	<script type="text/javascript" src="Scripts/Common.js"></script>
	<script type="text/javascript" src="Scripts/DailyEntry.js"></script>
</head>
<body onload="$('#dailyEntryForm').show();ShowWaitDlg(false);init_body();">
	<form id="dailyEntryForm" runat="server" defaultfocus="TodayText">
		<script type="text/javascript">
			ShowWaitDlg();
			$("#dailyEntryForm").hide();
		</script>
		<div class="ui-layout-north layout-container">
			<nav class="navbar navbar-default">
			  <div class="container-fluid">
				 <div class="navbar-header">
					<a class="navbar-brand" href="dailyentry.aspx">MPS Daily Entry</a>
				 </div>
				 <div>
					<ul class="nav navbar-nav">
					  <li><a href="DocSearch.aspx?text={8F349AA0-AEBF-45a9-911D-93FD253024A0}">Order</a></li>
					  <li><a href="DocSearch.aspx?text={1457718A-4A96-40a6-9BA1-0F7B88AD72FB}">Tasks</a></li> 
					  <li><a href="DocSearch.aspx?text=DBUPDATE_NEW_TEMPLATE.XLS">DB Templates</a></li> 
					  <li><a href="DocSearch.aspx?text={9C0807EA-F77C-4e85-9702-B8A698015AB3}">Emails</a></li> 
					  <li><a href="vacations.aspx">Vacations</a></li> 
					  <li><a href="http://mps.resnet.com:2222/requests.aspx">BST</a></li> 
					  <li><a href="MonthlySum.aspx">Month</a></li> 
					  <li><a runat="server" id="repa" href="">My Reports</a></li>
					  <li><a href="TotalPlan.aspx">Calendar</a></li> 
					</ul>
				 </div>
				 <a href="login.aspx?ReturnUrl=dailyentry.aspx" type="button" class="btn btn-default navbar-btn pull-right">
					<span class="glyphicon glyphicon-user"></span>Logout
				 </a>
			  </div>
			</nav>
			<div id="actioncontainer1" class="container">
				<div id="mainactions" class="row">
					<button id="todaybtn" type="button" class="btn btn-primary btn-xs">Today!</button>
					<button id="discbtn" type="button" class="btn btn-primary btn-xs">Discard</button>
					<button id="newrbtn" type="button" class="btn btn-primary btn-xs">New Record</button>
					<button id="delebtn" type="button" class="btn btn-primary btn-xs">Delete Record</button>
				</div>
			</div>
			<div id="actioncontainer2" class="container">
				<div class="row">
					<div class="col-sm-2">
						<div id="datepicker"></div>
					</div>
					<div class="col-sm-2">
							<table align="center">
								<tr>
									<td>
										<button type="button" class="btn btn-primary" style="width: 100%; margin: 1px;" onclick="window.location='DailySearch.aspx'">Search TR</button>
									</td>
								</tr>
								<tr>
									<td>
										<button type="button" class="btn btn-primary" style="width: 100%; margin: 1px;" onclick="window.location='DocSearch.aspx'">Search DOCs</button>
									</td>
								</tr>
								<tr>
									<td>
										<button type="button" class="btn btn-primary" style="width: 100%; margin: 1px;" onclick="window.location='EditUsers.aspx'">Users</button>
									</td>
								</tr>
								<tr>
									<td>
										<button type="button" class="btn btn-primary" style="width: 100%; margin: 1px;" onclick="window.location='DailyReport.aspx'">Report</button>
										<br />
									</td>
								</tr>
								<tr>
									<td>
										<asp:Label ID="BirthdayLabel" runat="server" Text="Birthday" CssClass="verysmalltext"
											ForeColor="Red"></asp:Label>
										<br />
									</td>
								</tr>
							</table>
					</div>
					<div class="col-sm-2">
							<asp:ImageButton ID="PeronalImage" runat="server" Height="100px" OnClientClick="window.open(&quot;dailyentry.aspx&quot;,&quot;_self&quot;);" Width="100px" />
					</div>
					<div class="col-sm-2">
						<br/>
						<p id="progressmess">Time Left:...</p>
						<div class="progress">
							<div class="progress-bar progress-bar-striped active" role="progressbar" aria-valuenow="40" aria-valuemin="0" aria-valuemax="100" style="width: 0%">
								0%
							</div>
						</div>
						<p id="progressmessdown">Time Left:...</p>
					</div>
					<div class="col-sm-2">
						<div class="row">
							<h6 class="col-sm-5">Time in:</h6>
							<asp:TextBox ID="mTimeIN" runat="server" AutoPostBack="False" AutoCompleteType="Disabled" TabIndex="1" CssClass="input-xs col-sm-3"></asp:TextBox>
							<button id="InBtn" type="button" class="btn btn-primary btn-xs col-sm-3">&lt;&lt; Now</button>
						</div>
						<div class="row">
							<h6 class="col-sm-5">Time out:</h6>
							<asp:TextBox ID="mTimeOut" runat="server" AutoPostBack="False" AutoCompleteType="Disabled" TabIndex="2" CssClass="input-xs col-sm-3"></asp:TextBox>
							<button id="OutBtn" type="button" class="btn btn-primary btn-xs col-sm-3">&lt;&lt; Now</button>
						</div>
						<div class="row">
							<h6 class="col-sm-5">Break:</h6>
							<asp:TextBox ID="mBreakTime" runat="server" AutoPostBack="False" AutoCompleteType="Disabled" TabIndex="3" CssClass="input-xs col-sm-3"></asp:TextBox>
						</div>
						<div class="row">
							<div class="checkbox">
								<label>
									<input id="copycheck" type="checkbox" value="" />Copy Last Day</label>
							</div>
							<div class="checkbox">
								<label>
									<input id="RunTime" type="checkbox" value="" />RunTime Timer</label>
							</div>
							<div class="checkbox">
								<label>
									<input id="AutoTT" type="checkbox" value="" />Auto TT update</label>
							</div>
						</div>
					</div>
					<span class="col-sm-2">
						<asp:ImageButton ID="Save" runat="server" Height="100px" ImageUrl="IMAGES/BigSave.JPG"
							OnClick="Save_Click" ToolTip="Save the report" Visible="True" Width="100px" TabIndex="6" />
					</span>
				</div>
			</div>
		</div>
		<div class="ui-layout-center layout-container">
			<div style="display: inline-block; width: 100%; height: 100%;">
				<asp:TextBox ID="TodayText" runat="server" Height="100%" TextMode="MultiLine" TabIndex="4" Style="resize: none; width: 100%; float: left;"></asp:TextBox>
			</div>
		</div>
		<div class="ui-layout-south layout-container" style="padding-top: 0px;">
			<div id="PlanContainer" style="width: 100%; height: 100%; padding-top: 0px;" class="layout-container">
				<div class="ui-layout-north layout-container">
					<div style="width: 100%; height: 100%;">
						<input id="PlanAssignedButton" class="verysmalltext" type="button" onclick="{ TogglePlan(); }" value="Assigned Plan" />
						<input id="PlanMyButton" class="verysmalltext" type="button" onclick="{ TogglePlan(); }" value="My Plan" name="My Plan" />
						<asp:HyperLink ID="HyperLink7" runat="server" CssClass="smalltext" NavigateUrl="~/EditPlan.aspx">Edit Plan</asp:HyperLink>
					</div>
				</div>
				<div class="ui-layout-center layout-container">
					<div id="plancontainer" style="border: 1px solid black; width: 100%; height: 100%;">
						<asp:TextBox ID="TomorrowText" runat="server" Height="100%" TextMode="MultiLine"
							Width="100%" TabIndex="5"></asp:TextBox>
						<div id="TomorrowPlan" style="width: 100%; height: 100%;">Loading...</div>
					</div>
				</div>
			</div>
		</div>
	</form>
</body>
</html>
