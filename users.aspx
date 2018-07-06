<%@ Page Title="Users" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="users.aspx.cs" Inherits="Users" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/users_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/users_js")%>
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
					<th>Address</th>
					<th>Login</th>
					<th>Password</th>
					<th>Phone</th>
					<th>Admin</th>
					<th>WORKFLOW</th>
					<th>RETIRED</th>
				</tr>
			</thead>
			<tbody>
				<tr ng-repeat="u in users | orderBy : 'PERSON_NAME'" class="{{u.changed?'data-changed':''}}">
					<td><input ng-disabled="readonly" class="intable-data-input" type="text" ng-model="u.PERSON_NAME" ng-change="itemchanged(u)"></td>
					<td><input ng-disabled="readonly" class="intable-data-input" type="text" ng-model="u.ADDRESS" ng-change="itemchanged(u)"></td>
					<td><input ng-disabled="readonly" class="intable-data-input" type="text" ng-model="u.LOGIN" ng-change="itemchanged(u)"></td>
					<td><input ng-disabled="readonly" class="intable-data-input" type="password" ng-model="u.PASSWORD" ng-change="itemchanged(u)" placeholder="********"></td>
					<td><input ng-disabled="readonly" class="intable-data-input" type="text" ng-model="u.PHONE" ng-change="itemchanged(u)"></td>
					<td align="center"><input ng-disabled="readonly" type="checkbox" ng-model="u.ISADMIN" ng-change="itemchanged(u)"></td>
					<td align="center"><input ng-disabled="readonly" type="checkbox" ng-model="u.INWORK" ng-change="itemchanged(u)"></td>
					<td align="center"><input ng-disabled="readonly" type="checkbox" ng-model="u.RETIRED" ng-change="itemchanged(u)"></td>
				</tr>
			</tbody>
		</table>
	</div>
</asp:Content>
