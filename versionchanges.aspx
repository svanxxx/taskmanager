<%@ Page Title="Change Log" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="versionchanges.aspx.cs" Inherits="VersionChanges" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/versionchanges_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/versionchanges_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="row">
			<div class="col-lg-2">
				<div class="alert alert-info">
					<span class="glyphicon glyphicon-globe"></span>
					<strong>FlexLM server</strong> installation - follow next link: <a href="getinstall.ashx?type=flex&version=0.0.0" target="_blank" class="alert-link">click here</a>.
				</div>
				<div class="alert alert-info">
					<span class="glyphicon glyphicon-globe"></span>
					<strong>Fieldpro Client</strong> installation - follow next link: <a href="getinstall.ashx?type=client&version=0.0.0" target="_blank" class="alert-link">click here</a>.
				</div>
			</div>
			<div class="col-lg-8">
				<h2>Change Log</h2>
				<div class="panel-group">
					<div ng-repeat="v in versions" class="panel panel-primary">
						<div class="panel-heading">
							<div class="btn-group">
								<span class="btn btn-info">{{v.version}}</span>
								<button ng-click="GetFile(v, 'efip')" type="button" class="btn btn-primary"><span class="glyphicon glyphicon-download"></span>&nbsp;eFieldpro</button>
								<button ng-click="GetFile(v, 'cx')" type="button" class="btn btn-primary"><span class="glyphicon glyphicon-download"></span>&nbsp;Models</button>
								<button ng-click="GetFile(v, 'onsite')" type="button" class="btn btn-primary"><span class="glyphicon glyphicon-download"></span>&nbsp;Onsite</button>
								<button ng-click="GetFile(v, 'demo')" type="button" class="btn btn-primary"><span class="glyphicon glyphicon-download"></span>&nbsp;Demo</button>
							</div>
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
