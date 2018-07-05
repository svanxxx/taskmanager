<%@ Page Title="Daily Report" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="dailyreport.aspx.cs" Inherits="DailyReport" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/dailyreport_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/dailyreport_js")%>
	<script src="http://mps.resnet.com/cdn/angular/angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="row">
			<div class="col-sm-2">
				<h4>Daily Report for date: </h4>
			</div>
			<div class="col-sm-2">
				<input type="date" id="date" class="form-control input-sm" ng-model="today" ng-change="changeDate()" ng-disabled="!loaded()">
			</div>
		</div>
		<table class="table table-hover table-bordered">
			<colgroup>
				<col width="10%" />
				<col width="30%" />
				<col width="30%" />
				<col width="30%" />
			</colgroup>
			<thead>
				<tr class="info">
					<th>Person</th>
					<th>Yesterday</th>
					<th>Today</th>
					<th>Plan</th>
				</tr>
			</thead>
			<tbody>
				<tr ng-repeat="u in users | filter:{ INWORK: true } | orderBy : 'PERSON_NAME'">
					<td>
						<img ng-src="{{u.ID | getUserImgById:this}}" alt="Smile" height="20" width="20">
						{{u.PERSON_NAME}}
					</td>
					<td>
						<div ng-repeat="v in vacations | filter: { AUSER : u.TTUSERID, DATE: yesterdaystring }">
							<a href="showtask.aspx?ttid={{v.ID}}" target="_blank">
								<h3><span class="glyphicon glyphicon-plane"></span></h3>
							</a>
						</div>
						<div ng-repeat="l in u.YESTERDAY track by $index">
							<span>{{l | limitTo:80}}</span><br>
						</div>
					</td>
					<td>
						<div ng-repeat="v in vacations | filter: { AUSER : u.TTUSERID, DATE: todaystring }">
							<a href="showtask.aspx?ttid={{v.ID}}" target="_blank">
								<h3><span class="glyphicon glyphicon-plane"></span></h3>
							</a>
						</div>
						<div ng-repeat="l in u.TODAY track by $index">
							<span>{{l | limitTo:80}}</span><br>
						</div>
					</td>
					<td>
						<img src="IMAGES/process.gif" ng-hide="planLoaded(u.ID)" />
						<div>
							<h3 class="vacation-box">{{getUpcomingdays(u)}}</h3>
							<a href="showtask.aspx?ttid={{v.ID}}" target="_blank" ng-repeat="v in vacations | filter: { AUSER : u.TTUSERID } | filter: {DATE: '!' + todaystring} | filter: {DATE: '!' + yesterdaystring}">
								<h3 class="vacation-box"><span class="glyphicon glyphicon-plane"></span></h3>
							</a>
						</div>
						<div ng-repeat="d in u.PLAN track by $index" ng-style="{{d.DISPO | getDispoColorById:this}}" class="task">
							<a href="showtask.aspx?ttid={{d.ID}}" target="_blank">
								<span class="badge">{{d.ID}}</span>
							</a>
							<span class="label label-danger">{{d.ESTIM}}</span>
							<span data-toggle="tooltip" title="{{d.SUMMARY}}">{{d.SUMMARY | limitTo:80}}</span>
						</div>
					</td>
				</tr>
			</tbody>
		</table>
	</div>
</asp:Content>
