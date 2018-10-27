<%@ Page Title="Change Log" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="versionchanges.aspx.cs" Inherits="VersionChanges" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/versionchanges_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/versionchanges_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="row">
			<div class="col-lg-2"></div>
			<div class="col-lg-8">
				<h2>Change Log</h2>
				<div class="panel-group">
					<div ng-repeat="v in versions" class="panel panel-primary">
						<div class="panel-heading">
							<span>{{v.version}}</span>
							<button type="button" class="btn-efieldpro btn btn-default btn-xs fip-down btn-efieldpro" ng-click="GetFile(v, 'efip')">
								<span class="glyphicon glyphicon-download"></span>eFieldpro
							</button>
							<button type="button" class="btn btn-default btn-xs fip-down btn-models" ng-click="GetFile(v, 'cx')">
								<span class="glyphicon glyphicon-download"></span>Models
							</button>
							<button type="button" class="btn btn-default btn-xs fip-down btn-onsite" ng-click="GetFile(v, 'onsite')">
								<span class="glyphicon glyphicon-download"></span>Onsite
							</button>
							<button type="button" class="btn btn-default btn-xs fip-down btn-demo" ng-click="GetFile(v, 'demo')">
								<span class="glyphicon glyphicon-download"></span>Demo
							</button>
							<button type="button" class="btn btn-default btn-xs fip-down btn-client" ng-click="GetFile(v, 'client')">
								<span class="glyphicon glyphicon-download"></span>Client
							</button>
						</div>
						<div class="panel-body">
							<div ng-repeat="c in v.changes">
								<a href="showtask.aspx?ttid={{c.ttid}}" target="_blank">
									<span class="badge">{{c.ttid}}</span>
								</a>
								<span>{{c.summary}}</span>
							</div>
						</div>
					</div>
				</div>
			</div>
			<div class="col-lg-2"></div>
		</div>
	</div>
</asp:Content>
