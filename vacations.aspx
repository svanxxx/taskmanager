<%@ Page Title="Vacations" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="vacations.aspx.cs" Inherits="Vacations" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/vacations_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/vacations_js")%>
	<script src="http://mps.resnet.com/cdn/angular/angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<table id="headertable" class="table table-bordered table-dataheader">
			<colgroup>
				<col style="width: {{50/(users.length + 1)}}%">
				<col style="width: {{50/(users.length + 1)}}%">
				<col style="width: {{100/(users.length + 1)}}%" ng-repeat="u in users">
			</colgroup>
			<thead>
				<tr>
					<th>Mon</th>
					<th>Day</th>
					<th ng-repeat="u in users | orderBy : 'PERSON_NAME'">{{u.LOGIN}}</th>
				</tr>
			</thead>
		</table>
		<table id="datatable" class="table table-bordered table-datadata">
			<colgroup>
				<col style="width: {{50/(users.length + 1)}}%">
				<col style="width: {{50/(users.length + 1)}}%">
				<col style="width: {{100/(users.length + 1)}}%" ng-repeat="u in users">
			</colgroup>
			<tbody>
				<tr ng-repeat="d in days" style="background-color: {{d.getMonth() % 2 ? 'white' : 'lightgray'}}" year="{{d.getFullYear()}}" month="{{d.getMonth()+1}}" day="{{d.getDate()}}">
					<td class="rotate" style="vertical-align: middle; display: {{d.getDate() == 1 ? 'table-cell' : 'none'}}" rowspan="{{d.getDate() == 1 ? d.monthDays() : 1}}">
						{{monthNames[d.getMonth()]}}
					</td>
					<td week="{{d.getDay()}}" style="background-color: {{(d.getDay() == 6 || d.getDay() == 0) ? 'DodgerBlue' : (d.valueOf() == today.valueOf() ? 'yellow' : '')}}">{{d.getDate()}}
					</td>
					<td style="background-color: {{(d.getDay() == 6 || d.getDay() == 0) ? 'DodgerBlue' : (d.valueOf() == today.valueOf() ? 'yellow' : '')}}" ng-repeat="u in users | orderBy : 'PERSON_NAME'">
						<a ng-show="hasVacation(u, d)" href="showtask.aspx?ttid={{getVacation(u, d)}}" target="_blank">
							<span class="glyphicon glyphicon-plane"></span>
						</a>
					</td>
				</tr>
			</tbody>
		</table>
	</div>
</asp:Content>
