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
		<div ng-cloak ng-show="defectsselected" class="panel panel-primary">
			<div class="panel-heading">Select Action</div>
			<div class="panel-body">
				<div class="row">
					<div class="col-sm-2">
						<input type="checkbox" ng-model="apply.disposition.use">
						<label>Disposition:</label>
					</div>
					<div class="col-sm-10">
						<select class="form-control input-sm" ng-model="apply.disposition.value">
							<option ng-repeat="d in dispos" value="{{d.ID}}">{{d.DESCR}}</option>
						</select>
					</div>
				</div>
				<div class="row">
					<div class="col-sm-2">
						<input type="checkbox" ng-model="apply.component.use">
						<label>Component:</label>
					</div>
					<div class="col-sm-10">
						<select class="form-control input-sm" ng-model="apply.component.value">
							<option ng-repeat="c in comps" value="{{c.ID}}">{{c.DESCR}}</option>
						</select>
					</div>
				</div>
				<div class="row">
					<div class="col-sm-2">
						<input type="checkbox" ng-model="apply.severity.use">
						<label>Severity:</label>
					</div>
					<div class="col-sm-10">
						<select class="form-control input-sm" ng-model="apply.severity.value">
							<option ng-repeat="c in severs" value="{{c.ID}}">{{c.DESCR}}</option>
						</select>
					</div>
				</div>
				<div class="row">
					<div class="col-sm-2">
						<input type="checkbox" ng-model="apply.user.use">
						<label>User:</label>
					</div>
					<div class="col-sm-10">
						<select class="form-control input-sm" ng-model="apply.user.value">
							<option ng-repeat="u in users" value="{{u.ID}}">{{u.FULLNAME}}</option>
						</select>
					</div>
				</div>
				<button type="button" class="btn btn-success" ng-click="changeDefects()">Apply</button>
			</div>
		</div>
		<label for="searchtxt">Enter phrase in double quotes or a set of words to be found:</label>
		<input class="form-control" id="searchtxt" type="text" ng-model="DefectsFilter.text" ng-change="changed=true" ng-keypress="onGo($event)" autofocus />
		<table class="table table-bordered table-colresizable">
			<thead>
				<tr>
					<th>
						<input type="button" value="&#9745" ng-click="checkall()"></th>
					<th>ID</th>
					<th>Time</th>
					<th>
						<div class="dropdown middlecol" ng-style="{{styleFiltered('users')}}">
							<a href="#" data-toggle="dropdown" class="refmenu dropdown-toggle">User<b class="caret"></b></a>
							<ul class="dropdown-menu">
								<button class="refclear" type="button" ng-click="resetReferenceFilter('users', $event)">&#9746</button>
								<input class="refselector" type="text"/>
								<li class="usersshortlist">
									<div ng-repeat="u in users" class="checkbox">
										<label>
											<input ng-click="changeReferenceFilter(u.ID, 'users')" type="checkbox" ng-checked="{{referenceFiltered(u.ID, 'users')}}">{{u.FULLNAME}}
										</label>
									</div>
								</li>
							</ul>
						</div>
					</th>
					<th>Ref</th>
					<th>Summary</th>
					<th>
						<div class="dropdown middlecol" ng-style="{{styleFiltered('dispositions')}}">
							<a href="#" data-toggle="dropdown" class="refmenu dropdown-toggle">Disposition<b class="caret"></b></a>
							<ul class="dropdown-menu">
								<button class="refclear" type="button" ng-click="resetReferenceFilter('dispositions', $event)">&#9746</button>
								<input class="refselector" type="text"/>
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
						<div class="dropdown middlecol" ng-style="{{styleFiltered('components')}}">
							<a href="#" data-toggle="dropdown" class="refmenu dropdown-toggle">Component<b class="caret"></b></a>
							<ul class="dropdown-menu">
								<button class="refclear" type="button" ng-click="resetReferenceFilter('components', $event)">&#9746</button>
								<input class="refselector" type="text"/>
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
						<div class="dropdown middlecol" ng-style="{{styleFiltered('severities')}}">
							<a href="#" data-toggle="dropdown" class="refmenu dropdown-toggle">Severity<b class="caret"></b></a>
							<ul class="dropdown-menu">
								<button class="refclear" type="button" ng-click="resetReferenceFilter('severities', $event)">&#9746</button>
								<input class="refselector" type="text"/>
								<li>
									<div ng-repeat="s in severs" class="checkbox">
										<label>
											<input ng-click="changeReferenceFilter(s.ID, 'severities')" type="checkbox" ng-checked="{{referenceFiltered(s.ID, 'severities')}}">{{s.DESCR}}
										</label>
									</div>
								</li>
							</ul>
						</div>
					</th>
					<th>
						<div class="dropdown middlecol ng-style="{{styleFiltered('createdUsers')}}">
							<a href="#" data-toggle="dropdown" class="refmenu dropdown-toggle">Created<b class="caret"></b></a>
							<ul class="dropdown-menu">
								<button class="refclear" type="button" ng-click="resetReferenceFilter('createdUsers', $event)">&#9746</button>
								<input class="refselector" type="text"/>
								<li class="usersshortlist">
									<div ng-repeat="u in users" class="checkbox">
										<label>
											<input ng-click="changeReferenceFilter(u.ID, 'createdUsers')" type="checkbox">{{u.FULLNAME}}
										</label>
									</div>
								</li>
							</ul>
						</div>
					</th>
					<th>Date Created</th>
					<th>Date Enter</th>
				</tr>
			</thead>
			<tbody>
				<tr ng-repeat="d in defects" ng-style="d.DISPO | getDispoColorById:this">
					<td><input type="checkbox" ng-model="d.checked"></td>
					<td><a href="showtask.aspx?ttid={{d.ID}}">{{d.ID}}</a></td>
					<td>{{d.ESTIM}}</td>
					<td>{{d.AUSER | getUserById:this}}</td>
					<td>{{d.REFERENCE}}</td>
					<td>{{d.SUMMARY}}</td>
					<td>{{d.DISPO | getDispoById:this}}</td>
					<td>{{d.COMP | getCompById:this}}</td>
					<td>{{d.SEVE | getSeveById:this}}</td>
					<td>{{d.CREATEDBY | getUserById:this}}</td>
					<td>{{d.CREATED}}</td>
					<td>{{d.DATE}}</td>
				</tr>
			</tbody>
		</table>
		<h5>{{'Number of defects:' + defects.length}}</h5>
	</div>
</asp:Content>
