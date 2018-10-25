<%@ Page Title="Builds" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="builds.aspx.cs" Inherits="Builds" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/builds_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/builds_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
	<script src="scripts/jquery.signalR-2.3.0.min.js"></script>
	<script src="signalr/hubs"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="panel panel-info" ng-cloak>
			<div class="panel-heading"></div>
			<div class="panel-body">
				<div class="list-group">
					<div class="list-group-item" ng-repeat="b in builds" target="_blank">
						<div class="row" style="background-color: {{b.COLOR}}">
							<div class="col-sm-1">
								<span class="glyphicon glyphicon-time"></span><span>{{b.DATE}}</span>
							</div>
							<div class="col-sm-1">
								<div class="col-sm-1">
									<a href="showtask.aspx?ttid={{b.TTID}}" target="_blank">
										<span class="badge">{{b.TTID}}</span>
										<span>{{b.STATUS}}</span>
									</a>
								</div>
							</div>
							<div class="col-sm-2">
								<img class="rep-img img-circle" src="{{'getUserImg.ashx?ttid=' + b.TTUSERID}}" alt="Smile" height="20" width="20">
								<span class="glyphicon glyphicon-comment"></span><span>{{b.NOTES}}</span>
							</div>
							<div class="col-sm-2">
								<span class="glyphicon glyphicon-blackboard"></span><span>{{b.MACHINE}}</span>
								<div class="progress" ng-show="b.STATUS.includes('wait')==true">
									<div class="progress-bar progress-bar-striped active progress-bar-warning" role="progressbar" style="width: 100%">
										{{b.STATUS}}...
									</div>
								</div>
								<div class="progress" ng-show="b.STATUS.includes('Building')==true">
									<div class="progress-bar progress-bar-striped active" role="progressbar" style="width: 100%">
										{{b.STATUSTXT?b.STATUSTXT:b.STATUS}}
									</div>
								</div>
							</div>
							<div class="col-sm-1">
								<span class="glyphicon glyphicon-fire"></span>
								<span>{{b.DATEUP}}</span>
							</div>
							<div class="col-sm-4">
								<a href="getBuildLog.ashx?id={{b.ID}}">{{b.STATUSTXT}}
								</a>
							</div>
						</div>
					</div>
				</div>
			</div>
		</div>
	</div>
</asp:Content>
