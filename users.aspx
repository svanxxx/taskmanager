<%@ Page Title="Users" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="users.aspx.cs" Inherits="Users" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<link href="css/users.css" rel="stylesheet" />
	<script src="scripts/users.js"></script>
	<script src="http://mps.resnet.com/cdn/angular/angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="alert alert-danger savebutton btn-group-vertical" ng-cloak ng-show="changed">
			<button type="button" class="btn btn-lg btn-danger" ng-click="save()">Save</button>
			<button type="button" class="btn btn-lg btn-danger" ng-click="discard()">Discard</button>
		</div>
		<table class="table table-hover table-bordered">
			<thead>
				<tr class="info">
					<th>Name</th>
					<th>Address</th>
					<th>Login</th>
					<th>Password</th>
					<th>Phone</th>
					<th>WORK</th>
				</tr>
			</thead>
			<tbody>
				<tr ng-repeat="u in users">
					<td ng-click="enterdata(u, 'PERSON_NAME')">{{u.PERSON_NAME}}</td>
					<td ng-click="enterdata(u, 'ADDRESS')">{{u.ADDRESS}}</td>
					<td ng-click="enterdata(u, 'LOGIN')">{{u.LOGIN}}</td>
					<td ng-click="enterdata(u, 'PASSWORD')">{{u.PASSWORD | passwordFilter}}</td>
					<td ng-click="enterdata(u, 'PHONE')">{{u.PHONE}}</td>
					<td ng-click="enterdata(u, 'INWORK')">{{u.INWORK}}</td>
				</tr>
			</tbody>
		</table>
	</div>
</asp:Content>
