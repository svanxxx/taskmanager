<%@ Page Title="Change Log" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="versionchanges.aspx.cs" Inherits="VersionChanges" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/versionchanges_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/versionchanges_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="row">
			<div class="col-sm-12">
				<h2><span class="glyphicon glyphicon-dashboard"></span>&nbsp;Change Log</h2>
			</div>
		</div>
		<div class="row">
			<div class="col-lg-2">
				<div class="alert alert-info">
					<span class="glyphicon glyphicon-cloud-download"></span>
					<strong>FlexLM server</strong> installation - follow next link: <span class="label label-info"><a href="getinstall.ashx?type=flex&version=0.0.0" target="_blank" class="alert-link">click here</a></span>.
				</div>
				<div class="alert alert-info">
					<span class="glyphicon glyphicon-cloud-download"></span>
					<strong>Fieldpro Client</strong> installation - follow next link: <span class="label label-info"><a href="getinstall.ashx?type=client&version=0.0.0" target="_blank" class="alert-link">click here</a></span>.
				</div>
				<div class="alert alert-info">
					<span class="glyphicon glyphicon-book"></span>
					<strong>Fieldpro Server Installation Guide</strong> follow next link: 
					<span class="label label-info">
						<a href="<%=Settings.CurrentSettings.INSTALLGUIDE.ToString()%>" target="_blank" class="alert-link">click here</a>
					</span>.
				</div>
				<div class="alert alert-info">
					<span class="glyphicon glyphicon-tasks"></span>
					<strong>MS SQL server </strong> follow next link: 
					<span class="label label-info">
						<a href="https://www.microsoft.com/en-us/sql-server/sql-server-downloads" target="_blank" class="alert-link">click here</a>
					</span>.
				</div>

				
			</div>
			<div class="col-lg-8">
				<div class="panel-group">
					<div ng-repeat="v in versions" class="panel panel-info">
						<div class="panel-heading">
							<div class="btn-group">
								<span class="btn btn-info"><span class="glyphicon glyphicon-tag"></span>&nbsp;{{v.version}}</span>
								<button ng-click="GetFile(v, 'efip')" type="button" class="btn btn-default"><span class="glyphicon glyphicon-cloud-download"></span>&nbsp;eFieldpro</button>
								<button ng-click="GetFile(v, 'cx')" type="button" class="btn btn-default"><span class="glyphicon glyphicon-cloud-download"></span>&nbsp;Models</button>
								<button ng-click="GetFile(v, 'onsite')" type="button" class="btn btn-default"><span class="glyphicon glyphicon-cloud-download"></span>&nbsp;Onsite</button>
								<button ng-click="GetFile(v, 'demo')" type="button" class="btn btn-default"><span class="glyphicon glyphicon-cloud-download"></span>&nbsp;Demo</button>
							</div>
						</div>
						<div class="panel-body">
							<div ng-repeat="c in v.changes">
								<a href="showtask.aspx?ttid={{c.ttid}}" target="_blank">
									<span class="badge alert-info">{{c.ttid}}</span>
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
