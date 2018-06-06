<%@ Page Title="Tasks Report" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="mytr.aspx.cs" Inherits="MyTR" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<script src="scripts/references.js"></script>
	<script src="scripts/mytr.js"></script>
	<script src="http://mps.resnet.com/cdn/angular/angular.min.js"></script>
	<link href="css/mytr.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="panel panel-info person-box">
			<div class="panel-heading vertical-align">
				<div class="col-sm-2 col-centered">
					<div class="panel panel-primary person-box">
						<div class="panel-heading person-img">
							<img ng-src="{{userimg}}" alt="smile" width="80" height="80" />
						</div>
						<div class="panel-body person-lab">
							<label>{{user.PERSON_NAME}}</label>
						</div>
					</div>
				</div>
				<div class="col-sm-2">
					<button ng-disabled="loaded()" ng-click="addRec()" type="button" class="btn btn-xs btn-success btn-block">Add</button>
					<button ng-disabled="!loaded()" ng-click="deleteRec()" type="button" class="btn btn-xs btn-danger btn-block">Delete</button>
					<input required ng-model="date" ng-change="findRec()" class="form-control date-input" type="date" />
					<div class="progress person-box">
						<div class="progress-bar progress-bar-striped {{percentdonestyle}} active" role="progressbar" aria-valuenow="{{percentdone}}" aria-valuemin="0" aria-valuemax="100" style="width: {{percentdone}}%">
							{{percentdone}}%
						</div>
					</div>
				</div>
				<div class="col-sm-5 datelabel">
					<h2>{{datestring}}</h2>
					<label>{{status}}</label>
				</div>
				<div class="col-sm-2 panel person-box time-box">
					<div class="row row-timeline">
						<div class="col-sm-3">
							<label for="timein">In:</label>
						</div>
						<div class="col-sm-8">
							<input ng-disabled="!loaded()" id="timein" required type="time" class="input-sm form-control" ng-model="trrec.IN" />
						</div>
					</div>
					<div class="row row-timeline">
						<div class="col-sm-3">
							<label for="timeou">Out:</label>
						</div>
						<div class="col-sm-8">
							<input ng-disabled="!loaded()" id="timeou" required type="time" class="input-sm form-control" ng-model="trrec.OUT" />
						</div>
						<div class="col-sm-2 center">
							<input ng-model="autotime" ng-change="changeAutoDate()" type="checkbox" value="" data-toggle="tooltip" title="Automatically adjust time out when the page is opened.">
						</div>
					</div>
					<div class="row row-timeline">
						<div class="col-sm-3">
							<label for="timebr" style="">Break:</label>
						</div>
						<div class="col-sm-8">
							<input ng-disabled="!loaded()" id="timebr" required type="time" class="input-sm form-control" ng-model="trrec.BREAK" />
						</div>
					</div>
				</div>
				<div class="col-sm-2">
					<input ng-model="copylastday" ng-change="oncopylastday()" type="checkbox" value="" data-toggle="tooltip" title="When you add a recrod - previous day details will be automatically copied.">Copy last day
					<button ng-click="todayRec()" ng-disabled="isTodayRecord()" type="button" class="btn {{isTodayRecord() ? 'btn-default' : 'btn-primary'}} btn-lg btn-block" data-toggle="tooltip" title="Click this button to create new daily record for today or show the data from already created recrod.">Today!</button>
				</div>
			</div>
		</div>
		<div class="panel-body dailyreport">
			<textarea ng-disabled="!loaded()" ng-model="trrec.DONE" class="form-control" rows="10" autofocus></textarea>
		</div>
		<ul class="nav nav-pills">
			<li class="small active"><a data-toggle="pill" href="#plan">Plan<span class="badge">{{defects.length}}</span></a></li>
			<li class="small"><a data-toggle="pill" href="#unscheduled">Unscheduled<span class="badge">{{unscheduled.length}}</span></a></li>
		</ul>
		<div class="tab-content panel panel-default">
			<div id="plan" class="tab-pane fade in active">
				<div ng-repeat="d in defects" ng-style="{{d.DISPO | getDispoColorById:this}}" class="task alert">
					<a href="showtask.aspx?ttid={{d.ID}}" target="_blank"><span class="badge">{{d.ID}}</span></a>{{d.SUMMARY}}
					<button ng-click="workTask(d)" data-toggle="tooltip" title="Start work on this task now!" type="button" class="btn btn-default btn-xs btn-workme"><span class="glyphicon glyphicon-circle-arrow-up"></span></button>
					<div class="dropdown btn-workme">
						<button class="btn btn-default btn-xs dropdown-toggle" type="button" data-toggle="dropdown">
							<span class="caret"></span>
						</button>
						<ul class="dropdown-menu">
							<li ng-repeat="disp in dispos" ng-click="changeDispo(d, disp)"><a href="#">{{disp.DESCR}}</a></li>
						</ul>
					</div>
				</div>
			</div>
			<div id="unscheduled" class="tab-pane fade">
				<div ng-repeat="d in unscheduled" ng-style="{{d.DISPO | getDispoColorById:this}}" class="task alert">
					<a href="showtask.aspx?ttid={{d.ID}}" target="_blank"><span class="badge">{{d.ID}}</span></a>{{d.SUMMARY}}
				</div>
			</div>
		</div>
</asp:Content>
