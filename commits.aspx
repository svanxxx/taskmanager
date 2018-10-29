<%@ Page Title="Branches" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="branches.aspx.cs" Inherits="Branches" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/commits_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/commits_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="panel panel-info" ng-cloak>
			<div class="panel-heading" style="text-align: center">Current Branch: {{branch}}</div>
			<div class="panel-body">
				<div class="list-group">
					<div class="list-group-item" ng-repeat="c in commits">
						<div class="row">
							<div class="col-sm-2">
								<span class="glyphicon glyphicon-time"></span>
								<span>{{c.DATE}}</span>
							</div>
							<div class="col-sm-2">
								<span class="glyphicon glyphicon-user"></span>
								<span>{{c.AUTHOR}}</span>
							</div>
							<div class="col-sm-5">
								<span class="	glyphicon glyphicon-comment"></span>
								<span>{{c.NOTES}}</span>
							</div>
							<div class="col-sm-3">
								<span class="glyphicon glyphicon-pushpin"></span>
								<span>{{c.COMMIT}}</span>
							</div>
						</div>
					</div>
				</div>
			</div>
		</div>
		<div class="row">
			<div class="col-sm-11">
				<ul class="pager">
					<li><a style="cursor: pointer" ng-click="decPage()">Previous</a></li>
					<span>{{page}}</span>
					<li><a style="cursor: pointer" ng-click="incPage()">Next</a></li>
				</ul>
			</div>
			<div class="col-sm-1">
				show by:
				<select class="form-control" ng-change="changeShowBy()" ng-model="showby" ng-options="x for x in showbys" />
			</div>
		</div>
	</div>
</asp:Content>
