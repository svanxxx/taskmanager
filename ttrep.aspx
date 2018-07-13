<%@ Page Title="Tasks" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="ttrep.aspx.cs" Inherits="TTRep" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/ttrep_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/ttrep_js")%>
	<script src="http://mps.resnet.com/cdn/angular/angular.min.js"></script>
	<script src="http://mps.resnet.com/cdn/colResizable/colResizable-1.5.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="alert alert-danger savebutton btn-group-vertical" ng-cloak ng-show="changed">
			<button type="button" class="btn btn-lg btn-info" ng-click="applyfilter()">Apply Filter</button>
			<button type="button" class="btn btn-lg btn-danger" ng-click="discardfilter()">Discard</button>
		</div>
		<label for="searchtxt">Enter phrase in double quotes or a set of words to be found:</label>
		<input class="form-control" id="searchtxt" type="text" ng-model="DefectsFilter.text" ng-change="changed=true" ng-keypress="onGo($event)" autofocus />
		<table class="table table-bordered">
			<thead>
				<tr>
					<th>
						TT ID
					</th>
					<th>Time</th>
					<th>
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
					<th>Reference</th>
					<th>Summary</th>
					<th>
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
					<th>
						<div class="dropdown middlecol">
							<a href="#" data-toggle="dropdown" class="dropdown-toggle">Component<b class="caret"></b></a>
							<ul class="dropdown-menu">
								<li>
									<div ng-repeat="d in comps" class="checkbox">
										<label>
											<input ng-click="changeReferenceFilter(d.ID, 'components')" type="checkbox" ng-checked="{{referenceFiltered(d.ID, 'components')}}">{{d.DESCR}}
										</label>
									</div>
								</li>
							</ul>
						</div>
					</th>
					<th>
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
				<tr ng-repeat="d in defects" ng-style="d.DISPO | getDispoColorById:this">
					<td><a href="showtask.aspx?ttid={{d.ID}}">{{d.ID}}</a></td>
					<td>{{d.ESTIM}}</td>
					<td>{{d.AUSER | getUserById:this}}</td>
					<td>{{d.REFERENCE}}</td>
					<td>{{d.SUMMARY}}</td>
					<td>{{d.DISPO | getDispoById:this}}</td>
					<td>{{d.COMP | getCompById:this}}</td>
					<td>{{d.CREATEDBY | getUserById:this}}</td>
				</tr>
			</tbody>
		</table>
	</div>
</asp:Content>