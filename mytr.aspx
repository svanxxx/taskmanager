<%@ Page Title="Tasks Report" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="mytr.aspx.cs" Inherits="MyTR" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/mytr_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/mytr_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
	<script src="scripts/jquery.signalR-2.3.0.min.js"></script>
	<script src="signalr/hubs"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-cloak id="controllerholder" ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="row">
			<div class="col-lg-2 hidden-sm hidden-xs text-center">
				<div class="btn-group-vertical">
					<button type="button" class="btn btn-danger" ng-cloak ng-show="haveBirthday" ng-click="congratulate()">
						<img class="rounded-circle" ng-src="{{'getUserImg.ashx?id=' + birthdayID}}" alt="Smile" height="60" width="60">
						<span>&hArr;&nbsp;{{birthdayYears}}*</span>
						<img src="images/cake.jpg" alt="Smile" height="60" width="60">
						<span>=</span>
						<span class="glyphicon glyphicon-thumbs-up"></span>
					</button>
					<a id="mytasks" href="#" class="btn btn-outline-secondary" role="button" target="_blank"><i class="fas fa-compress-arrows-alt"></i>&nbsp;Tasks assigned to me</a>
					<a id="metasks" href="#" class="btn btn-outline-secondary" role="button" target="_blank"><i class="fas fa-expand-arrows-alt"></i>&nbsp;Tasks created by me</a>
					<a href="<%=Settings.CurrentSettings.BSTSITE.ToString()%>?showall=1&PROGABB=<%=CurrentContext.UserLogin()%>" class="btn btn-outline-secondary" role="button" target="_blank"><i class="fa fa-link"></i>&nbsp;My BST requests</a>
					<a href="dailysearch.aspx" class="btn btn-outline-secondary" role="button" target="_blank"><i class="fas fa-sort-numeric-down"></i>&nbsp;My Records</a>
					<button type="button" class="btn btn-outline-secondary" ng-click="addTask()"><i class="fas fa-wrench"></i>&nbsp;Start New Task</button>
					<button type="button" class="btn btn-outline-secondary" ng-click="planTask()"><i class="far fa-calendar-check"></i>&nbsp;Plan New Task</button>
				</div>
			</div>
			<div class="col-lg-8">
				<div class="card person-box person-bar">
					<div class="card-header vertical-align">
						<div class="row person-items">
							<div class="col-lg-2 col-xs-6 col-centered">
								<div class="card person-box h-100">
									<img class="rounded-circle" ng-src="{{'getUserImg.ashx?id=' + user.ID}}" alt="smile" width="80" height="80" style="margin: auto;"/>
									<div class="card-body person-lab">
										<label>{{user.PERSON_NAME}}</label>
									</div>
								</div>
							</div>
							<div class="col-lg-2 col-xs-6">
								<button ng-disabled="loaded()" ng-click="addRec()" type="button" class="btn btn-outline-success btn-block">Add</button>
								<button ng-disabled="!loaded()" ng-click="deleteRec()" type="button" class="btn btn-outline-danger btn-block">Delete</button>
								<input required ng-model="date" ng-change="findRec()" class="form-control date-input" type="date" />
							</div>
							<div class="col-lg-3 hidden-xs datelabel">
								<h4>{{datestring}}</h4>
								<h5>{{timedone}}</h5>
								<div class="progress person-box">
									<div class="progress-bar progress-bar-striped {{percentdonestyle}} active progress-bar-animated" role="progressbar" aria-valuenow="{{percentdone}}" aria-valuemin="0" aria-valuemax="100" style="width: {{percentdone}}%">
										{{percentdone}}
									</div>
								</div>
								<h6><i>{{status}}</i></h6>
							</div>
							<div class="col-lg-3 col-xs-6 panel person-box time-box">
								<div class="row row-timeline">
									<div class="col-sm-3">
										<label for="timein">In:</label>
									</div>
									<div class="col-sm-7">
										<input ng-disabled="!loaded()" id="timein" required type="time" class="input-sm form-control" ng-model="trrec.IN" />
									</div>
								</div>
								<div class="row row-timeline">
									<div class="col-sm-3">
										<button class="btn btn-outline-secondary" type="button" ng-click="out()">Out:</button>
									</div>
									<div class="col-sm-7">
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
									<div class="col-sm-7">
										<input ng-disabled="!loaded()" id="timebr" required type="time" class="input-sm form-control" ng-model="trrec.BREAK" />
									</div>
								</div>
							</div>
							<div class="col-lg-2 col-xs-6">
								<input ng-model="copylastday" ng-change="oncopylastday()" type="checkbox" value="" data-toggle="tooltip" title="When you add a record - previous day details will be automatically copied.">Copy last day
					<button ng-click="todayRec()" ng-disabled="isTodayRecord()" type="button" class="btn {{isTodayRecord() ? '' : 'btn-primary'}} btn-lg btn-block" data-toggle="tooltip" title="Click this button to create new daily record for today or show the data from already created recrod.">Today!</button>
							</div>
						</div>
					</div>
				</div>
				<div class="card-body dailyreport">
					<textarea ng-disabled="!loaded()" ng-model="trrec.DONE" class="form-control" rows="10" autofocus></textarea>
				</div>
				<ul class="nav nav-pills" role="tablist">
					<li class="nav-item">
						<a class="nav-link active" data-toggle="tab" href="#plan">Plan&nbsp;<span class="badge badge-light">{{defects.length}}</span></a>
					</li>
					<li class="nav-item">
						<a class="nav-link" data-toggle="tab" href="#unscheduled">Unscheduled&nbsp;<span class="badge badge-light">{{unscheduled.length}}</span></a>
					</li>
					<li class="nav-item">
						<a class="nav-link" data-toggle="tab" href="#activity">Tasks Activity</a>
					</li>
				</ul>
				<div class="tab-content">
					<div id="plan" class="tab-pane active">
						<table style="width: 100%">
							<tr class="task" ng-repeat="d in defects" ng-style="{{d.DISPO | getDispoColorById:this}}">
								<td><a href="showtask.aspx?ttid={{d.ID}}" target="_blank"><span class="badge badge-pill badge-secondary">{{d.ID}}</span></a></td>
								<td><span class="badge badge-danger">{{d.ESTIM}}</span></td>
								<td><span data-toggle="tooltip" title="{{d.SUMMARY}}">{{d.SUMMARY | limitTo:135}}</span></td>
								<td>
									<img class="rounded-circle" height="20" width="20" class="btn-workme" ng-src="{{'getUserImg.ashx?id=' + d.SMODTRID}}" title="{{d.SMODIFIER}}" /></td>
								<td>
									<button ng-click="workTask(d)" data-toggle="tooltip" title="Start work on this task now!" type="button" class="btn btn-xs btn-workme"><i class="fas fa-arrow-alt-circle-up"></i></button>
								</td>
								<td>
									<div class="dropdown btn-workme">
										<button type="button" class="btn dropdown-toggle btn-xs" data-toggle="dropdown"></button>
										<div class="dropdown-menu">
											<a class="dropdown-item" ng-repeat="disp in dispos" ng-click="changeDispo(d, disp)" style="background-color: {{disp.COLOR}}" href>{{disp.DESCR}}</a>
										</div>
									</div>
								</td>
							</tr>
						</table>
					</div>
					<div id="unscheduled" class="tab-pane fade">
						<table style="width: 100%">
							<tr class="task" ng-repeat="d in unscheduled" ng-style="{{d.DISPO | getDispoColorById:this}}">
								<td><a href="showtask.aspx?ttid={{d.ID}}" target="_blank"><span class="badge badge-pill badge-secondary">{{d.ID}}</span></a></td>
								<td><span class="badge badge-danger">{{d.ESTIM}}</span></td>
								<td><span data-toggle="tooltip" title="{{d.SUMMARY}}">{{d.SUMMARY | limitTo:135}}</span></td>
								<td>
									<button ng-click="workTaskUns(d)" data-toggle="tooltip" title="Start work on this task now!" type="button" class="btn btn-xs btn-workme"><i class="fas fa-arrow-alt-circle-up"></i></button>
								</td>
							</tr>
						</table>
					</div>
					<div id="activity" class="tab-pane fade">
						<strong>Created tasks:</strong><a ng-repeat="t in trrec.CREATEDTASKS" href="showtask.aspx?ttid={{t}}" target="_blank"><span class="badge badge-pill badge-secondary">{{t}}</span></a>
						<br />
						<strong>Scheduled tasks:</strong><a ng-repeat="t in trrec.SCHEDULEDTASKS" href="showtask.aspx?ttid={{t}}" target="_blank"><span class="badge badge-pill badge-secondary">{{t}}</span></a>
						<br />
						<strong>Modified tasks:</strong><a ng-repeat="t in trrec.MODIFIEDTASKS" href="showtask.aspx?ttid={{t}}" target="_blank"><span class="badge badge-pill badge-secondary">{{t}}</span></a>
					</div>
				</div>
			</div>
			<div class="text-center col-lg-2 hidden-sm hidden-xs">
				<div class="btn-group-vertical">
					<a style="margin-bottom: 1px; background-color: {{u.STATUS == 1 ? '#0000ff3d' : '#ff000096'}}" href="editplan.aspx?userid={{u.ID}}" class="btn btn-info" role="button" target="_blank" ng-repeat="u in mpsusers">
						<img class="rounded-circle" style="float: left" ng-src="getUserImg.ashx?id={{u.ID}}" alt="Smile" height="20" width="20">
						{{u.LOGIN}}
					</a>
				</div>
			</div>
		</div>
	</div>
</asp:Content>
