<%@ Page Title="Builds" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="builds.aspx.cs" Inherits="Builds" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/builds_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/builds_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
	<script src="scripts/jquery.signalR-2.3.0.min.js"></script>
	<script src="signalr/hubs"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<table class="table table-hover table-bordered">
			<thead>
				<tr class="info">
					<th>User</th>
					<th>Requested Time</th>
					<th>Notes</th>
					<th>Machine</th>
					<th>Time updated</th>
					<th>TT ID</th>
					<th>Progress</th>
					<th>Status</th>
				</tr>
			</thead>
			<tbody>
				<tr ng-repeat="b in builds" style="background-color:{{b.STATUS.includes('FAILED') ? '#ff00004d' : ''}}">
					<td><img class="rep-img img-circle" src="{{'getUserImg.ashx?ttid=' + b.TTUSERID}}" alt="Smile" height="20" width="20"></td>
					<td>{{b.DATE}}</td>
					<td>{{b.NOTES}}</td>
					<td>{{b.MACHINE}}</td>
					<td>{{b.DATEUP}}</td>
					<td>
						<a href="showtask.aspx?ttid={{b.TTID}}" target="_blank">
							<span class="badge">{{b.TTID}}</span>
						</a>
					</td>
					<td><a href="getBuildLog.ashx?id={{b.ID}}">{{b.STATUSTXT}}</a></td>
					<td>{{b.STATUS}}</td>
				</tr>
			</tbody>
		</table>
	</div>
</asp:Content>
