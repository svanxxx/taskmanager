<%@ Page Title="Daily Reports Search" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="dailysearch.aspx.cs" Inherits="DailySearch" %>

<%@ Register Src="~/controls/DefectSpentControl.ascx" TagName="defSpent" TagPrefix="uc" %>
<%@ Register Src="~/controls/DefectNumControl.ascx" TagName="defNum" TagPrefix="uc" %>
<%@ Register Src="~/controls/DefectEstControl.ascx" TagName="defEst" TagPrefix="uc" %>
<%@ Register Src="~/controls/DefectDBControl.ascx" TagName="defDB" TagPrefix="uc" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/dailysearch_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/dailysearch_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="row" ng-cloak>
			<div class="col-lg-2">
				<div class="card text-center">
					<div class="card-header">
						<img class="rounded-circle" ng-src="{{'getUserImg.ashx?sz=100&id=' + state.filter.userid}}" alt="Smile" height="100" width="100">
					</div>
				</div>
			</div>
			<div class="col-lg-8">
				<div class="jumbotron p-1 mb-0 text-center">
					<h2><i class="fas fa-search"></i>Daily Reports Search</h2>
				</div>
				<div class="card">
					<div class="card-header">
						<div class="d-flex">
							<div class="input-group input-group-sm">
								<div class="input-group-prepend">
									<span class="input-group-text">Person:</span>
								</div>
								<select ng-model="state.filter.userid" class="form-control">
									<option ng-repeat="u in mpsusers" value="{{u.ID}}">{{u.PERSON_NAME}}</option>
								</select>
							</div>
							<div class="input-group input-group-sm">
								<div class="input-group-prepend">
									<span class="input-group-text d-none d-xl-block">From:</span>
								</div>
								<input required ng-model="state.filter.startdate" class="form-control" type="date" />
							</div>
							<div class="input-group input-group-sm">
								<div class="input-group-prepend">
									<span class="input-group-text d-none d-xl-inline">Through:</span>
								</div>
								<input required ng-model="state.filter.enddate" class="form-control" type="date" />
							</div>
							<div class="input-group input-group-sm">
								<div class="input-group-prepend">
									<span class="input-group-text d-none d-xl-inline">Text:</span>
								</div>
								<input required ng-model="state.filter.text" class="form-control" type="search" ng-keypress="onGo($event)" autofocus />
							</div>
							<button id="searchbtn" type="button" class="btn btn-block btn-primary btn-sm" ng-click="loadData(false)">Load</button>
						</div>
					</div>
					<div class="card-body">
						<ul class="list-group list-group-flush">
							<li ng-repeat="r in state.reports" class="list-group-item">
								<div class="media">
									<div class="media-body">
										<div ng-repeat="l in r.DONE track by $index" ng-bind-html="l | rawHtml"></div>
										<div class="list-group">
											<div ng-repeat="e in r.TASKSEVENTS" class="list-group-item p-1">
												<uc:defSpent member="e" runat="server" />
												<uc:defNum member="e.DEFECT" runat="server" />
												<uc:defEst member="e.DEFECT" runat="server" />
												<uc:defDB member="e.DEFECT" runat="server" />
												<span data-toggle="tooltip" title="{{e.DEFECT.SUMMARY}}" ng-bind-html="e.DEFECT.SUMMARY | sumFormat | limitTo:135"></span>
											</div>
										</div>
									</div>
									<small><i>{{r.DATE}}</i></small>
								</div>
							</li>
						</ul>
					</div>
				</div>
			</div>
			<div class="col-lg-2"></div>
		</div>
	</div>
</asp:Content>
