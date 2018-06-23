<%@ Page Title="Severities" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="severities.aspx.cs" Inherits="Severities" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/severities_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/severities_js")%>
	<script src="http://mps.resnet.com/cdn/angular/angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="alert alert-danger savebutton btn-group-vertical" ng-cloak ng-show="changed">
			<button type="button" class="btn btn-lg btn-info" ng-click="save()">Save</button>
			<button type="button" class="btn btn-lg btn-danger" ng-click="discard()">Discard</button>
		</div>
		<table class="table table-hover table-bordered">
			<thead>
				<tr class="info">
					<th>Name</th>
					<th>Order</th>
					<th>Plan</th>
				</tr>
			</thead>
			<tbody>
				<tr ng-repeat="d in severs | orderBy : 'FORDER'" class="{{d.changed?'data-changed':''}}">
					<td ng-click="enterdata(d, 'DESCR')">{{d.DESCR}}</td>
					<td ng-click="enterdata(d, 'FORDER')">{{d.FORDER}}</td>
					<td ng-click="enterdata(d, 'PLAN')">{{d.PLAN}}</td>
				</tr>
			</tbody>
		</table>
	</div>
</asp:Content>
