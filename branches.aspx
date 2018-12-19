<%@ Page Title="Branches" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="branches.aspx.cs" Inherits="Branches" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/branches_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/branches_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="panel panel-info" ng-cloak>
			<div class="panel-heading" style="text-align: center"><a href ng-click="filterby('')">Branches</a></div>
			<div class="panel-body">
				<div class="list-group">
					<div class="item-branch list-group-item" ng-repeat="b in state.branches" style="background-color: {{b.COLOR}}">
						<div class="row">
							<div class="col-sm-3">
								<div ng-show="b.TTID < 0">
									<span class="glyphicon glyphicon-tag"></span><span class="label label-warning">{{b.NAME}}</span>
								</div>
								<div ng-show="b.TTID > 0">
									<span class="glyphicon glyphicon-user"></span>
									<a href="showtask.aspx?ttid={{b.TTID}}" target="_blank"><span class="badge ng-binding">{{b.TTID}}</span></a>
								</div>
							</div>
							<div class="col-sm-3">
								<span class="glyphicon glyphicon-time"></span>
								<td><a href="commits.aspx?branch={{b.NAME}}">{{b.DATE}}</a></td>
							</div>
							<div class="col-sm-3">
								<img class="rep-img img-circle" ng-src="{{'getUserImg.ashx?eml=' + b.AUTHOREML}}" alt="Smile" height="20" width="20">
								<td><a ng-click="filterby(b.AUTHOR)" href>{{b.AUTHOR}}</a></td>
							</div>
							<div class="col-sm-3">
								<span class="glyphicon glyphicon-envelope"></span>
								<td><a href="mailto:{{b.AUTHOREML}}">{{b.AUTHOREML}}</a></td>
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
					<span>{{state.page}}</span>
					<li><a style="cursor: pointer" ng-click="incPage()">Next</a></li>
				</ul>
			</div>
			<div class="col-sm-1">
				show by:
				<select class="form-control" ng-change="changeShowBy()" ng-model="state.showby" ng-options="x for x in showbys" />
			</div>
		</div>
	</div>
</asp:Content>
