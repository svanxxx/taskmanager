<%@ Page Title="Log" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="log.aspx.cs" Inherits="log" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/log_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/log_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<button type="button" class="btn btn-light btn-sm" ng-click="dec()">Prev</button>
		<span class="badge badge-light">{{page}}</span>
		<button type="button" class="btn btn-light btn-sm" ng-click="inc()">Next</button>
		<button type="button" class="btn btn-danger btn-sm" ng-click="clear()">clear</button>
		<div class="form-group">
			<textarea class="form-control" rows="30" ng-model="text" ng-disabled="true"></textarea>
		</div>
	</div>
</asp:Content>
