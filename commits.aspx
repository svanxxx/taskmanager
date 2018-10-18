<%@ Page Title="Branches" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="branches.aspx.cs" Inherits="Branches" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/commits_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/commits_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="alert alert-success text-center">
			Current Branch: {{branch}}
		</div>
		<table class="table table-hover table-bordered">
			<thead>
				<tr class="info">
					<th>Date</th>
					<th>Author</th>
					<th>Commit</th>
					<th>Notes</th>
				</tr>
			</thead>
			<tbody>
				<tr ng-repeat="c in commits">
					<td>{{c.DATE}}</td>
					<td>{{c.AUTHOR}}</td>
					<td>{{c.COMMIT}}</td>
					<td>{{c.NOTES}}</td>
				</tr>
			</tbody>
		</table>
	</div>
</asp:Content>
