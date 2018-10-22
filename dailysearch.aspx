<%@ Page Title="Daily Reports Search" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="dailysearch.aspx.cs" Inherits="DailySearch" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/dailysearch_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/dailysearch_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="row">
			<div class="col-lg-2"></div>
			<div class="col-lg-8">
				<h2 class="rep-cap">Daily Reports Search</h2>
				<div class="panel panel-primary">
					<div class="panel-heading">
						<div class="row row-eq-height">
							<div class="col-lg-1">
								<img class="img-circle" ng-src="{{'getUserImg.ashx?id=' + selectedpersonID}}" alt="Smile" height="60" width="60">
							</div>
							<div class="col-lg-4">
								<label>Person:</label>
								<select ng-change="loadData(false)" ng-model="selectedpersonID" class="form-control">
									<option ng-repeat="u in mpsusers" value="{{u.ID}}">{{u.PERSON_NAME}}</option>
								</select>
							</div>
							<div class="col-lg-3">
								<label>From:</label>
								<input required ng-model="startdate" ng-change="loadData(false)" class="form-control" type="date" />
							</div>
							<div class="col-lg-3">
								<label>Through:</label>
								<input required ng-model="enddate" ng-change="loadData(false)" class="form-control" type="date" />
							</div>
						</div>
					</div>
					<div class="panel-body">
						<div class="panel-group">
							<div ng-repeat="r in reports" class="panel panel-info">
								<div class="panel-heading">
									<span class="glyphicon glyphicon-hourglass"></span>
									<span>{{r.DATE}}</span>
									<img class="img-circle rep-img" src="{{'getUserImg.ashx?id='+selectedpersonID}}" alt="Smile" height="20" width="20">
								</div>
								<div class="panel-body">
									<div ng-repeat="l in r.DONE">
										<h5 ng-bind-html="l | rawHtml">
											<h5>
									</div>
								</div>
							</div>
						</div>
					</div>
				</div>
			</div>
			<div class="col-lg-2"></div>
		</div>
	</div>
</asp:Content>
