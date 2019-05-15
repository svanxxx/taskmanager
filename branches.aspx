<%@ Page Title="Branches" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="branches.aspx.cs" Inherits="Branches" %>

<%@ Register Src="~/controls/PagerControl.ascx" TagName="pagercontrol" TagPrefix="uc" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/branches_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/branches_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="card" ng-cloak>
			<div class="card-header" style="text-align: center">
				<div class="alert alert-info mb-0">
					<a class="alert-link" href ng-click="filterby('')"><i class="fas fa-code-branch"></i>Branches</a>
				</div>
			</div>
			<div class="card-body">
				<div class="list-group">
					<div class="list-group-item list-group-item-action" ng-repeat="b in state.branches" style="background-color: {{b.COLOR}}">
						<div class="row">
							<div class="col-sm-3">
								<div ng-show="b.TTID < 0">
									<i class="fas fa-tag"></i><span class="badge badge-light">{{b.NAME}}</span>
								</div>
								<div ng-show="b.TTID > 0">
									<a href="showtask.aspx?ttid={{b.TTID}}" target="_blank"><span class="badge badge-pill badge-secondary ng-binding">{{b.TTID}}</span></a>{{b.NAME.replace('TT' + b.TTID, '')}}
								</div>
							</div>
							<div class="col-sm-3">
								<i class="far fa-clock"></i>{{b.DATE}}
							</div>
							<div class="col-sm-3">
								<a href="commits.aspx?branch={{b.NAME}}">Commits...</a>
							</div>
							<div class="col-sm-3">
								<img class="rep-img rounded-circle" ng-src="{{'getUserImg.ashx?sz=20&eml=' + b.AUTHOREML}}" alt="Smile" height="20" width="20">
								<a ng-click="filterby(b.AUTHOR)" href>{{b.AUTHOR}}</a>
							</div>
						</div>
					</div>
				</div>
			</div>
		</div>
		<uc:pagercontrol runat="server" />
	</div>
</asp:Content>
