<%@ Page Title="Edit Plan" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="editplan.aspx.cs" Inherits="PlanEditor" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<script src="scripts/references.js"></script>
	<link href="css/editplan.css" rel="stylesheet" />
	<script src="scripts/editplan.js"></script>
	<script src="http://mps.resnet.com/cdn/angular/angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<ul class="nav nav-pills">
			<li class="{{currentuser===u.TTUSERID?'active':''}}" ng-click="changeuser(u)" ng-repeat="u in filtered = (users | filter:{ INWORK: true })">
				<a class="person" data-toggle="pill" href="#">
					<img ng-src="{{u.ID | getUserImgById:this}}" alt="Smile" height="20" width="20">{{u.LOGIN}}
				</a>
			</li>
		</ul>
		<div>
			<ul class="nav nav-pills">
				<li class="small active"><a data-toggle="pill" href="#plan">Plan<span class="badge">{{defects.length}}</span></a></li>
				<li class="small"><a data-toggle="pill" href="#unscheduled">Unscheduled<span class="badge">{{unscheduled.length}}</span></a></li>
			</ul>
			<div class="tab-content panel panel-default">
				<div id="plan" class="tab-pane fade in active">
					<div ng-repeat="d in defects" ng-style="{{d.DISPO | getDispoColorById:this}}" class="task alert">
						<a href="showtask.aspx?ttid={{d.ID}}" target="_blank"><span class="badge">{{d.ID}}</span></a>{{d.SUMMARY}}<input class="taskselector" type="radio" name="optradio" ng-keydown="taskMove(d, $event)">
					</div>
				</div>
				<div id="unscheduled" class="tab-pane fade">
					<div ng-repeat="d in unscheduled" ng-style="{{d.DISPO | getDispoColorById:this}}" class="task alert">
						<a href="showtask.aspx?ttid={{d.ID}}" target="_blank"><span class="badge">{{d.ID}}</span></a>{{d.SUMMARY}}
					</div>
				</div>
			</div>
		</div>
	</div>
</asp:Content>
