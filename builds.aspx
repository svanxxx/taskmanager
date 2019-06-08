<%@ Page Title="Builds" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="builds.aspx.cs" Inherits="Builds" %>

<%@ Register Src="~/controls/BuildsControl.ascx" TagName="builds" TagPrefix="uc" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/builds_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/buildshelper_js")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/builds_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
	<script src="scripts/jquery.signalR-2.3.0.min.js"></script>
	<script src="signalr/hubs"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<input type="hidden" id="buildtime" value="<%=Settings.CurrentSettings.BUILDTIME.ToString()%>" />
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<uc:builds runat="server" />
	</div>
</asp:Content>
