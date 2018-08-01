<%@ Page Title="Version Changes" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="versionchanges.aspx.cs" Inherits="VersionChanges" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/versionchanges_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/versionchanges_js")%>
	<script src="http://mps.resnet.com/cdn/angular/angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<h2>Version changes</h2>
		<div class="panel-group">
			<div ng-repeat="v in versions" class="panel panel-primary">
				<div class="panel-heading">
					<span>{{v.version}}</span>
					<button type="button" class="btn btn-default btn-xs fip-down" ng-click="GetFile(v, 'efip')">
						<span class="glyphicon glyphicon-download"></span>FiP
					</button>
					<button type="button" class="btn btn-default btn-xs fip-down" ng-click="GetFile(v, 'cx')">
						<span class="glyphicon glyphicon-download"></span>CX
					</button>
					<button type="button" class="btn btn-default btn-xs fip-down" ng-click="GetFile(v, 'onsite')">
						<span class="glyphicon glyphicon-download"></span>Onsite
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
</asp:Content>
