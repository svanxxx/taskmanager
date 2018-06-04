<%@ Page Title="Task" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="ttrep.aspx.cs" Inherits="TTRep" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<script src="scripts/ttrep.js"></script>
	<script src="http://mps.resnet.com/cdn/angular/angular.min.js"></script>
	<link href="css/ttrep.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<table class="table table-bordered">
			<thead>
				<tr>
					<th nowrap>TT ID</th>
					<th nowrap>Time</th>
					<th nowrap>User</th>
					<th nowrap>Reference</th>
					<th nowrap>Summary</th>
					<th nowrap>Disposition</th>
					<th nowrap>Created</th>
				</tr>
			</thead>
			<tbody>
				<tr ng-repeat="d in defects" ng-style="{{d.DISPO | getDispoColorById:this}}">
					<td><a href="showtask.aspx?ttid={{d.ID}}">{{d.ID}}</a></td>
					<td>{{d.ESTIM}}</td>
					<td nowrap>{{d.AUSER | getUserById:this}}</td>
					<td nowrap>{{d.REFERENCE}}</td>
					<td nowrap>{{d.SUMMARY}}</td>
					<td nowrap>{{d.DISPO | getDispoById:this}}</td>
					<td nowrap>{{d.CREATEDBY | getUserById:this}}</td>
				</tr>
			</tbody>
		</table>
	</div>
</asp:Content>
