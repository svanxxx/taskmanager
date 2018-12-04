<%@ Page Title="Builder" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="builder.aspx.cs" Inherits="builder" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/builder_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/builder_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="row">
			<div class="col-md-3"></div>
			<div class="col-md-6">
				<div class="alert alert-success">
					<strong>Attention!</strong> Do not use/click/see this page if you are not aware what it does!
				</div>
				<button ng-click="start()" ng-disabled="readonly || progress()" type="button" class="btn btn-block btn-success">Get git.<img ng-show="progress()" height="20" width="20" src="images/process.gif" /></button>
				<div class="well well-sm" ng-show="gitStatus.length > 0" ng-bind-html="gitStatus | rawHtml"></div>
				<button ng-show="gitStatus.length > 0" ng-click="release()" ng-disabled="readonly || progress()" type="button" class="btn btn-block btn-success">Increment Version.<img ng-show="progress()" height="20" width="20" src="images/process.gif" /></button>
				<div ng-show="psStatus.length > 0" class="well well-sm" ng-bind-html="psStatus | rawHtml"></div>
				<button ng-show="psStatus.length > 0" ng-click="push2Master()" ng-disabled="readonly || progress()" type="button" class="btn btn-block btn-success">Push To mater.<img ng-show="progress()" height="20" width="20" src="images/process.gif" /></button>
				<div ng-show="pushStatus.length > 0" class="well well-sm" ng-bind-html="pushStatus | rawHtml"></div>
			</div>
			<div class="col-md-3"></div>
		</div>
	</div>
</asp:Content>
