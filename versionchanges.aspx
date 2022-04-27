<%@ Page Title="Change Log" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="versionchanges.aspx.cs" Inherits="VersionChanges" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/versionchanges_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/versionchanges_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<input type="hidden" id="TESTAPIURL" value="<%=Settings.CurrentSettings.TESTAPIURL.ToString()%>" />
	<input type="hidden" id="TESTAPIKEY" value="<%=Settings.CurrentSettings.TESTAPIKEY.ToString()%>" />
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
					<strong>MS SQL server </strong>follow next link: 
					<span class="badge badge-info">
						<a href="https://www.microsoft.com/en-us/sql-server/sql-server-downloads" target="_blank" class="badge badge-light">click here</a>
					</span>.
				</div>
				<div class="alert alert-info" ng-show="isadmin" ng-click="alertVersion()">
					<button type="button" class="btn btn-info btn-s">
						<strong>Alert Version build</strong>&nbsp;&nbsp;&nbsp;&nbsp;<i class="fas fa-bell"></i>
					</button>
				</div>
			</div>
			<div class="col-lg-8">
				<div class="panel-group">
					<div ng-repeat="v in versions" class="card">
						<div class="card-header">
							<div class="btn-group">
								<button id="{{v.version}}" ng-click="copyurl(v.version)" type="button" class="btn bg-light"><i class="fas fa-tags"></i>&nbsp;{{v.version}}</button>
								<button <%=CurrentContext.Client ? "disabled" : ""%> ng-click="GetFile(v, 'efip')" type="button" class="btn bg-light"><i class="fas fa-cloud-download-alt"></i>&nbsp;Fieldpro</button>
								<button <%=CurrentContext.Client ? "disabled" : ""%> ng-click="GetFile(v, 'cx')" type="button" class="btn bg-light"><i class="fas fa-cloud-download-alt"></i>&nbsp;Models</button>
								<button <%=CurrentContext.Client ? "disabled" : ""%> ng-click="GetFile(v, 'onsite')" type="button" class="btn bg-light"><i class="fas fa-cloud-download-alt"></i>&nbsp;Onsite</button>
								<button <%=CurrentContext.Client ? "disabled" : ""%> ng-click="GetFile(v, 'demo')" type="button" class="btn bg-light"><i class="fas fa-cloud-download-alt"></i>&nbsp;Demo</button>
								<button ng-click="selectTestVersion(v.testVersion)" data-toggle="modal" data-target="#testsleModal" type="button" class="btn bg-light"><i class="fas fa-bug"></i></button>
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
		<style>
			td, th {
				padding: 0px !important;
			}
		</style>
		<div class="modal fade" id="testsleModal" tabindex="-1" role="dialog" aria-labelledby="testsleModalLabel" aria-hidden="true">
			<div class="modal-dialog modal-dialog-centered" role="document">
				<div class="modal-content">
					<div class="modal-header">
						<h5 class="modal-title" id="testsleModalLabel">Test version: {{testVersion}}</h5>
						<button type="button" class="close" data-dismiss="modal" aria-label="Close">
							<span aria-hidden="true">&times;</span>
						</button>
					</div>
					<div class="modal-body">
						<table class="table table-sm table-bordered">
							<thead class="table-dark">
								<tr class="text-center">
									<th scope="col">#</th>
									<th scope="col">Test</th>
									<th scope="col">ex</th>
									<th scope="col">db</th>
									<th scope="col">ou</th>
									<th scope="col">er</th>
									<th scope="col">wr</th>
								</tr>
							</thead>
							<tbody>
								<tr ng-repeat="t in tests" ng-style="{'background-color':t.color}">
									<th class="text-center" scope="row">{{t.row}}</th>
									<td class="d-flex">
										<span>{{t.testcase}}</span>
										<i ng-click="showTestDesc(t)" data-toggle="modal" data-target="#descModalLong" class="ml-auto fas fa-question-circle" style="cursor: pointer"></i>
									</td>
									<td class="text-center">{{t.ex}}</td>
									<td class="text-center">{{t.db}}</td>
									<td class="text-center">{{t.ou}}</td>
									<td class="text-center">{{t.er}}</td>
									<td class="text-center">{{t.wr}}</td>
								</tr>
							</tbody>
						</table>
						<div ng-show="!tests" class="text-center">
							<img style="width: 1em" src="images/process.gif" />
							Loading data...
						</div>
					</div>
					<div class="modal-footer">
						<div class="dropdown mr-auto">
							<button class="btn btn-outline-secondary dropdown-toggle" type="button" id="dropdownMenuButton" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
								Test filter: <strong>{{testFilter}}</strong>
							</button>
							<div class="dropdown-menu" aria-labelledby="dropdownMenuButton" ng-show="!isClient">
								<div style="cursor: pointer" class="dropdown-item" ng-repeat="f in filters" ng-click="selectTestFilter(f.name)">{{f.name}}</div>
							</div>
						</div>
						<button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
					</div>
				</div>
			</div>
		</div>
		<div class="modal fade" id="descModalLong" tabindex="-1" role="dialog" aria-labelledby="descModalLongTitle" aria-hidden="true">
			<div class="modal-dialog modal-dialog-centered modal-lg" role="document">
				<div class="modal-content">
					<div class="modal-header">
						<h5 class="modal-title" id="descModalLongTitle">Test Description</h5>
						<button type="button" class="close" data-dismiss="modal" aria-label="Close">
							<span aria-hidden="true">&times;</span>
						</button>
					</div>
					<div class="modal-body">
						<textarea ng-disabled="true" ng-model="testDesc" class="form-control" id="exampleFormControlTextarea1" rows="30" style="font-size:1em"></textarea>
					</div>
					<div class="modal-footer">
						<button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
					</div>
				</div>
			</div>
		</div>
	</div>
</asp:Content>
