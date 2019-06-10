<%@ Page Title="Branches" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="branches.aspx.cs" Inherits="Branches" %>

<%@ Register Src="~/controls/BranchesControl.ascx" TagName="branches" TagPrefix="uc" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/branches_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/branches_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="container">
			<uc:branches runat="server" />
		</div>
	</div>
</asp:Content>
