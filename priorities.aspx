<%@ Page Title="Priorities" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="priorities.aspx.cs" Inherits="Priorities" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/priorities_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/priorities_js")%>
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
					<th>Type name</th>
					<th>Order</th>
				</tr>
			</thead>
			<tbody>
				<tr ng-repeat="d in priors | orderBy : 'FORDER'" class="{{d.changed?'data-changed':''}}">
					<td ng-click="enterdata(d, 'DESCR')">{{d.DESCR}}</td>
					<td ng-click="enterdata(d, 'FORDER')">{{d.FORDER}}</td>
				</tr>
			</tbody>
		</table>
	</div>
</asp:Content>
