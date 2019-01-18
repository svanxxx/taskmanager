<%@ Page Title="Daily Reports Search" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="dailysearch.aspx.cs" Inherits="DailySearch" %>

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
						<img class="rounded-circle" ng-src="{{'getUserImg.ashx?id=' + state.filter.userid}}" alt="Smile" height="100" width="100">
					</div>
				</div>
			</div>
			<div class="col-lg-8">
				<div class="jumbotron p-1 mb-0 text-center">
					<h2><i class="fas fa-search"></i> Daily Reports Search</h2>
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
								<input required ng-model="state.filter.text" class="form-control" type="text" onkeydown="if (event.keyCode == 13) document.getElementById('searchbtn').click()" />
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
