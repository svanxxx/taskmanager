<%@ Page Title="Tracker" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="tracker.aspx.cs" Inherits="TrackerPage" %>

<%@ Register Src="~/controls/DefectSpentControl.ascx" TagName="defSpent" TagPrefix="uc" %>
<%@ Register Src="~/controls/DefectNumControl.ascx" TagName="defNum" TagPrefix="uc" %>
<%@ Register Src="~/controls/DefectEstControl.ascx" TagName="defEst" TagPrefix="uc" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/tracker_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/tracker_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
	<script <%="src='" + Settings.CurrentSettings.CHARTSJSCDN.ToString() + "Chart.bundle.min.js'" %>></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<input type="hidden" id="trackers" value='<%=Newtonsoft.Json.JsonConvert.SerializeObject(Tracker.Enum(CurrentContext.TTUSERID))%>' />
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<h2 class="text-center">Task Tracker</h2>
		<div class="row">
			<div class="col-3">
				<canvas id="chartpie" width="1400" height="1400"></canvas>
			</div>
			<div class="col-6">
				<div class="list-group">
					<div ng-repeat="d in defects" class="list-group-item p-1" ng-style="{{d.DISPO | getDispoColorById:this}}">
						<uc:defNum runat="server" />
						<uc:defEst runat="server" />
						<span data-toggle="tooltip" title="{{d.SUMMARY}}" ng-bind-html="d.SUMMARY | sumFormat | limitTo:135"></span>
						<span class="badge badge-pill badge-light float-right">{{d.VERSION}}</span>
					</div>
				</div>
			</div>
			<div class="col-3">
				<div class="btn-group" ng-show="isadmin">
					<button type="button" class="btn btn-outline-secondary dropdown-toggle" data-toggle="dropdown">
						Add new track list
					</button>
					<div class="dropdown-menu">
						<a class="dropdown-item" href ng-repeat="f in filters" ng-click="addTracker(f.ID, f.NAME)" style="cursor: pointer">{{f.NAME}}</a>
					</div>
				</div>
				<div class="list-group">
					<div ng-repeat="t in trackers" class="d-flex">
						<a href="?id={{t.ID}}" class="list-group-item list-group-item-action">{{t.NAME}}</a>
						<div class="btn-group" ng-show="isadmin" >
							<button type="button" class="btn btn-sm btn-outline-secondary dropdown-toggle" data-toggle="dropdown">
								<img class="rounded-circle" ng-src="{{'getUserImg.ashx?sz=25&ttid=' + t.IDCLIENT}}" alt="Smile" height="25" width="25" />
							</button>
							<div class="dropdown-menu dropdown-menu-right">
								<div class="dropdown-item" ng-repeat="u in users | filter : {ACTIVE: true}" style="cursor:pointer" ng-click="assignToClient(t.ID, u.ID)">
									<img class="rounded-circle" ng-src="{{'getUserImg.ashx?sz=20&ttid=' + u.ID}}" alt="Smile" height="20" width="20" />
									{{u.FULLNAME}}
								</div>
							</div>
						</div>
						<button ng-show="isadmin" type="button" class="close" ng-click="delTracker(t.ID)">&times;</button>
					</div>
				</div>
			</div>
		</div>
	</div>
</asp:Content>
