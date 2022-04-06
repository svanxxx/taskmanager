<%@ Page Title="Tracker" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="tracker.aspx.cs" Inherits="TrackerPage" %>

<%@ Register Src="~/controls/DefectSpentControl.ascx" TagName="defSpent" TagPrefix="uc" %>
<%@ Register Src="~/controls/DefectNumControl.ascx" TagName="defNum" TagPrefix="uc" %>
<%@ Register Src="~/controls/DefectEstControl.ascx" TagName="defEst" TagPrefix="uc" %>
<%@ Register Src="~/controls/DefectDBControl.ascx" TagName="defDB" TagPrefix="uc" %>
<%@ Register Src="~/controls/DefectVerControl.ascx" TagName="defVer" TagPrefix="uc" %>
<%@ Register Src="~/controls/DefectEddControl.ascx" TagName="defEdd" TagPrefix="uc" %>
<%@ Register Src="~/controls/DefectUsrControl.ascx" TagName="defUsr" TagPrefix="uc" %>
<%@ Register Src="~/controls/DefectOrdControl.ascx" TagName="defOrd" TagPrefix="uc" %>
<%@ Register Src="~/controls/SelectUser.ascx" TagName="usrlist" TagPrefix="uc" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/tracker_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/tracker_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
	<script <%="src='" + Settings.CurrentSettings.CHARTSJSCDN.ToString() + "Chart.bundle.min.js'" %>></script>
	<script src="scripts/userimg.js"></script>
	<link rel="manifest" href="manifest.json">
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<input type="hidden" id="trackers" value='<%=Newtonsoft.Json.JsonConvert.SerializeObject(Tracker.Enum(CurrentContext.TTUSERID))%>' />
	<div ng-app="mpsapplication" ng-controller="mpscontroller" ng-cloak>
		<div class="dropdown-menu dropdown-menu-sm" id="context-menu">
			<a class="dropdown-item" href="#">Fire</a>
			<a class="dropdown-item" href="#">Close</a>
		</div>
		<uc:usrlist runat="server" />
		<div class="row">
			<div class="btn-group mx-auto">
				<button type="button" class="btn btn-outline-light text-dark" data-toggle="dropdown">
					<h2><i class="fas fa-file-contract"></i>&nbsp;{{pageName}}</h2>
				</button>
				<div class="dropdown-menu">
					<a ng-repeat="t in trackers" href="?id={{t.ID}}" class="dropdown-item">{{t.NAME}}</a>
				</div>
			</div>
			<div>
				<div class="dropleft">
					<button id="sortbtn" type="button" class="btn {{sort == 0 ? 'btn-outline-secondary' : 'btn-primary'}} dropdown-toggle" data-toggle="dropdown" data-toggle-second="tooltip" title="Click to see sort options">
						<i class="fas fa-sliders-h"></i>
					</button>
					<div class="dropdown-menu border-0">
						<div class="btn-group-vertical">
							<button type="button" class="btn {{sort == 0 ? 'btn-primary' : 'btn-outline-secondary'}}" ng-click="sort=0">Unsorted</button>
							<button type="button" class="btn {{sort == 1 ? 'btn-primary' : 'btn-outline-secondary'}}" ng-click="sort=1">Sort By Date</button>
							<button type="button" class="btn {{sort == 2 ? 'btn-primary' : 'btn-outline-secondary'}}" ng-click="sort=2">Sort By Version</button>
						</div>
					</div>
				</div>
			</div>
		</div>
		<div class="row">
			<div class="col-md-3">
				<div class="shadow mb-2 mt-2">
					<canvas id="chartpie" width="1400" height="1400"></canvas>
				</div>
				<div class="shadow mt-4">
					<table class="table table-sm">
						<thead class="thead-light">
							<tr>
								<th>Weekly Activity</th>
								<th>Value</th>
							</tr>
						</thead>
						<tbody>
							<tr>
								<td>Delivered (Count)</td>
								<td>{{STATS.FINISHED}}</td>
							</tr>
							<tr>
								<td>Worked (hrs)</td>
								<td>{{STATS.SPENTHOURS}}</td>
							</tr>
							<tr>
								<td>Added (count)</td>
								<td>{{STATS.CREATED}}</td>
							</tr>
							<tr>
								<td>Added (hrs)</td>
								<td>{{STATS.CREATEDHOURS}}</td>
							</tr>
							<tr>
								<td>Projected End Date</td>
								<td>{{STATS.EDD}}</td>
							</tr>
							<tr>
								<td>Total Tasks (Count)</td>
								<td>{{STATS.TOTAL}}</td>
							</tr>
							<tr>
								<td>Total Tasks (hrs)</td>
								<td>{{STATS.TOTALHOURS}}</td>
							</tr>
							<tr>
								<td>Remaining Total Tasks (hrs)</td>
								<td>{{STATS.REMAINHOURS}}</td>
							</tr>
						</tbody>
					</table>
				</div>
			</div>
			<div class="col-md-6 mb-2">
				<input ng-model="newtask" type="text" class="form-control form-control-sm" onkeydown="return event.key != 'Enter';" ng-keyup="messageKey($event)" ng-show="isadmin && simpleTracker">
				<div class="list-group shadow">
					<div ng-repeat="d in defects | defectsTrackFilter:sort" class="list-group-item p-1" ng-style="{{d.DISPO | getDispoColorById:this}}">
						<uc:defNum runat="server" />
						<uc:defEst runat="server" />
						<uc:defDB runat="server" />
						<span class="taskrect" data-toggle="tooltip" title="{{d.SUMMARY}}" ng-bind-html="d.SUMMARY | sumFormat | limitTo:135"></span>
						<uc:defOrd runat="server" class="float-right" ng-show="isadmin" />
						<uc:defUsr onchange="console.log('ddd')" ng-show="isadmin" runat="server" class="float-right" />
						<uc:defVer runat="server" />
						<uc:defEdd runat="server" />
					</div>
				</div>
			</div>
			<div class="col-md-3">
				<div class="card shadow mb-2">
					<div class="card-body text-center">
						<img class="rounded-circle" ng-src="{{'getUserImg.ashx?sz=150&ttid=' + trackUserID()}}" alt="Smile" height="150" width="150" />
						<h4>{{trackUserID() | getUserById:this}}</h4>
					</div>
				</div>
				<div class="shadow">
					<div class="list-group mb-2">
						<div ng-repeat="t in trackers" class="d-flex mb-2">
							<svg width="10" height="3em">
								<rect ng-repeat="c in t.Completes" ng-attr-y="{{c.Y+'%'}}" width="100%" ng-attr-height="{{c.PERCENT+'%'}}" style="fill: {{c.COLOR}}; stroke-width: 1; stroke: rgb(0,0,0)" />
							</svg>
							<a href="?id={{t.ID}}" class="list-group-item list-group-item-action shadow">{{t.NAME}}</a>
							<div class="btn-group" ng-show="isadmin">
								<button type="button" class="btn btn-sm btn-outline-secondary dropdown-toggle" data-toggle="dropdown">
									<img data-toggle="tooltip" title="<img src='{{'getUserImg.ashx?sz=150&ttid=' + t.IDCLIENT}}' />" class="rounded-circle" ng-src="{{'getUserImg.ashx?sz=25&ttid=' + t.IDCLIENT}}" alt="Smile" height="25" width="25" />
								</button>
								<div class="dropdown-menu dropdown-menu-right">
									<div class="dropdown-item" ng-repeat="u in users | filter : {ACTIVE: true}" style="cursor: pointer" ng-click="assignToClient(t.ID, u.ID)">
										<img class="rounded-circle" ng-src="{{'getUserImg.ashx?sz=20&ttid=' + u.ID}}" alt="Smile" height="20" width="20" />
										{{u.FULLNAME}}
									</div>
								</div>
							</div>
							<button ng-show="isadmin" type="button" class="close" ng-click="delTracker(t.ID)">&times;</button>
						</div>
						<div class="d-flex">
							<div class="btn-group flex-fill" ng-show="isadmin">
								<button type="button" class="btn btn-outline-secondary dropdown-toggle" data-toggle="dropdown">
									Add new track list
								</button>
								<div class="dropdown-menu">
									<a class="dropdown-item" href ng-repeat="f in filters" ng-click="addTracker(f.ID, f.NAME)" style="cursor: pointer">
										<i class="fas fa-user-friends text-danger" ng-show="f.SHARED"></i>
										<i class="fas fa-user" ng-hide="f.SHARED"></i>
										{{f.NAME}}
									</a>
								</div>
							</div>
							<button ng-show="isadmin" type="button" class="btn btn-outline-secondary flex-fill" ng-click="addTracker()">Simple Tracker</button>
						</div>
					</div>
				</div>
			</div>
		</div>
	</div>
</asp:Content>
