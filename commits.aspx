<%@ Page Title="Branches" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="branches.aspx.cs" Inherits="Branches" %>

<%@ Register Src="~/controls/CommitsControl.ascx" TagName="commits" TagPrefix="uc" %>
<%@ Register Src="~/controls/PagerControl.ascx" TagName="pagercontrol" TagPrefix="uc" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/commits_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/commits_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="card" ng-cloak>
			<div class="card-header" style="text-align: center">
				<div class="alert alert-info mb-0">
					<a class="alert-link" href="branches.aspx" >Current Branch: {{branch}}</a>
				</div>
			</div>
			<div class="card-body">
				<uc:commits runat="server" />
			</div>
		</div>
		<uc:pagercontrol runat="server" />
	</div>
</asp:Content>
