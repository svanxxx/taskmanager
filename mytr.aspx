<%@ Page Title="Tasks Report" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="mytr.aspx.cs" Inherits="MyTR" %>

<%@ Register Src="~/controls/DefectSpentControl.ascx" TagName="defSpent" TagPrefix="uc" %>
<%@ Register Src="~/controls/DefectNumControl.ascx" TagName="defNum" TagPrefix="uc" %>
<%@ Register Src="~/controls/DefectEstControl.ascx" TagName="defEst" TagPrefix="uc" %>
<%@ Register Src="~/controls/DefectDBControl.ascx" TagName="defDB" TagPrefix="uc" %>
<%@ Register Src="~/controls/UsrControl.ascx" TagName="usr" TagPrefix="uc" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/mytr_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/mytr_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
	<script src="scripts/jquery.signalR-2.3.0.min.js"></script>
	<script src="signalr/hubs"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<input type="hidden" id="trrec" value='<%=TRRec.GetRecString()%>' />
	<div ng-cloak id="controllerholder" ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="row">
			<div class="col-lg-2 text-center">
				<div class="d-flex flex-wrap">
					<button type="button" class="btn btn-danger" ng-cloak ng-show="haveBirthday" ng-click="congratulate()">
						<img class="rounded-circle" ng-src="{{haveBirthday ? ('getUserImg.ashx?sz=60&id=' + birthdayID) : ''}}" alt="Smile" height="60" width="60">
						<span>&hArr;&nbsp;{{birthdayYears}}*</span>
						<img ng-src="{{haveBirthday ? 'images/cake.jpg' : ''}}" alt="Smile" height="60" width="60">
						<span>=</span>
						<i class="fas fa-thumbs-up"></i>
					</button>
				</div>
				<div class="toast" data-autohide="false">
					<div class="toast-header">
						<strong class="mr-auto text-primary">Reports</strong>
					</div>
					<div class="toast-body">
						<div class="btn-group-vertical">
							<a class="btn btn-sm btn-outline-secondary" id="mytasks" href="#" role="button" target="_blank"><i class="fas fa-compress-arrows-alt"></i>&nbsp;<span class="d-none d-md-inline">Tasks assigned<span></a>
							<a class="btn btn-sm btn-outline-secondary" id="metasks" href="#" role="button" target="_blank"><i class="fas fa-expand-arrows-alt"></i>&nbsp;<span class="d-none d-md-inline">Tasks created</span></a>
							<a class="btn btn-sm btn-outline-secondary" href="<%=Settings.CurrentSettings.BSTSITE.ToString()%>?showall=1&PROGABB=<%=CurrentContext.UserLogin()%>" role="button" target="_blank"><i class="fa fa-link"></i>&nbsp;<span class="d-none d-md-inline">My BST</span></a>
							<a class="btn btn-sm btn-outline-secondary" href="dailysearch.aspx" role="button" target="_blank"><i class="fas fa-sort-numeric-down"></i>&nbsp;<span class="d-none d-md-inline">My Records</span></a>
						</div>
					</div>
				</div>
				<div class="toast" data-autohide="false">
					<div class="toast-header">
						<a href="versionchanges.aspx">The Latest Version: {{versionTag.NAME}}</a>
					</div>
					<div class="toast-body">
						<button ng-click="GetFile(versionTag.NAME, 'efip')" type="button" class="btn btn-outline-light text-dark btn-sm"><i class="fas fa-cloud-download-alt"></i>&nbsp;Fieldpro</button>
						<button ng-click="GetFile(versionTag.NAME, 'cx')" type="button" class="btn btn-outline-light text-dark btn-sm"><i class="fas fa-cloud-download-alt"></i>&nbsp;Models</button>
						<button ng-click="GetFile(versionTag.NAME, 'onsite')" type="button" class="btn btn-outline-light text-dark btn-sm"><i class="fas fa-cloud-download-alt"></i>&nbsp;Onsite</button>
						<button ng-click="GetFile(versionTag.NAME, 'demo')" type="button" class="btn btn-outline-light text-dark btn-sm"><i class="fas fa-cloud-download-alt"></i>&nbsp;Demo</button>
					</div>
				</div>
			</div>
			<div class="col-lg-8">
				<div class="card person-box person-bar">
					<div class="card-header vertical-align">
						<div class="row person-items">
							<div class="col-lg-2 col-xs-6 col-centered">
								<div class="card person-box h-100">
									<img class="rounded-circle" <%= "src='" + CurrentContext.ImagesUrl() + CurrentContext.UserLogin() + "'"%> alt="smile" width="80" height="80" style="margin: auto;" />
									<div class="card-body p-0 m-0">
										<label><% =CurrentContext.UserName() %></label>
									</div>
								</div>
							</div>
							<div class="col-lg-2 col-xs-6">
								<button ng-disabled="loaded()" ng-click="addRec()" type="button" class="btn btn-outline-success btn-block btn-sm">Add</button>
								<button ng-disabled="!loaded()" ng-click="deleteRec()" type="button" class="btn btn-outline-danger btn-block btn-sm">Delete</button>
								<input required ng-model="date" ng-change="findRec()" class="form-control date-input form-control-sm" type="date" aria-label="Find record" />
							</div>
							<div class="col-lg-3 hidden-xs datelabel">
								<h4>{{datestring}}</h4>
								<h5>{{timedone}}</h5>
								<button type="button" class="btn btn-outline-light text-dark btn-block btn-sm" aria-label="Time Status">
									<div class="progress person-box">
										<div class="progress-bar progress-bar-striped {{percentdonestyle}} active progress-bar-animated" role="progressbar" aria-valuenow="{{percentdone}}" aria-valuemin="0" aria-valuemax="100" style="width: {{percentdone}}%">
											{{percentdone}}
										</div>
									</div>
								</button>
							</div>
							<div class="col-lg-3 col-xs-6 panel person-box">
								<div class="input-group input-group-sm mb-1">
									<div class="input-group-prepend w-25">
										<span class="input-group-text">In:</span>
									</div>
									<input ng-disabled="!loaded()" id="timein" required type="time" class="input-sm form-control" ng-model="trrec.IN" aria-label="Time In" />
								</div>
								<div class="input-group input-group-sm mb-1">
									<div class="input-group-prepend w-25">
										<button class="btn" type="button" ng-click="out()">Out:</button>
									</div>
									<input ng-disabled="!loaded()" id="timeou" required type="time" class="input-sm form-control" ng-model="trrec.OUT" aria-label="Time Out" />
									<div class="input-group-append" data-toggle="tooltip" title="Automatically adjust Time-Out while page is kept open.">
										<button ng-click="onChangeAutoTime()" type="button" class="btn btn-outline-secondary btn-sm" aria-label="Set Time Auto Adjust">
											<span class="fas fa-check" ng-show="autotime"></span>
											<span class="fas fa-times" ng-show="!autotime"></span>
										</button>
									</div>
								</div>
								<div class="input-group input-group-sm">
									<div class="input-group-prepend w-25">
										<span class="input-group-text">Break:</span>
									</div>
									<input ng-disabled="!loaded()" id="timebr" required type="time" class="input-sm form-control" ng-model="trrec.BREAK" aria-label="Break Time" />
								</div>
							</div>
							<div class="col-lg-2 col-xs-6">
								<button ng-click="todayRec()" ng-disabled="isTodayRecord()" type="button" class="btn btn-outline-secondary {{isTodayRecord() ? '' : 'btn-primary'}} btn-block mb-1" data-toggle="tooltip" title="Click this button to create new daily record for today or show the data from already created recrod.">Today!</button>
								<div class="input-group mb-1 input-group-sm" data-toggle="tooltip" title="When you add a record - previous day details will be automatically copied.">
									<div class="input-group-prepend">
										<span class="input-group-text">Copy Last Day</span>
									</div>
									<button ng-click="onCopyLastDay()" type="button" class="btn btn-outline-secondary btn-sm" aria-label="Use last day details">
										<span class="fas fa-check" ng-show="copylastday"></span>
										<span class="fas fa-times" ng-show="!copylastday"></span>
									</button>
								</div>
								<button type="button" class="btn btn-outline-light text-dark btn-sm btn-block"><i>{{status}}</i></button>
							</div>
						</div>
					</div>
				</div>
				<div class="card-body dailyreport">
					<div class="list-group">
						<div ng-repeat="e in trrec.TASKSEVENTS" class="list-group-item p-1" ng-style="{{e.DEFECT.DISPO | getDispoColorById:this}}">
							<div class="dropdown float-left dropright">
								<a class="dropdown-toggle" data-toggle="dropdown" href="#">
									<uc:defSpent member="e" runat="server" />
								</a>
								<div class="dropdown-menu">
									<div class="dropdown-item" ng-repeat="t in [1,2,3,4,5,6,7,8]" style="cursor: pointer" ng-click="spendEvent(e.ID, t)">{{t}}</div>
									<div class="dropdown-divider"></div>
									<div class="dropdown-item" style="cursor: pointer" ng-click="deleteEvent(e.ID)">× - delete event</div>
								</div>
							</div>
							<uc:defNum member="e.DEFECT" runat="server" />
							<uc:defEst member="e.DEFECT" runat="server" />
							<uc:defDB member="e.DEFECT" runat="server" />
							<span data-toggle="tooltip" title="{{e.DEFECT.SUMMARY}}" ng-bind-html="e.DEFECT.SUMMARY | sumFormat | limitTo:135"></span>
						</div>
					</div>
					<textarea ng-show="showTextRep" ng-disabled="!loaded()" ng-model="trrec.DONE" class="form-control" rows="10" autofocus aria-label="Details"></textarea>
				</div>
				<div class="d-flex justify-content-between">
					<ul class="nav nav-pills">
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
					<div>
						<button type="button" class="btn btn-sm btn-outline-secondary flex-fill" ng-click="addTask()"><i class="fas fa-wrench"></i>&nbsp;<span class="d-none d-md-inline">Start New Task</span></button>
						<button type="button" class="btn btn-sm btn-outline-secondary flex-fill" ng-click="planTask()"><i class="far fa-calendar-check"></i>&nbsp;<span class="d-none d-md-inline">Plan New Task</span></button>
						<button type="button" class="btn btn-sm btn-outline-secondary flex-fill" ng-click="changeShowTextRep();"><i class="fas {{showTextRep?'fa-chevron-up':'fa-chevron-down'}}"></i>&nbsp;<span class="d-none d-md-inline">{{showTextRep?'Hide text':'Show text'}}</span></button>
					</div>
				</div>

				<div class="tab-content">
					<div id="plan" class="tab-pane active">
						<table style="width: 100%">
							<tr class="task" ng-repeat="d in defects" ng-style="{{d.DISPO | getDispoColorById:this}}">
								<td>
									<uc:defNum runat="server" />
								</td>
								<td style="white-space: nowrap;">
									<uc:defEst runat="server" />
									<uc:defDB runat="server" />
								</td>
								<td><span data-toggle="tooltip" title="{{d.SUMMARY}}" ng-bind-html="d.SUMMARY | sumFormat | limitTo:135"></span></td>
								<td>
									<img class="rounded-circle" height="20" width="20" class="btn-workme" ng-src="{{'getUserImg.ashx?sz=20&id=' + d.SMODTRID}}" title="{{d.SMODIFIER}}" /></td>
								<td>
									<button ng-click="workTask(d)" data-toggle="tooltip" title="Start work on this task now!" aria-label="Start!" type="button" class="btn btn-xs btn-workme"><i class="fas fa-arrow-alt-circle-up"></i></button>
								</td>
								<td>
									<div class="dropdown btn-workme">
										<button type="button" class="btn dropdown-toggle btn-xs" data-toggle="dropdown" aria-label="Change"></button>
										<div class="dropdown-menu">
											<a class="dropdown-item" ng-repeat="disp in dispos" ng-click="changeDispo(d, disp, -1)" style="background-color: {{disp.COLOR}}" href>{{disp.DESCR}}</a>
										</div>
									</div>
								</td>
							</tr>
						</table>
					</div>
					<div id="unscheduled" class="tab-pane fade">
						<table style="width: 100%">
							<tr class="task" ng-repeat="d in unscheduled" ng-style="{{d.DISPO | getDispoColorById:this}}">
								<td>
									<uc:defNum runat="server" />
								</td>
								<td>
									<uc:defEst runat="server" />
									<uc:defDB runat="server" />
								</td>
								<td><span data-toggle="tooltip" title="{{d.SUMMARY}}" ng-bind-html="d.SUMMARY | sumFormat | limitTo:135"></span></td>
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
			<div class="text-center col-lg-2">
				<div class="toast" data-autohide="false">
					<div class="toast-header">
						<strong class="mr-auto text-primary">Team</strong>
					</div>
					<div class="toast-body">
						<div class="d-flex flex-wrap">
							<a style="margin-bottom: 1px; margin-right: 1px;" href="editplan.aspx?userid={{u.ID}}" class="btn {{roomUserClass(u.STATUS)}}  flex-fill" role="button" target="_blank" ng-repeat="u in roomUsers">
								<uc:usr size="20" runat="server" userid="u.TTID" style="float: left" />
								<span class="d-none d-md-inline">{{u.ID | getMPSUserLoginById:this}}</span>
							</a>
						</div>
					</div>
				</div>
				<div class="toast" data-autohide="false">
					<div class="toast-header">
						<strong class="mr-auto text-primary">Git Activity</strong>
					</div>
					<div class="toast-body">
						<div ng-repeat="c in todayCommits">
							<img class="rep-img rounded-circle" ng-src="getUserImg.ashx?sz=20&amp;eml={{c.AUTHOREML}}" alt="Smile" height="20" width="20" style="float: left">
							<span>{{c.LOGIN}}</span>
							<div style="float: left" ng-show="c.TTID > 0">
								<uc:defNum member="c" runat="server" style="float: left" />
							</div>
							<span class="{{c.TTID > 0 ? '' : 'badge badge-success'}}">{{c.TTSUMMARY}}</span>
						</div>
					</div>
				</div>
			</div>
		</div>
	</div>
</asp:Content>
