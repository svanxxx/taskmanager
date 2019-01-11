<%@ Page Title="Builds" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="builds.aspx.cs" Inherits="Builds" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/builds_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/buildshelper_js")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/builds_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
	<script src="scripts/jquery.signalR-2.3.0.min.js"></script>
	<script src="signalr/hubs"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<input type="hidden" id="buildtime" value="<%=Settings.CurrentSettings.BUILDTIME.ToString()%>" />
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="panel panel-info" ng-cloak>
			<div class="panel-heading" style="text-align: center">Builds History</div>
			<div class="panel-body">
				<div class="list-group">
					<div class="list-group-item" ng-repeat="b in builds" target="_blank" style="background-color: {{b.COLOR}}">
						<div class="row">
							<div class="col-sm-1">
								<span class="label label-info ng-binding"><span class="glyphicon glyphicon-time"></span>{{b.DATE}}</span>
							</div>
							<div class="col-sm-2">
								<a href="showtask.aspx?ttid={{b.TTID}}" target="_blank">
									<span class="badge">{{b.TTID}}</span>
								</a>
								<span>{{b.STATUS}}</span>
							</div>
							<div class="col-sm-2">
								<img class="rep-img rounded-circle" ng-src="{{'getUserImg.ashx?ttid=' + b.TTUSERID}}" alt="Smile" height="20" width="20">
								<span class="glyphicon glyphicon-comment"></span><span>{{b.NOTES}}</span>
							</div>
							<div class="col-sm-2">
								<span class="glyphicon glyphicon-blackboard"></span><span>{{b.MACHINE}}&nbsp({{b.DURATION}} min)</span>
								<div class="progress" ng-show="b.STATUS.includes('wait')==true">
									<div class="progress-bar progress-bar-striped active progress-bar-warning" role="progressbar" style="width: 100%">
										{{b.STATUS}}...
									</div>
								</div>
								<div class="progress" ng-show="b.STATUS.includes('Building')==true">
									<div class="progress-bar progress-bar-striped active" role="progressbar" style="width: {{b.PERCENT}}%">
										{{b.STATUSTXT?b.STATUSTXT:b.STATUS}}
									</div>
								</div>
							</div>
							<div class="col-sm-1">
								<span class="label label-info ng-binding"><span class="glyphicon glyphicon-time"></span>{{b.DATEUP}}</span>
							</div>
							<div class="col-sm-3">
								<a href="getBuildLog.ashx?id={{b.ID}}">{{b.STATUSTXT}}
								</a>
							</div>
						</div>
					</div>
				</div>
			</div>
		</div>
		<div class="row">
			<div class="col-sm-11">
				<ul class="pager">
					<li><a style="cursor: pointer" ng-click="decPage()">Previous</a></li>
					<span>{{page}}</span>
					<li><a style="cursor: pointer" ng-click="incPage()">Next</a></li>
				</ul>
			</div>
			<div class="col-sm-1">
				show by:
				<select class="form-control" ng-change="changeShowBy()" ng-model="showby" ng-options="x for x in showbys" />
			</div>
		</div>
	</div>
</asp:Content>
