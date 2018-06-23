<%@ Page Title="Dispositions" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="dispositions.aspx.cs" Inherits="Dispositions" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/dispositions_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/dispositions_js")%>
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
					<th>Disposition name</th>
					<th>Color</th>
					<th>Order</th>
					<th>Requires Work</th>
					<th>Working Now</th>
				</tr>
			</thead>
			<tbody>
				<tr ng-repeat="d in dispos | orderBy : 'FORDER'">
					<td ng-click="enterdata(d, 'DESCR')">{{d.DESCR}}</td>
					<td ng-click="enterdata(d, 'COLOR')">{{d.COLOR}}</td>
					<td ng-click="enterdata(d, 'FORDER')">{{d.FORDER}}</td>
					<td ng-click="enterdata(d, 'REQUIREWORK')">{{d.REQUIREWORK}}</td>
					<td ng-click="enterdata(d, 'WORKING')">{{d.WORKING}}</td>
				</tr>
			</tbody>
		</table>
	</div>
</asp:Content>
