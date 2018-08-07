<%@ Page Title="Statistics" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="statistics.aspx.cs" Inherits="Statistics" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/statistics_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/statistics_js")%>
	<script src="http://mps.resnet.com/cdn/angular/angular.min.js"></script>
	<script src="http://mps.resnet.com/cdn/chart/Chart.bundle.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="row">
			<div class="col-sm-4">
				<h4>Report for: </h4>
			</div>
			<div class="col-sm-4">
				<input class="form-control" ng-change="loadData()" ng-model="daterep" type="month" />
			</div>
			<div class="col-sm-4">
				<input class="form-control" ng-change="loadData()" ng-model="daterepend" type="month" />
			</div>
		</div>
		<ul class="nav nav-pills">
			<li class="small active"><a data-toggle="pill" href="#vacations">Vacations</a></li>
			<li><a class="small" data-toggle="pill" href="#hours">Hours</a></li>
			<li><a class="small" data-toggle="pill" href="#tasks">Tasks</a></li>
		</ul>
		<div class="tab-content">
			<div id="vacations" class="tab-pane fade in active">
				<div class="row">
					<div class="panel panel-success">
						<div class="panel-heading">Sick days and vacations</div>
						<div class="panel-body">
							<canvas id="sick" width="800" height="400"></canvas>
						</div>
					</div>
				</div>
			</div>
			<div id="hours" class="tab-pane fade">
				<div class="row">
					<div class="col-sm-6">
						<div class="panel panel-success">
							<div class="panel-heading">Total Hours</div>
							<div class="panel-body">
								<canvas id="hourspermonth" width="100" height="100"></canvas>
							</div>
						</div>
					</div>
					<div class="col-sm-6">
						<div class="panel panel-success">
							<div class="panel-heading">Total hours by person</div>
							<div class="panel-body">
								<canvas id="hourspermonthP" width="400" height="400"></canvas>
							</div>
						</div>
					</div>
				</div>
			</div>
			<div id="tasks" class="tab-pane fade">
				<div class="row">
					<div class="col-sm-6">
						<div class="panel panel-success">
							<div class="panel-heading">Count</div>
							<div class="panel-body">
								<canvas id="countpermonth" width="400" height="400"></canvas>
							</div>
						</div>
					</div>
					<div class="col-sm-6">
						<div class="panel panel-success">
							<div class="panel-heading">Count by person</div>
							<div class="panel-body">
								<canvas id="countpermonthP" width="400" height="400"></canvas>
							</div>
						</div>
					</div>
				</div>
			</div>
		</div>
	</div>
</asp:Content>
