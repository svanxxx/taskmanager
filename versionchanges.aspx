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
				<h2><i class="fab fa-joomla"></i>&nbsp;Change Log</h2>
			</div>
		</div>
		<div class="row">
			<div class="col-lg-2">
				<div class="alert alert-info">
					<i class="fas fa-cloud-download-alt"></i>
					<strong>FlexLM server</strong> installation - follow next link: <span class="badge badge-info"><a href="getinstall.ashx?type=flex&version=0.0.0" target="_blank" class="badge badge-light">click here</a></span>.
				</div>
				<div class="alert alert-info">
					<i class="fas fa-cloud-download-alt"></i>
					<strong>Fieldpro Client</strong> installation - follow next link: <span class="badge badge-info"><a href="getinstall.ashx?type=client&version=0.0.0" target="_blank" class="badge badge-light">click here</a></span>.
				</div>
				<div class="alert alert-info">
					<i class="fas fa-book"></i>
					<strong>Fieldpro Server Installation Guide</strong> follow next link: 
					<span class="badge badge-info">
						<a href="<%=Settings.CurrentSettings.INSTALLGUIDE.ToString()%>" target="_blank" class="badge badge-light">click here</a>
					</span>.
				</div>
				<div class="alert alert-info">
					<i class="fas fa-database"></i>
					<strong>MS SQL server </strong> follow next link: 
					<span class="badge badge-info">
						<a href="https://www.microsoft.com/en-us/sql-server/sql-server-downloads" target="_blank" class="badge badge-light">click here</a>
					</span>.
				</div>
			</div>
			<div class="col-lg-8">
				<div class="panel-group">
					<div ng-repeat="v in versions" class="card">
						<div class="card-header">
							<div class="btn-group">
								<button ng-click="copyurl(v.version)" type="button" class="btn bg-light"><i class="fas fa-tags"></i>&nbsp;{{v.version}}</button>
								<button ng-click="GetFile(v, 'efip')" type="button" class="btn bg-light"><i class="fas fa-cloud-download-alt"></i>&nbsp;Fieldpro</button>
								<button ng-click="GetFile(v, 'cx')" type="button" class="btn bg-light"><i class="fas fa-cloud-download-alt"></i>&nbsp;Models</button>
								<button ng-click="GetFile(v, 'onsite')" type="button" class="btn bg-light"><i class="fas fa-cloud-download-alt"></i>&nbsp;Onsite</button>
								<button ng-click="GetFile(v, 'demo')" type="button" class="btn bg-light"><i class="fas fa-cloud-download-alt"></i>&nbsp;Demo</button>
							</div>
						</div>
						<div class="card-body">
							<div ng-repeat="c in v.changes">
								<a href="showtask.aspx?ttid={{c.ttid}}" target="_blank">
									<span class="badge badge-pill badge-secondary">{{c.ttid}}</span>
								</a>
								<span ng-bind-html="c.summary | sumFormat"></span>
							</div>
						</div>
					</div>
				</div>
			</div>
			<div class="col-lg-2"></div>
		</div>
	</div>
</asp:Content>
