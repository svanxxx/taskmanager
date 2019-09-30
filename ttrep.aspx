<%@ Page Title="Tasks" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="ttrep.aspx.cs" Inherits="TTRep" %>

<%@ Register Src="~/controls/DefectNumControl.ascx" TagName="defNum" TagPrefix="uc" %>
<%@ Register Src="~/controls/DefectEstControl.ascx" TagName="defEst" TagPrefix="uc" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/ttrep_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/ttrep_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
	<script <%="src='" + Settings.CurrentSettings.COLRESIZABLECDN.ToString() + "colResizable-1.5.min.js'" %>></script>
	<script src="scripts/jquery.signalR-2.3.0.min.js"></script>
	<script src="signalr/hubs"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<input type="hidden" id="filters" value='<%=Newtonsoft.Json.JsonConvert.SerializeObject(StoredDefectsFilter.Enum(CurrentContext.TTUSERID))%>' />
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="alert alert-danger savebutton btn-group-vertical" ng-cloak ng-show="changed">
			<button type="button" class="btn btn-lg btn-info" ng-click="applyfilter()">Apply Filter</button>
			<button type="button" class="btn btn-lg btn-danger" ng-click="discardfilter()">Discard</button>
		</div>

		<div class="modal" id="batchchanges">
			<div class="modal-dialog modal-dialog-centered">
				<div class="modal-content">
					<div class="modal-header">
						<h4 class="modal-title">Select actions to apply to selected tasks</h4>
						<button type="button" class="close" data-dismiss="modal">&times;</button>
					</div>
					<div class="modal-body">
						<div class="input-group input-group-sm">
							<div class="input-group-prepend">
								<div class="input-group-text">
									<input type="checkbox" ng-model="apply.disposition.use">
								</div>
							</div>
							<div class="input-group-prepend">
								<span class="input-group-text">Disposition</span>
							</div>
							<select class="form-control" ng-model="apply.disposition.value">
								<option ng-repeat="d in dispos" value="{{d.ID}}">{{d.DESCR}}</option>
							</select>
						</div>
						<div class="input-group input-group-sm">
							<div class="input-group-prepend">
								<div class="input-group-text">
									<input type="checkbox" ng-model="apply.component.use">
								</div>
							</div>
							<div class="input-group-prepend">
								<span class="input-group-text">Component</span>
							</div>
							<select class="form-control" ng-model="apply.component.value">
								<option ng-repeat="c in comps" value="{{c.ID}}">{{c.DESCR}}</option>
							</select>
						</div>
						<div class="input-group input-group-sm">
							<div class="input-group-prepend">
								<div class="input-group-text">
									<input type="checkbox" ng-model="apply.severity.use">
								</div>
							</div>
							<div class="input-group-prepend">
								<span class="input-group-text">Severity</span>
							</div>
							<select class="form-control" ng-model="apply.severity.value">
								<option ng-repeat="c in severs" value="{{c.ID}}">{{c.DESCR}}</option>
							</select>
						</div>
						<div class="input-group input-group-sm">
							<div class="input-group-prepend">
								<div class="input-group-text">
									<input type="checkbox" ng-model="apply.user.use">
								</div>
							</div>
							<div class="input-group-prepend">
								<span class="input-group-text">User</span>
							</div>
							<select class="form-control" ng-model="apply.user.value">
								<option ng-repeat="u in users" value="{{u.ID}}">{{u.FULLNAME}}</option>
							</select>
						</div>
						<div class="input-group input-group-sm">
							<div class="input-group-prepend">
								<div class="input-group-text">
									<input type="checkbox" ng-model="apply.priority.use">
								</div>
							</div>
							<div class="input-group-prepend">
								<span class="input-group-text">Priority</span>
							</div>
							<select class="form-control" ng-model="apply.priority.value">
								<option ng-repeat="p in prios | orderBy:'FORDER'" value="{{p.ID}}">{{p.DESCR}}</option>
							</select>
						</div>
						<div class="input-group input-group-sm">
							<div class="input-group-prepend">
								<div class="input-group-text">
									<input type="checkbox" ng-model="apply.estim.use">
								</div>
							</div>
							<div class="input-group-prepend">
								<span class="input-group-text">Estimated</span>
							</div>
							<input type="text" class="form-control" ng-model="apply.estim.value">
						</div>
						<div class="input-group input-group-sm">
							<div class="input-group-prepend">
								<div class="input-group-text">
									<input type="checkbox" ng-model="apply.date.use">
								</div>
							</div>
							<div class="input-group-prepend">
								<span class="input-group-text">Date</span>
							</div>
							<input type="date" class="form-control" ng-model="apply.date.value">
						</div>
						<div class="input-group input-group-sm">
							<div class="input-group-prepend">
								<div class="input-group-text">
									<input type="checkbox" ng-model="apply.summary.use">
								</div>
							</div>
							<div class="input-group-prepend" data-toggle="tooltip" title="Enter two values for replacement or only the second for entire summary set">
								<span class="input-group-text">Summary</span>
							</div>
							<input type="text" class="form-control" ng-model="apply.summary.value1">
							<input type="text" class="form-control" ng-model="apply.summary.value2">
						</div>
					</div>
					<div class="modal-footer">
						<button type="button" class="btn btn-primary" ng-click="changeDefects()" id="createttbtn">Apply</button>
						<button type="button" class="btn btn-danger" data-dismiss="modal">Cancel</button>
					</div>
				</div>
			</div>
		</div>

		<div class="row pb-2">
			<div class="col-sm-3">
				<div class="input-group input-group-sm">
					<div class="input-group-prepend btn-group">
						<button onclick="copyurl()" data-toggle="tooltip" title="Copy link to this report to clipboard" type="button" class="btn btn-outline-secondary btn-sm"><i class="fas fa-link"></i></button>
						<button ng-click="loadData()" data-toggle="tooltip" title="Reload Tasks" type="button" class="btn btn-outline-secondary btn-sm"><i class="fas fa-sync-alt"></i></button>
						<button ng-disabled="!defectsselected" data-toggle="modal" data-target="#batchchanges" type="button" class="btn btn-outline-secondary btn-sm"><i data-toggle="tooltip" title="Apply bulk changes" class="fas fa-check-double"></i></button>
						<button ng-click="duplicate()" ng-disabled="!defectsselected" type="button" class="btn btn-outline-secondary btn-sm"><i data-toggle="tooltip" title="Clone Selected" class="fas fa-clone"></i></button>
					</div>
					<div class="input-group-append">
						<span data-toggle="tooltip" title="Stats:" class="input-group-text"><i class="fas fa-hashtag"></i>{{defects.length}}:<span ng-bind-html="effort | rawHtml"></span></span>
					</div>
				</div>
			</div>
			<div class="col-sm-3">
				<div class="input-group input-group-sm">
                    <button ng-click="filterCopyurl()" data-toggle="tooltip" title="Copy link to this filter to clipboard" type="button" class="btn btn-outline-secondary btn-sm"><i class="fas fa-link"></i></button>
					<div class="btn-group">
						<button type="button" class="btn-sm btn btn-outline-secondary dropdown-toggle" data-toggle="dropdown">
							<i class="fas fa-filter"></i>{{selectedFilter == null ? ' - Select - ' : selectedFilter.NAME}}
						</button>
						<div class="dropdown-menu">
							<button ng-repeat="f in filters" ng-click="applySelectedFilter(f.ID)" type="button" class="dropdown-item btn btn-outline-secondary">
								<i class="fas {{f.SHARED ? 'fa-user-friends text-danger' : 'fa-user-check'}}"></i>
								{{f.NAME}}
							</button>
						</div>
					</div>
					<div class="input-group-append btn-group">
						<div class="btn-group">
							<button type="button" class="btn-sm btn btn-outline-secondary dropdown-toggle" data-toggle="dropdown">
								<i class="fas fa-save"></i>
							</button>
							<div class="dropdown-menu">
								<button ng-click="saveFilter(true)" type="button" class="dropdown-item btn btn-outline-secondary"><i class="fas fa-user-check"></i>Personal</button>
								<button ng-click="saveFilter(false)" type="button" class="dropdown-item btn btn-outline-secondary"><i class="fas fa-user-friends text-danger"></i>Shared</button>
							</div>
						</div>
						<button ng-click="resetFilter()" data-toggle="tooltip" title="Reset Filter" type="button" class="btn btn-outline-secondary"><i class="fas fa-home"></i></button>
						<button ng-click="deleteFilter()" data-toggle="tooltip" title="Delete Filter" type="button" class="btn btn-outline-secondary"><i class="fas fa-trash-alt"></i></button>
					</div>
				</div>
			</div>
			<div class="col-sm-6">
				<div class="input-group input-group-sm">
					<div class="input-group-prepend">
						<span class="input-group-text">Enter phrase in double quotes or a set of words to be found:</span>
					</div>
					<input class="form-control" id="searchtxt" type="search" name="srch" ng-model="DefectsFilter.text" ng-change="changed=true" ng-keypress="onGo($event)" autofocus />
				</div>
			</div>
		</div>
		<table class="table table-bordered table-colresizable">
			<thead>
				<tr>
					<th>
						<button ng-click="checkall()" type="button" class="btn btn-sm btn-default"><i class="fas fa-check-square"></i></button>
					</th>
					<th class="{{classFiltered('ID')}}">
						<div class="dropdown" data-toggle="tooltip" title="Task ID">
							<button type="button" class="btn btn-outline-light text-dark btn-sm dropdown-toggle" data-toggle="dropdown">
								ID
							</button>
							<div class="dropdown-menu">
								<div class="input-group input-group-sm dropdown-item-text">
									<div class="input-group-prepend">
										<button type="button" ng-click="DefectsFilter.ID = '';" class="btn btn-primary btn-sm"><i class="fas fa-broom"></i></button>
									</div>
									<input type="text" class="form-control" ng-model="DefectsFilter.ID">
								</div>
								<span class="dropdown-item-text">Enter ids in form: xxx,yyy-zzz,aaa separating by comma, with ranges indicated by - symbol</span>
							</div>
						</div>
					</th>
					<th class="{{classFiltered('startEstim', 'endEstim')}}">
						<div class="dropdown" data-toggle="tooltip" title="Time Estimation">
							<button type="button" class="btn btn-outline-light text-dark btn-sm dropdown-toggle" data-toggle="dropdown"><i class="far fa-clock"></i></button>
							<div class="dropdown-menu">
								<button ng-click="DefectsFilter.startEstim='';DefectsFilter.endEstim='';" type="button" class="btn btn-outline-light text-dark dropdown-item-text">Reset Estimation Limits</button>
								<div class="input-group input-group-sm dropdown-item-text">
									<div class="input-group-prepend"><span class="input-group-text">From:</span></div>
									<input type="number" class="form-control" ng-model="DefectsFilter.startEstim">
									<div class="input-group-append">
										<button type="button" ng-click="DefectsFilter.startEstim = '';" class="btn btn-primary btn-sm"><i class="fas fa-broom"></i></button>
									</div>
								</div>
								<div class="input-group input-group-sm dropdown-item-text">
									<div class="input-group-prepend"><span class="input-group-text">UpTo:</span></div>
									<input type="number" class="form-control" ng-model="DefectsFilter.endEstim">
									<div class="input-group-append">
										<button type="button" ng-click="DefectsFilter.endEstim = '';" class="btn btn-primary btn-sm"><i class="fas fa-broom"></i></button>
									</div>
								</div>
							</div>
						</div>
					</th>
					<th class="{{classFiltered('users')}}">
						<div class="dropdown middlecol" data-toggle="tooltip" title="Assigned To User">
							<button type="button" class="btn btn-outline-light text-dark btn-sm dropdown-toggle" data-toggle="dropdown">
								User
							</button>
							<div class="dropdown-menu {{filterusers?'':'pre-scrollable'}}">
								<div class="input-group input-group-sm">
									<div class="input-group-prepend" data-toggle="tooltip" title="Show/Hide all/active users">
										<button type="button" ng-click="filterusers=!filterusers;updateUsersFilter();$event.stopPropagation();" class="btn btn-primary btn-sm">
											<i class="fas fa-users" ng-show="filterusers"></i>
											<i class="fas fa-user-minus" ng-show="!filterusers"></i>
										</button>
									</div>
									<input type="text" class="form-control refselector">
									<div class="input-group-append" data-toggle="tooltip" title="Reset filter for this column">
										<button type="button" ng-click="resetReferenceFilter('users', $event)" class="btn btn-primary btn-sm"><i class="fas fa-broom"></i></button>
									</div>
								</div>
								<div ng-repeat="u in filtered = (users | filter:{ show: true })" class="dropdown-item pt-0 pb-0">
									<label class="form-check-label">
										<img async class="rounded-circle" ng-src="{{'getUserImg.ashx?sz=30&ttid=' + u.ID}}" alt="Smile" height="30" width="30">
										<input ng-click="changeReferenceFilter(u.ID, 'users')" ng-checked="u.filter" type="checkbox" value="">
										<span>{{u.FULLNAME}}</span>
									</label>
								</div>
							</div>
						</div>
					</th>
					<th>Ref</th>
					<th>Summary</th>
					<th class="{{classFiltered('dispositions')}}">
						<div class="dropdown middlecol" data-toggle="tooltip" title="Disposition">
							<button type="button" class="btn btn-outline-light text-dark btn-sm dropdown-toggle" data-toggle="dropdown">
								Disp
							</button>
							<div class="dropdown-menu dropdown-menu-right">
								<div class="input-group input-group-sm">
									<input type="text" class="form-control refselector">
									<div class="input-group-append">
										<button type="button" ng-click="resetReferenceFilter('dispositions', $event)" class="btn btn-primary btn-sm"><i class="fas fa-broom"></i></button>
									</div>
								</div>
								<div ng-repeat="d in dispos" class="dropdown-item pt-0 pb-0" ng-style="d.ID | getDispoColorById:this">
									<label class="form-check-label">
										<input ng-click="changeReferenceFilter(d.ID, 'dispositions')" ng-checked="d.filter" type="checkbox" value="">
										<span>{{d.DESCR}}</span>
									</label>
								</div>
							</div>
						</div>
					</th>
					<th class="{{classFiltered('components')}}">
						<div class="dropdown middlecol" data-toggle="tooltip" title="Component">
							<button type="button" class="btn btn-outline-light text-dark btn-sm dropdown-toggle" data-toggle="dropdown">
								Comp
							</button>
							<div class="dropdown-menu dropdown-menu-right pre-scrollable">
								<div class="input-group input-group-sm">
									<input type="text" class="form-control refselector">
									<div class="input-group-append">
										<button type="button" ng-click="resetReferenceFilter('components', $event)" class="btn btn-primary btn-sm"><i class="fas fa-broom"></i></button>
									</div>
								</div>
								<div ng-repeat="d in comps" class="dropdown-item pt-0 pb-0">
									<label class="form-check-label">
										<input ng-click="changeReferenceFilter(d.ID, 'components')" ng-checked="d.filter" type="checkbox" value="">
										<span>{{d.DESCR}}</span>
									</label>
								</div>
							</div>
						</div>
					</th>
					<th class="{{classFiltered('severities')}}">
						<div class="dropdown middlecol" data-toggle="tooltip" title="Severity">
							<button type="button" class="btn btn-outline-light text-dark btn-sm dropdown-toggle" data-toggle="dropdown">
								Sev
							</button>
							<div class="dropdown-menu dropdown-menu-right pre-scrollable">
								<div class="input-group input-group-sm">
									<input type="text" class="form-control refselector">
									<div class="input-group-append">
										<button type="button" ng-click="resetReferenceFilter('severities', $event)" class="btn btn-primary btn-sm"><i class="fas fa-broom"></i></button>
									</div>
								</div>
								<div ng-repeat="d in severs" class="dropdown-item pt-0 pb-0">
									<label class="form-check-label">
										<input ng-click="changeReferenceFilter(d.ID, 'severities')" ng-checked="d.filter" type="checkbox" value="">
										<span>{{d.DESCR}}</span>
									</label>
								</div>
							</div>
						</div>
					</th>
					<th class="{{classFiltered('createdUsers')}}">
						<div class="dropdown middlecol">
							<button type="button" class="btn btn-outline-light text-dark btn-sm dropdown-toggle" data-toggle="dropdown">By</button>
							<div class="dropdown-menu dropdown-menu-right {{filterusers?'':'pre-scrollable'}}">
								<div class="input-group input-group-sm">
									<div class="input-group-prepend" data-toggle="tooltip" title="Show/Hide all/active users">
										<button type="button" ng-click="filterusers=!filterusers;updateUsersFilter();$event.stopPropagation();" class="btn btn-primary btn-sm">
											<i class="fas fa-users" ng-show="filterusers"></i>
											<i class="fas fa-user-minus" ng-show="!filterusers"></i>
										</button>
									</div>
									<input type="text" class="form-control refselector">
									<div class="input-group-append" data-toggle="tooltip" title="Reset filter for this column">
										<button type="button" ng-click="resetReferenceFilter('createdUsers', $event)" class="btn btn-primary btn-sm"><i class="fas fa-broom"></i></button>
									</div>
								</div>
								<div ng-repeat="u in filtered = (users | filter:{ show: true })" class="dropdown-item pt-0 pb-0">
									<label class="form-check-label">
										<img async class="rounded-circle" ng-src="{{'getUserImg.ashx?sz=30&ttid=' + u.ID}}" alt="Smile" height="30" width="30">
										<input ng-click="changeReferenceFilter(u.ID, 'createdUsers')" ng-checked="u.createdFilter" type="checkbox" value="">
										<span>{{u.FULLNAME}}</span>
									</label>
								</div>
							</div>
						</div>
					</th>
					<th class="{{classFiltered('startDateCreated', 'endDateCreated')}}">
						<div class="dropdown" data-toggle="tooltip" title="Date Created">
							<button type="button" class="btn btn-outline-light text-dark btn-sm dropdown-toggle" data-toggle="dropdown"><i class="far fa-clock"></i>Cre</button>
							<div class="dropdown-menu dropdown-menu-right">
								<button class="btn btn-outline-secondary dropdown-item pl-5 pr-5 ml-2 mr-2" type="button" ng-click="DefectsFilter.startDateCreated='';DefectsFilter.endDateCreated='';">Reset Date Created Filter</button>
								<div class="input-group input-group-sm dropdown-item-text">
									<div class="input-group-prepend"><span class="input-group-text">From:</span></div>
									<input type="date" class="form-control" ng-model="DefectsFilter.startDateCreated">
									<div class="input-group-append">
										<button type="button" ng-click="DefectsFilter.startDateCreated = '';" class="btn btn-primary btn-sm"><i class="fas fa-broom"></i></button>
									</div>
								</div>
								<div class="input-group input-group-sm dropdown-item-text">
									<div class="input-group-prepend"><span class="input-group-text">UpTo:</span></div>
									<input type="date" class="form-control" ng-model="DefectsFilter.endDateCreated">
									<div class="input-group-append">
										<button type="button" ng-click="DefectsFilter.endDateCreated = '';" class="btn btn-primary btn-sm"><i class="fas fa-broom"></i></button>
									</div>
								</div>
							</div>
						</div>
					</th>
					<th class="{{classFiltered('startDateEnter', 'endDateEnter')}}">
						<div class="dropdown" data-toggle="tooltip" title="Date Entered">
							<button type="button" class="btn btn-outline-light text-dark btn-sm dropdown-toggle" data-toggle="dropdown"><i class="far fa-clock"></i>Ent</button>
							<div class="dropdown-menu dropdown-menu-right">
								<button class="btn btn-outline-secondary dropdown-item pl-5 pr-5 ml-2 mr-2" type="button" ng-click="DefectsFilter.startDateEnter='';DefectsFilter.endDateEnter='';">Reset Date Entered Filter</button>
								<div class="dropdown-item-text input-group input-group-sm">
									<div class="input-group-prepend"><span class="input-group-text">From:</span></div>
									<input type="date" class="form-control" ng-model="DefectsFilter.startDateEnter">
									<div class="input-group-append">
										<button type="button" ng-click="DefectsFilter.startDateEnter = '';" class="btn btn-primary btn-sm"><i class="fas fa-broom"></i></button>
									</div>
								</div>
								<div class="dropdown-item-text input-group input-group-sm">
									<div class="input-group-prepend"><span class="input-group-text">UpTo:</span></div>
									<input type="date" class="form-control" ng-model="DefectsFilter.endDateEnter">
									<div class="input-group-append">
										<button type="button" ng-click="DefectsFilter.endDateEnter = '';" class="btn btn-primary btn-sm"><i class="fas fa-broom"></i></button>
									</div>
								</div>
							</div>
						</div>
					</th>
				</tr>
			</thead>
			<tbody ng-cloak>
				<tr ng-repeat="d in defects" ng-style="d.DISPO | getDispoColorById:this">
					<td>
						<input type="checkbox" ng-model="d.checked"></td>
					<td>
						<uc:defNum runat="server" />
					</td>
					<td>
						<uc:defEst runat="server" />
					</td>
					<td><a target="_blank" href="{{'editplan.aspx?userid='}}{{d.AUSER | getUserTRIDById:this}}">{{d.AUSER | getUserById:this}}</a></td>
					<td>{{d.REFERENCE}}</td>
					<td ng-bind-html="d.SUMMARY | sumFormat"></td>
					<td>{{d.DISPO | getDispoById:this}}</td>
					<td>{{d.COMP | getCompById:this}}</td>
					<td>{{d.SEVE | getSeveById:this}}</td>
					<td>{{d.CREATEDBY | getUserById:this}}</td>
					<td>{{d.CREATED}}</td>
					<td>{{d.DATE}}</td>
				</tr>
			</tbody>
		</table>
	</div>
</asp:Content>
