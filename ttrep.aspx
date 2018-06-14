<%@ Page Title="Task" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="ttrep.aspx.cs" Inherits="TTRep" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<script src="scripts/references.js"></script>
	<script src="scripts/ttrep.js"></script>
	<script src="http://mps.resnet.com/cdn/angular/angular.min.js"></script>
	<link href="css/ttrep.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="alert alert-danger savebutton btn-group-vertical" ng-cloak ng-show="changed">
			<button type="button" class="btn btn-lg btn-info" ng-click="applyfilter()">Apply Filter</button>
			<button type="button" class="btn btn-lg btn-danger">Discard</button>
		</div>
		<table class="table table-bordered">
			<thead>
				<tr>
					<th nowrap>TT ID</th>
					<th nowrap>Time</th>
					<th nowrap>
						<div class="dropdown middlecol">
							<a href="#" data-toggle="dropdown" class="dropdown-toggle">User<b class="caret"></b></a>
							<ul class="dropdown-menu">
								<li class="usersshortlist">
									<div ng-repeat="u in users" class="checkbox">
										<label>
											<input ng-click="changeReferenceFilter(u.ID, 'users')" type="checkbox" ng-checked="{{referenceFiltered(u.ID, 'users')}}">{{u.FIRSTNAME + ' ' + u.LASTNAME}}
										</label>
									</div>
								</li>
							</ul>
						</div>
					</th>
					<th nowrap>Reference</th>
					<th nowrap>Summary</th>
					<th nowrap>
						<div class="dropdown middlecol">
							<a href="#" data-toggle="dropdown" class="dropdown-toggle">Disposition<b class="caret"></b></a>
							<ul class="dropdown-menu">
								<li>
									<div ng-repeat="d in dispos" class="checkbox">
										<label>
											<input ng-click="changeReferenceFilter(d.ID, 'dispositions')" type="checkbox" ng-checked="{{referenceFiltered(d.ID, 'dispositions')}}">{{d.DESCR}}
										</label>
									</div>
								</li>
							</ul>
						</div>
					</th>
					<th nowrap>
						<div class="dropdown middlecol">
							<a href="#" data-toggle="dropdown" class="dropdown-toggle">Created<b class="caret"></b></a>
							<ul class="dropdown-menu">
								<li class="usersshortlist">
									<div ng-repeat="u in users" class="checkbox">
										<label>
											<input type="checkbox">{{u.FIRSTNAME + ' ' + u.LASTNAME}}
										</label>
									</div>
								</li>
							</ul>
						</div>
					</th>
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
