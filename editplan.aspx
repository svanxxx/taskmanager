<%@ Page Title="Edit Plan" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="editplan.aspx.cs" Inherits="PlanEditor" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/editplan_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/editplan_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
	<script src="scripts/jquery.signalR-2.3.0.min.js"></script>
	<script src="signalr/hubs"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-cloak ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="alert alert-danger savebutton btn-group-vertical" ng-cloak ng-show="changed">
			<button type="button" class="btn btn-lg btn-info" ng-click="saveDefects()">Save</button>
			<button type="button" class="btn btn-lg btn-danger" ng-click="discardDefects()">Discard</button>
		</div>
		<div class="d-flex">
			<button type="button" class="flex-fill btn btn-sm {{currentuserid===u.ID?'btn-secondary':'btn-outline-secondary'}}" ng-click="changeuser(u, true)" ng-repeat="u in filtered = (users | filter:{ INWORK: true })" data-toggle="tooltip" title="{{u.PERSON_NAME}}">
				<img class="rounded-circle" ng-src="{{'getUserImg.ashx?id=' + u.ID}}" alt="Smile" height="20" width="20" style="margin: auto;" />
				{{u.LOGIN}}
			</button>
		</div>
		<div class="row mt-2">
			<div class="col-xl-2 d-none d-xl-block">
				<div class="alert alert-primary">
					<p>To schedule the task: click on radio button for the task! Then holding <strong>Ctrl</strong> key press <strong>move up</strong> or <strong>move down</strong> to move the task in list correspondingly.</p>
					<p>
						Click
						<button type="button" class="btn btn-xs"><i class="fas fa-arrow-up"></i></button>
						to move task the the top which will result in the task being done first of all
					</p>
					<p>
						Click
						<button type="button" class="btn btn-xs"><i class="fas fa-arrow-right"></i></button>
						to move task out of schedlue
					</p>
					<p>
						In unscheduled section click
						<button type="button" class="btn btn-xs"><i class="fas fa-arrow-left"></i></button>
						to move task into the schedlue being on the top of list
					</p>
				</div>
				<div class="alert alert-primary">
					<button onclick="copyurl()" type="button" class="btn btn-link"><i class="fas fa-copy"></i>Copy page link</button>
				</div>
			</div>
			<div class="col-xl-9 col-lg-10">
				<div>
					<ul class="nav nav-pills nav-justified p-0">
						<li class="nav-item p-0">
							<a class="nav-link active" data-toggle="pill" href="#plan">
								<span>Plan </span>
								<span class="badge badge-light">{{defects.length}}</span>
							</a>
						</li>
						<li class="nav-item p-0">
							<a class="nav-link" data-toggle="pill" href="#unscheduled">Unscheduled <span class="badge badge-light">{{unscheduled.length}}</span></a>
						</li>
					</ul>
					<div class="tab-content panel panel-default">
						<div id="plan" class="tab-pane active">
							<table style="width: 100%">
								<tr class="task alert {{d.orderchanged?'data-changed':''}}" ng-repeat="d in defects" ng-style="{{d.DISPO | getDispoColorById:this}}">
									<td><a href="showtask.aspx?ttid={{d.ID}}" target="_blank"><span class="badge badge-pill badge-secondary">{{d.ID}}</span></a></td>
									<td><span class="badge badge-danger">{{d.ESTIM}}</span></td>
									<td><span data-toggle="tooltip" title="{{d.SUMMARY}}">{{d.SUMMARY | limitTo:135}}</span></td>
									<td>
										<input class="taskselector" type="radio" name="optradio" ng-keydown="taskMove(d, $event)"></td>
									<td>
										<img height="20" width="20" class="rounded-circle taskselector" ng-src="{{'getUserImg.ashx?id=' + d.SMODTRID}}" title="{{d.SMODIFIER}}" /></td>
									<td>
										<button ng-click="tasktotop(d)" data-toggle="tooltip" title="Move to top" type="button" class="btn btn-xs taskselector"><i class="fas fa-arrow-up"></i></button>
									</td>
									<td>
										<button ng-click="unscheduletask(d)" data-toggle="tooltip" title="Remove from the schedule list" type="button" class="btn btn-xs taskselector"><i class="fas fa-arrow-right"></i></button>
									</td>
								</tr>
							</table>
						</div>
						<div id="unscheduled" class="tab-pane fade">
							<table style="width: 100%">
								<tr class="task alert {{d.orderchanged?'data-changed':''}}" ng-repeat="d in unscheduled" ng-style="{{d.DISPO | getDispoColorById:this}}">
									<td><a href="showtask.aspx?ttid={{d.ID}}" target="_blank"><span class="badge badge-pill badge-secondary">{{d.ID}}</span></a></td>
									<td><span class="badge badge-danger">{{d.ESTIM}}</span></td>
									<td><span data-toggle="tooltip" title="{{d.SUMMARY}}">{{d.SUMMARY | limitTo:135}}</span></td>
									<td>
										<button ng-click="scheduletask(d)" data-toggle="tooltip" title="Add to schedule list" type="button" class="btn btn-xs taskselector"><i class="fas fa-arrow-left"></i></button>
									</td>
								</tr>
							</table>
						</div>
					</div>
				</div>
			</div>
			<div class="col-xl-1 col-lg-2">
				<div class="btn-group-vertical btn-group-sm d-flex">
					<button type="button" class="btn btn-sm btn-outline-secondary mb-1" ng-click="add2Plan()" data-toggle="tooltip" title="Add Task To Currently Selected User On The Top Of The Plan">
						<i class="fas fa-plus-circle"></i>Add Task
					</button>
					<button ng-show="isadmin" type="button" class="btn btn-sm btn-outline-secondary" ng-click="addVacs()" data-toggle="tooltip" title="Add vacations to current user">
						<i class="fas fa-plane"></i>Add Vacations
					</button>
				</div>
			</div>
		</div>
	</div>
</asp:Content>
