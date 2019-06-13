<%@ Page Title="Builder" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="merger.aspx.cs" Inherits="merger" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/merger_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/merger_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="row">
			<div class="col-md-3">
				<div class="alert alert-info mt-2" style="text-align: center" ng-hide="lockid == null">
					<a data-toggle="tooltip" title="Click to see full plan for the person" target="_blank" href="editplan.aspx?userid={{lockid}}">
						<img class="rounded-circle" ng-src="{{'getUserImg.ashx?sz=60&id=' + lockid}}" alt="Smile" height="60" width="60" />
						<div>
							<strong>Locked by: {{lockid | getUserById:this}}</strong>
						</div>
					</a>
					<i class="fas fa-unlock-alt"></i>
				</div>
			</div>
			<div class="col-md-6">
				<div class="alert alert-success">
					<i class="fas fa-object-group" style="font-size:150%"></i>&nbsp;&nbsp;Merging branch: <strong>{{branch}}</strong>
				</div>
				<div class="alert alert-success">
					<strong>Attention!</strong> Do not use/click/see this page if you are not aware what it does!
				</div>
				<button ng-click="start()" ng-disabled="readonly || progress()" type="button" class="btn btn-block btn-success">Get git.<img ng-show="progress()" height="20" width="20" src="images/process.gif" /></button>
				<pre ng-show="gitStatus.length > 0"><code ng-bind-html="gitStatus | rawHtml"></code></pre>
				<button ng-show="gitStatus.length > 0" ng-click="rebase()" ng-disabled="readonly || progress()" type="button" class="btn btn-block btn-success">Rebase Branch On Master.<img ng-show="progress()" height="20" width="20" src="images/process.gif" /></button>
				<pre ng-show="rebaseStatus.length > 0"><code ng-bind-html="rebaseStatus | rawHtml"></code></pre>
				<button ng-show="rebaseStatus.length > 0" ng-click="push2Master()" ng-disabled="readonly || progress()" type="button" class="btn btn-block btn-success">Push To Origin.<img ng-show="progress()" height="20" width="20" src="images/process.gif" /></button>
				<div ng-show="pushStatus.length > 0" class="well well-sm" ng-bind-html="pushStatus | rawHtml"></div>
			</div>
			<div class="col-md-3"></div>
		</div>
	</div>
</asp:Content>
