<%@ Page Title="Edit Plan" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="editplan.aspx.cs" Inherits="PlanEditor" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/editplan_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/editplan_js")%>
	<script src="http://mps.resnet.com/cdn/angular/angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="alert alert-danger savebutton btn-group-vertical" ng-cloak ng-show="changed">
			<button type="button" class="btn btn-lg btn-info" ng-click="saveDefects()">Save</button>
			<button type="button" class="btn btn-lg btn-danger" ng-click="discardDefects()">Discard</button>
		</div>
		<ul class="nav nav-pills userslist">
			<li class="{{currentuserid===u.TTUSERID?'active':''}}" ng-click="changeuser(u)" ng-repeat="u in filtered = (users | filter:{ INWORK: true })">
				<a class="person" data-toggle="pill" href="#">
					<img ng-src="{{'getUserImg.ashx?id=' + u.ID}}" alt="Smile" height="20" width="20">{{u.LOGIN}}
				</a>
			</li>
		</ul>
		<div>
			<ul class="nav nav-pills userssublist">
				<li class="small active"><a data-toggle="pill" href="#plan">Plan<span class="badge">{{defects.length}}</span></a></li>
				<li class="small"><a data-toggle="pill" href="#unscheduled">Unscheduled<span class="badge">{{unscheduled.length}}</span></a></li>
			</ul>
			<div class="tab-content panel panel-default">
				<div id="plan" class="tab-pane fade in active">
					<div ng-repeat="d in defects" ng-style="{{d.DISPO | getDispoColorById:this}}" class="task alert {{d.orderchanged?'data-changed':''}}">
						<a href="showtask.aspx?ttid={{d.ID}}" target="_blank">
							<span class="badge">{{d.ID}}</span>
						</a>
						<span class="label label-danger">{{d.ESTIM}}</span>
						<span class="tt-label" data-toggle="tooltip" title="{{d.SUMMARY}}">{{d.SUMMARY | limitTo:135}}</span>
						<button ng-click="unscheduletask(d)" data-toggle="tooltip" title="Remove from the schedule list" type="button" class="btn btn-default btn-xs taskselector"><span class="glyphicon glyphicon glyphicon-arrow-right"></span></button>
						<img height="20" width="20" class="taskselector" ng-src="{{'getUserImg.ashx?id=' + d.SMODTRID}}" title="{{d.SMODIFIER}}" />
						<input class="taskselector" type="radio" name="optradio" ng-keydown="taskMove(d, $event)">
					</div>
				</div>
				<div id="unscheduled" class="tab-pane fade">
					<div ng-repeat="d in unscheduled" ng-style="{{d.DISPO | getDispoColorById:this}}" class="task alert {{d.orderchanged?'data-changed':''}}">
						<a href="showtask.aspx?ttid={{d.ID}}" target="_blank">
							<span class="badge">{{d.ID}}</span>
						</a>
						<span class="label label-danger">{{d.ESTIM}}</span>
						<span class="tt-label" data-toggle="tooltip" title="{{d.SUMMARY}}">{{d.SUMMARY | limitTo:135}}</span>
						<button ng-click="scheduletask(d)" data-toggle="tooltip" title="Add to schedule list" type="button" class="btn btn-default btn-xs taskselector"><span class="glyphicon glyphicon-arrow-left"></span></button>
					</div>
				</div>
			</div>
		</div>
	</div>
</asp:Content>
