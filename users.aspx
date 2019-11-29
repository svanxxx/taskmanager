<%@ Page Title="Users" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="users.aspx.cs" Inherits="Users" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/users_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/users_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="alert alert-danger savebutton btn-group-vertical" ng-cloak ng-show="changed">
			<button type="button" class="btn btn-lg btn-info" ng-click="save()">Save</button>
			<button type="button" class="btn btn-lg btn-danger" ng-click="discard()">Discard</button>
		</div>
		<div class="table-responsive table-sm">
			<table class="table table-hover table-bordered">
				<thead class="thead-dark">
					<tr class="info">
						<th>IM</th>
						<th>Name</th>
						<th>Birthday</th>
						<th>Address</th>
						<th>Login</th>
						<th>Password</th>
						<th>Email</th>
						<th>Phone</th>
						<th>Admin</th>
						<th>WORKFLOW</th>
						<th>RETIRED</th>
						<th>CLIENT</th>
					</tr>
				</thead>
				<tbody>
					<tr ng-repeat="u in users" class="{{(u.changed?'data-changed':'')+(u.RETIRED ? 'table-secondary':'')}}">
						<td>
							<button ng-click="changeImg(u.ID)" type="button" class="btn btn-info p-0">
								<img class="rep-img" ng-src="{{'getUserImg.ashx?sz=25&id='+u.ID}}" alt=" " height="25" width="25"></button></td>
						<td>
							<input class="form-control form-control-sm border-0" ng-disabled="readonly" class="intable-data-input" type="text" ng-model="u.PERSON_NAME" ng-change="itemchanged(u)"></td>
						<td align="center">
							<input class="form-control form-control-sm border-0" ng-disabled="readonly" type="date" ng-model="u.BIRTHDAY" ng-change="itemchanged(u)"></td>
						<td>
							<input class="form-control form-control-sm border-0" ng-disabled="readonly" class="intable-data-input" type="text" ng-model="u.ADDRESS" ng-change="itemchanged(u)"></td>
						<td>
							<input class="form-control form-control-sm border-0" ng-disabled="readonly" class="intable-data-input" type="text" ng-model="u.LOGIN" ng-change="itemchanged(u)"></td>
						<td>
							<input class="form-control form-control-sm border-0" ng-disabled="readonly" class="intable-data-input" type="password" ng-model="u.PASSWORD" ng-change="itemchanged(u)" placeholder="********"></td>
						<td>
							<input class="form-control form-control-sm border-0" ng-disabled="readonly" class="intable-data-input" type="text" ng-model="u.EMAIL" ng-change="itemchanged(u)"></td>
						<td>
							<input class="form-control form-control-sm border-0" ng-disabled="readonly" class="intable-data-input" type="text" ng-model="u.PHONE" ng-change="itemchanged(u)"></td>
						<td align="center">
							<input class="form-check-input" ng-disabled="readonly" type="checkbox" ng-model="u.ISADMIN" ng-change="itemchanged(u)"></td>
						<td align="center">
							<input class="form-check-input" ng-disabled="readonly" type="checkbox" ng-model="u.INWORK" ng-change="itemchanged(u)"></td>
						<td align="center">
							<input class="form-check-input" ng-disabled="readonly" type="checkbox" ng-model="u.RETIRED" ng-change="itemchanged(u)"></td>
						<td align="center">
							<input class="form-check-input" ng-disabled="readonly" type="checkbox" ng-model="u.ISCLIENT" ng-change="itemchanged(u)"></td>
					</tr>
				</tbody>
			</table>
			<button type="button" class="btn btn btn-outline-secondary btn-sm" ng-show="!readonly" ng-click="newUser()">Add New User</button>
			<button type="button" class="btn btn btn-outline-secondary btn-sm" ng-show="!readonly" ng-click="filter = !filter">Toggle Active Users</button>
		</div>
	</div>
</asp:Content>
