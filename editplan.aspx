<%@ Page Title="Edit Plan" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="editplan.aspx.cs" Inherits="PlanEditor" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/editplan_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/editplan_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
	<script src="scripts/jquery.signalR-2.3.0.min.js"></script>
	<script src="signalr/hubs"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="alert alert-danger savebutton btn-group-vertical" ng-cloak ng-show="changed">
			<button type="button" class="btn btn-lg btn-info" ng-click="saveDefects()">Save</button>
			<button type="button" class="btn btn-lg btn-danger" ng-click="discardDefects()">Discard</button>
		</div>
		<div class="row">
			<div class="col-lg-2 col-md-2 hidden-sm hidden-xs">
				<div class="alert alert-success">
					<h5>To schedule the task: click on radio button for the task! Then holding <strong>Ctrl</strong> key press <strong>move up</strong> or <strong>move down</strong> to move the task in list correspondingly.</h5>
					<h5>Click <span class="glyphicon glyphicon-arrow-up"></span>to move task the the top which will result in the task being done first of all</h5>
					<h5>Click <span class="glyphicon glyphicon-arrow-right"></span>to move task out of schedlue</h5>
					<h5>In unscheduled section click <span class="glyphicon glyphicon-arrow-left"></span>to move task into the schedlue being on the top of list</h5>
				</div>
				<div class="alert alert-success">
					<button onclick="copyurl()" type="button" class="btn btn-link"><span class="glyphicon glyphicon-copy"></span>Copy page link</button>
				</div>
			</div>
			<div class="col-lg-9 col-md-10 col-sm-12 col-xs-12">
				<ul class="nav nav-tabs nav-justified userslist">
					<li class="{{currentuserid===u.ID?'active':''}}" ng-click="changeuser(u, true)" ng-repeat="u in filtered = (users | filter:{ INWORK: true })">
						<a class="person" data-toggle="pill" href="#">
							<img class="img-circle" ng-src="{{'getUserImg.ashx?id=' + u.ID}}" alt="Smile" height="30" width="30" />
							<h5><span class="label label-info">{{u.LOGIN}}</span></h5>
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
							<table style="width: 100%">
								<tr class="task alert {{d.orderchanged?'data-changed':''}}" ng-repeat="d in defects" ng-style="{{d.DISPO | getDispoColorById:this}}">
									<td><a href="showtask.aspx?ttid={{d.ID}}" target="_blank"><span class="badge">{{d.ID}}</span></a></td>
									<td><span class="label label-danger">{{d.ESTIM}}</span></td>
									<td><span data-toggle="tooltip" title="{{d.SUMMARY}}">{{d.SUMMARY | limitTo:135}}</span></td>
									<td><input class="taskselector" type="radio" name="optradio" ng-keydown="taskMove(d, $event)"></td>
									<td><img height="20" width="20" class="img-circle taskselector" ng-src="{{'getUserImg.ashx?id=' + d.SMODTRID}}" title="{{d.SMODIFIER}}" /></td>
									<td><button ng-click="tasktotop(d)" data-toggle="tooltip" title="Move to top" type="button" class="btn btn-default btn-xs taskselector"><span class="glyphicon glyphicon-arrow-up"></span></button></td>
									<td><button ng-click="unscheduletask(d)" data-toggle="tooltip" title="Remove from the schedule list" type="button" class="btn btn-default btn-xs taskselector"><span class="glyphicon glyphicon-arrow-right"></span></button></td>
								</tr>
							</table>
						</div>
						<div id="unscheduled" class="tab-pane fade">
							<table style="width: 100%">
								<tr class="task alert {{d.orderchanged?'data-changed':''}}" ng-repeat="d in unscheduled" ng-style="{{d.DISPO | getDispoColorById:this}}">
									<td><a href="showtask.aspx?ttid={{d.ID}}" target="_blank"><span class="badge">{{d.ID}}</span></a></td>
									<td><span class="label label-danger">{{d.ESTIM}}</span></td>
									<td><span data-toggle="tooltip" title="{{d.SUMMARY}}">{{d.SUMMARY | limitTo:135}}</span></td>
									<td><button ng-click="scheduletask(d)" data-toggle="tooltip" title="Add to schedule list" type="button" class="btn btn-default btn-xs taskselector"><span class="glyphicon glyphicon-arrow-left"></span></button></td>
								</tr>
							</table>
						</div>
					</div>
				</div>
			</div>
			<div class="col-lg-1 hidden-md hidden-sm hidden-xs">
			</div>
		</div>
	</div>
</asp:Content>
