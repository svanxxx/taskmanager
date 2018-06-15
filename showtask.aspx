<%@ Page Title="Task" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="showtask.aspx.cs" Inherits="Showtask" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<script src="scripts/references.js"></script>
	<link href="css/showtask.css" rel="stylesheet" />
	<script src="scripts/showtask.js"></script>
	<script src="http://mps.resnet.com/cdn/angular/angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="alert alert-danger savebutton btn-group-vertical" ng-cloak ng-show="changed">
			<button type="button" class="btn btn-lg btn-info" ng-click="saveDefect()">Save</button>
			<button type="button" class="btn btn-lg btn-danger" ng-click="discardDefect()">Discard</button>
		</div>
		<label for="summary">TT{{defect.ID}} {{defect.SUMMARY}}</label>
		<input type="text" class="form-control" id="summary" ng-disabled="!canChangeDefect()" ng-model="defect.SUMMARY">
		<div class="row toolbar" style="{{getDispoColor()}}">
			<div class="col-sm-3">
				<div class="row">
					<div class="col-sm-3">
						<h6>Type:</h6>
					</div>
					<div class="col-sm-9">
						<select class="form-control input-sm" id="type" ng-disabled="!canChangeDefect()" ng-model="defect.TYPE">
							<option value="{{t.ID}}" ng-repeat="t in types">{{t.DESCR}}</option>
						</select>
					</div>
				</div>
				<div class="row">
					<div class="col-sm-3">
						<h6>Product:</h6>
					</div>
					<div class="col-sm-9">
						<select class="form-control input-sm" id="product" ng-disabled="!canChangeDefect()" ng-model="defect.PRODUCT">
							<option value="{{p.ID}}" ng-repeat="p in products">{{p.DESCR}}</option>
						</select>
					</div>
				</div>
				<div class="row">
					<div class="col-sm-3">
						<h6>Reference:</h6>
					</div>
					<div class="col-sm-9">
						<input type="text" class="form-control input-sm" id="reference" ng-disabled="!canChangeDefect()" ng-model="defect.REFERENCE">
					</div>
				</div>
			</div>
			<div class="col-sm-3">
				<div class="row">
					<div class="col-sm-3">
						<h6>Disposition:</h6>
					</div>
					<div class="col-sm-9">
						<select class="form-control input-sm" id="dispo" ng-disabled="!canChangeDefect()" ng-model="defect.DISPO">
							<option ng-repeat="d in dispos" value="{{d.ID}}" style="background-color: {{d .COLOR}}">{{d.DESCR}}</option>
						</select>
					</div>
				</div>
				<div class="row">
					<div class="col-sm-3">
						<h6>Priority:</h6>
					</div>
					<div class="col-sm-9">
						<select class="form-control input-sm" id="prio" ng-disabled="!canChangeDefect()" ng-model="defect.PRIO">
							<option ng-repeat="p in priorities" value="{{p.ID}}">{{p.DESCR}}</option>
						</select>
					</div>
				</div>
				<div class="row">
					<div class="col-sm-3">
						<h6>Component:</h6>
					</div>
					<div class="col-sm-9">
						<select class="form-control input-sm" id="comp" ng-disabled="!canChangeDefect()" ng-model="defect.COMP">
							<option ng-repeat="c in comps" value="{{c.ID}}">{{c.DESCR}}</option>
						</select>
					</div>
				</div>
			</div>
			<div class="col-sm-3">
				<div class="row">
					<div class="col-sm-3">
						<h6>Severity:</h6>
					</div>
					<div class="col-sm-9">
						<select class="form-control input-sm" id="dispo" ng-disabled="!canChangeDefect()" ng-model="defect.SEVE">
							<option ng-repeat="s in severs" value="{{s.ID}}">{{s.DESCR}}</option>
						</select>
					</div>
				</div>
				<div class="row">
					<div class="col-sm-3">
						<h6>Date:</h6>
					</div>
					<div class="col-sm-9">
						<input type="date" id="date" class="form-control input-sm" ng-disabled="!canChangeDefect()" ng-model="defect.DATE">
					</div>
				</div>
				<div class="row">
					<div class="col-sm-3">
						<h6>Created:</h6>
					</div>
					<div class="col-sm-9">
						<select class="form-control input-sm" id="created" ng-disabled="!canChangeDefect()" ng-model="defect.CREATEDBY">
							<option ng-repeat="u in users" ng-show="u.ACTIVE" value="{{u.ID}}">{{u.FIRSTNAME}} {{u.LASTNAME}}</option>
						</select>
					</div>
				</div>
			</div>
			<div class="col-sm-3">
				<div class="row">
					<div class="col-sm-3">
						<h6>Assigned:</h6>
					</div>
					<div class="col-sm-9">
						<select class="form-control input-sm" id="auser" ng-disabled="!canChangeDefect()" ng-model="defect.AUSER">
							<option ng-repeat="u in users" ng-show="u.ACTIVE" value="{{u.ID}}">{{u.FIRSTNAME}} {{u.LASTNAME}}</option>
						</select>
					</div>
				</div>
				<div class="row">
					<div class="col-sm-3">
						<h6>Estimated:</h6>
					</div>
					<div class="col-sm-9">
						<input type="number" id="estim" class="form-control input-sm" ng-disabled="!canChangeDefect()" ng-model="defect.ESTIM">
					</div>
				</div>
				<div class="row">
					<div class="col-sm-3">
						<h6>Order:</h6>
					</div>
					<div class="col-sm-9">
						<input type="number" id="order" class="form-control input-sm" ng-disabled="!canChangeDefect()" ng-model="defect.ORDER">
					</div>
				</div>
			</div>
		</div>
		<ul class="nav nav-pills">
			<li class="small active"><a data-toggle="pill" href="#specification">Specification</a></li>
			<li><a class="small" data-toggle="pill" href="#detail">Details</a></li>
			<li><a class="small" data-toggle="pill" href="#workflow">Workflow</a></li>
			<li><a class="small" data-toggle="pill" href="#history">History</a></li>
			<li><a class="small" data-toggle="pill" href="#attachments">Attachments</a></li>
			<li><a class="small" data-toggle="pill" href="#lockinfo">Lock Info</a></li>
		</ul>
		<div class="tab-content">
			<div id="specification" class="tab-pane fade in active">
				<textarea class="form-control" id="spec" rows="30" ng-disabled="!canChangeDefect()" ng-model="defect.SPECS"></textarea>
			</div>
			<div id="detail" class="tab-pane fade">
				<textarea class="form-control" id="Description" rows="30" ng-disabled="!canChangeDefect()" ng-model="defect.DESCR"></textarea>
			</div>
			<div id="workflow" class="tab-pane fade">
				<div class="list-group">
					<a href="#" class="list-group-item" ng-repeat="h in events | orderBy : 'ORDER'">
						<div class="col-sm-3">
							<img ng-src="{{h.IDUSER | getUserImgById:this}}" alt="Smile" height="20" width="20">
							<b>{{h.IDUSER | getUserById:this}}</b>
						</div>
						<div class="col-sm-2">
							{{h.DATE}}
						</div>
						<div class="col-sm-1">
							{{h.EVENT}}
						</div>
						<div class="col-sm-3">
							<b>{{h.ASSIGNUSERID | getUserById:this}}</b> <span class="badge">{{h.TIME}}</span>
						</div>
						{{h.NOTES}}	&nbsp;
					</a>
				</div>
			</div>
			<div id="history" class="tab-pane fade">
				<div class="list-group">
					<a href="#" class="list-group-item" ng-repeat="h in history">
						<div class="col-sm-2">
							<img ng-src="{{h.IDUSER | getUserImgById:this}}" alt="Smile" height="20" width="20">
							<b>{{h.IDUSER | getUserById:this}}</b>
						</div>
						<div class="col-sm-2">
							{{h.DATE}}
						</div>
						<div class="col-sm-7">
							{{h.NOTES}}
						</div>
						&nbsp;
					</a>
				</div>
			</div>
			<div id="attachments" class="tab-pane fade">
				<button type="button" ng-disabled="!canChangeDefect()" ng-click="addFile()" id="button" class="btn btn-primary btn-xs">Add File...</button>
				<ul>
					<li ng-style="a.deleted ? {'text-decoration':'line-through'} : ''" ng-repeat="a in attachs"><a target="_blank" href="getattach.aspx?idrecord={{a.ID}}">{{a.FILENAME}}</a>&nbsp<button ng-click="deleteAttach(a.ID)" type="button" class="btn btn-danger btn-xs">Delete</button></li>
				</ul>
			</div>
			<div id="lockinfo" class="tab-pane fade">
				<span class="label label-info">{{lockedby}}</span>
				<img ng-src="{{getPersonImg(lockedby)}}" alt="Smile" height="42" width="42">
			</div>
		</div>
	</div>
</asp:Content>
