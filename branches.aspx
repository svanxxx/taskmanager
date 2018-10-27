<%@ Page Title="Branches" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="branches.aspx.cs" Inherits="Branches" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/branches_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/branches_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="panel panel-info" ng-cloak>
			<div class="panel-heading" style="text-align: center">Branches</div>
			<div class="panel-body">
				<div class="list-group">
					<div class="list-group-item" ng-repeat="b in branches" style="background-color: {{b.COLOR}}">
						<div class="row">
							<div class="col-sm-3">
								<span ng-show="b.TTID < 0">{{b.NAME}}</span>
								<a ng-show="b.TTID > 0" href="showtask.aspx?ttid={{b.TTID}}" target="_blank"><span class="badge ng-binding">{{b.TTID}}</span></a>
							</div>
							<div class="col-sm-3">
								<td><a href="commits.aspx?branch={{b.NAME}}">{{b.DATE}}</a></td>
							</div>
							<div class="col-sm-3">
								<td><a href="commits.aspx?branch={{b.NAME}}">{{b.AUTHOR}}</a></td>
							</div>
							<div class="col-sm-3">
								<td><a href="commits.aspx?branch={{b.NAME}}">{{b.AUTHOREML}}</a></td>
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
