<%@ Page Title="Task" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="showtask.aspx.cs" Inherits="Showtask" %>

<%@ Register Src="~/controls/CommitsControl.ascx" TagName="commits" TagPrefix="uc" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/showtask_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/buildshelper_js")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/showtask_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
	<script src="scripts/jquery.signalR-2.3.0.min.js"></script>
	<script src="signalr/hubs"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<input type="hidden" id="deflist" value="<%=Settings.CurrentSettings.DEFLISTENERS.ToString()%>" />
	<input type="hidden" id="buildtime" value="<%=Settings.CurrentSettings.BUILDTIME.ToString()%>" />
	<input type="hidden" id="testlink" value="<%=Settings.CurrentSettings.TESTREQUESTLINK.ToString()%>" />
	<input type="hidden" id="releasettid" value="<%=Settings.CurrentSettings.RELEASETTID.ToString()%>" />
	<input type="hidden" id="defectdefaults" value='<%=Newtonsoft.Json.JsonConvert.SerializeObject(DefectDefaults.CurrentDefaults)%>' />

	<div ng-app="mpsapplication" ng-controller="mpscontroller" ng-cloak>
		<div class="alert alert-danger savebutton btn-group-vertical" ng-cloak ng-show="changed">
			<button type="button" class="btn btn-lg btn-info" ng-click="saveDefect()">Save</button>
			<button type="button" class="btn btn-lg btn-danger" ng-click="discardDefect()">Discard</button>
		</div>
		<div class="row">
			<div class="col-lg-2 hidden-md">
				<button onclick="copyurl()" type="button" class="btn btn-outline-secondary btn-block btn-sm">Copy link&nbsp;<i class="fas fa-link"></i></button>
				<button ng-click="duplicate()" type="button" class="btn btn-outline-secondary btn-block btn-sm">Duplicate&nbsp;<i class="fas fa-clone"></i></button>
				<button ng-click="resettask()" ng-disabled="!canChangeDefect()" type="button" class="btn btn-outline-secondary btn-block btn-sm">Reset To Re-Use&nbsp;<i class="fas fa-recycle"></i></button>
			</div>
			<div class="col-lg-8 col-md-12">
				<div class="input-group input-group-sm mb-1">
					<div class="input-group-prepend">
						<button title="Copy task label to clipboard" data-toggle="tooltip" class="btn btn-default btn-sm" type="button" ng-click="cliplabl()">TT{{defect.ID}}</button>
					</div>
					<input type="text" class="form-control" ng-disabled="!canChangeDefect()" ng-model="defect.SUMMARY">
				</div>
				<div class="hidden-xs row toolbar mb-1" ng-style="defect.DISPO | getDispoColorById:this">
					<div class="col-sm-3">
						<div class="input-group input-group-sm">
							<div class="input-group-prepend w-25">
								<span class="input-group-text w-100"><a href="types.aspx" target="_blank">Type</a></span>
							</div>
							<select class="form-control" id="type" ng-disabled="!canChangeDefect()" ng-model="defect.TYPE">
								<option value="{{t.ID}}" ng-repeat="t in types">{{t.DESCR}}</option>
							</select>
						</div>
						<div class="input-group input-group-sm">
							<div class="input-group-prepend w-25">
								<span class="input-group-text w-100"><a href="products.aspx" target="_blank">Product</a></span>
							</div>
							<select class="form-control" id="product" ng-disabled="!canChangeDefect()" ng-model="defect.PRODUCT">
								<option value="{{p.ID}}" ng-repeat="p in products">{{p.DESCR}}</option>
							</select>
						</div>
						<div class="input-group input-group-sm">
							<div class="input-group-prepend w-25" title="Reference" data-toggle="tooltip">
								<span class="input-group-text w-100">Ref</span>
							</div>
							<input type="text" class="form-control" id="reference" ng-disabled="!canChangeDefect()" ng-model="defect.REFERENCE">
						</div>
					</div>
					<div class="col-sm-3">
						<div class="input-group input-group-sm">
							<div class="input-group-prepend w-25" title="Disposition" data-toggle="tooltip">
								<span class="input-group-text w-100"><a href="dispositions.aspx" target="_blank">Dispo</a></span>
							</div>
							<select class="form-control" id="dispo" ng-disabled="!canChangeDefect()" ng-model="defect.DISPO">
								<option ng-repeat="d in dispos | orderBy:'FORDER'" value="{{d.ID}}" style="background-color: {{d .COLOR}}">{{d.DESCR}}</option>
							</select>
						</div>
						<div class="input-group input-group-sm">
							<div class="input-group-prepend w-25">
								<span class="input-group-text w-100"><a href="priorities.aspx" target="_blank">Priority</a></span>
							</div>
							<select class="form-control" id="prio" ng-disabled="!canChangeDefect()" ng-model="defect.PRIO">
								<option ng-repeat="p in priorities | orderBy:'FORDER'" value="{{p.ID}}">{{p.DESCR}}</option>
							</select>
						</div>
						<div class="input-group input-group-sm">
							<div class="input-group-prepend w-25" title="Component" data-toggle="tooltip">
								<span class="input-group-text w-100"><a href="components.aspx" target="_blank">Comp</a></span>
							</div>
							<select class="form-control" id="comp" ng-disabled="!canChangeDefect()" ng-model="defect.COMP">
								<option ng-repeat="c in comps | orderBy:'FORDER'" value="{{c.ID}}">{{c.DESCR}}</option>
							</select>
						</div>
					</div>
					<div class="col-sm-3">
						<div class="input-group input-group-sm">
							<div class="input-group-prepend w-25">
								<span class="input-group-text w-100"><a href="severities.aspx" target="_blank">Severity</a></span>
							</div>
							<select class="form-control" id="dispo" ng-disabled="!canChangeDefect()" ng-model="defect.SEVE">
								<option ng-repeat="s in severs" value="{{s.ID}}">{{s.DESCR}}</option>
							</select>
						</div>
						<div class="input-group input-group-sm">
							<div class="input-group-prepend w-25">
								<span class="input-group-text w-100">Date</span>
							</div>
							<input type="date" id="date" class="form-control" ng-disabled="!canChangeDefect()" ng-model="defect.DATE">
						</div>
						<div class="input-group input-group-sm">
							<div class="input-group-prepend w-25">
								<span class="input-group-text w-100">Created</span>
							</div>
							<select class="form-control" id="created" ng-disabled="!canChangeDefect()" ng-model="defect.CREATEDBY">
								<option ng-repeat="u in users | orderBy:'FULLNAME'" ng-show="u.ACTIVE" value="{{u.ID}}">{{u.FULLNAME}}</option>
							</select>
						</div>
					</div>
					<div class="col-sm-3">
						<div class="input-group input-group-sm">
							<div class="input-group-prepend w-25">
								<span class="input-group-text w-100"><a href="editplan.aspx?userid={{defect.AUSER | getUserTRIDById:this}}" target="_blank">Assigned</a></span>
							</div>
							<select class="form-control" id="auser" ng-disabled="!canChangeDefect()" ng-model="defect.AUSER">
								<option ng-repeat="u in users | orderBy:'FULLNAME'" ng-show="u.ACTIVE" value="{{u.ID}}">{{u.FULLNAME}}</option>
							</select>
						</div>
						<div class="input-group input-group-sm">
							<div class="input-group-prepend w-25">
								<span class="input-group-text w-100">Effort</span>
							</div>
							<input type="number" min="1" max="999" id="estim" class="form-control w-25 pr-0" ng-disabled="!canChangeDefect()" ng-model="defect.ESTIM">
							<select class="form-control w-50" ng-disabled="!canChangeDefect()" ng-model="defect.ESTIMBY">
								<option ng-repeat="u in users | orderBy:'FULLNAME'" ng-show="u.ACTIVE" value="{{u.ID}}">{{u.FULLNAME}}</option>
							</select>
						</div>
						<div class="input-group input-group-sm">
							<div class="input-group-prepend w-25">
								<span class="input-group-text w-100">Order</span>
							</div>
							<input type="number" id="order" class="form-control" ng-disabled="!canChangeDefect()" ng-model="defect.ORDER">
							<button ng-disabled="!canChangeDefect()" type="button" class="bnt btn-outline-default" ng-click="chgOrder(false)"><i class="fas fa-arrow-up"></i></button>
							<button ng-disabled="!canChangeDefect()" type="button" class="bnt btn-outline-default" ng-click="chgOrder(true)"><i class="fas fa-arrow-down"></i></button>
						</div>
					</div>
				</div>
				<ul id="tasktabs" class="nav nav-tabs nav-justified" role="tablist">
					<li class="{{specsStyle()}} nav-item"><a class="nav-link small active" data-toggle="tab" href="#specification"><i class="far fa-list-alt"></i>{{tab_specs}}</a></li>
					<li class="nav-item"><a class="nav-link small" data-toggle="tab" href="#detail"><i class="fas fa-search-plus"></i>&nbsp;Details</a></li>
					<li class="nav-item" ng-click="changetab($event)"><a class="nav-link small" data-toggle="tab" href="#workflow"><i class="fas fa-sync"></i>{{tab_workflow}}</a></li>
					<li class="nav-item" ng-click="changetab($event)"><a class="nav-link small" data-toggle="tab" href="#history"><i class="fas fa-history"></i>{{tab_history}}</a></li>
					<li class="nav-item" ng-click="changetab($event)"><a class="nav-link small" data-toggle="tab" href="#attachments"><i class="fas fa-paperclip"></i>{{tab_attachs}}</a></li>
					<li class="nav-item" ng-click="changetab($event)" id="gittab"><a class="nav-link small" data-toggle="tab" href="#taskgit"><i class="fas fa-code-branch"></i>Git</a></li>
					<li class="nav-item" ng-click="changetab($event)" id="buildstab"><a class="nav-link small" data-toggle="tab" href="#taskbuilds"><i class="fas fa-tools"></i>{{tab_builds}}</a></li>
					<li class="nav-item" ng-click="changetab($event)"><a class="nav-link small" data-toggle="tab" href="#bst"><i class="fa fa-link"></i>{{tab_bst}}</a></li>
					<li class="nav-item"><a class="nav-link small" data-toggle="tab" href="#lockinfo"><i class="fas fa-lock"></i>&nbsp;Lock Info</a></li>
					<li class="nav-item"><a class="nav-link small" data-toggle="tab" href="#alarm"><i class="fas fa-envelope"></i>&nbsp;Alarm</a></li>
				</ul>
				<div class="tab-content">
					<div id="specification" class="tab-pane active">
						<textarea class="form-control form-control-sm" id="spec" rows="30" ng-disabled="!canChangeDefect()" ng-model="defect.SPECS"></textarea>
					</div>
					<div id="detail" class="tab-pane fade">
						<textarea class="form-control form-control-sm" id="Description" rows="30" ng-disabled="!canChangeDefect()" ng-model="defect.DESCR"></textarea>
					</div>
					<div id="bst" class="tab-pane fade">
						<div class="row">
							<div class="col-md-3">
								<input class="form-control form-control-sm" ng-model="batchsearch" type="text" />
								<ul class="nav nav-pills nav-justified">
									<li class="nav-item" ng-repeat="s in batchesslots">
										<a data-toggle="pill" class="nav-link py-1 {{$index==0?'active':''}}" href="#batches{{$index}}">{{$index+1}}</a>
									</li>
								</ul>
								<div class="tab-content">
									<div id="batches{{$index}}" class="tab-pane {{$index==0?'active':'fade'}}" ng-repeat="s in batchesslots">
										<div class="list-group">
											<a href="#" ng-click="add2Bst(batch)" class="list-group-item py-2" ng-repeat="batch in s">{{batch}}</a>
										</div>
									</div>
								</div>
							</div>
							<div class="col-md-9">
								<ul id="bsttabs" class="nav nav-pills">
									<li class="nav-item"><a id="{{bsttab_bat}}" class="nav-link py-1 active" data-toggle="tab" href="#batches">Batches <span ng-show="defect.BSTBATCHES !== ''" class="badge badge-pill badge-secondary">{{defect.BSTBATCHES.split("\n").length}}</span></a></li>
									<li class="nav-item"><a id="{{bsttab_com}}" class="nav-link py-1" data-toggle="tab" href="#commands">Commands <span ng-show="defect.BSTCOMMANDS !== ''" class="badge badge-pill badge-secondary">{{defect.BSTCOMMANDS.split("\n").length}}</span></a></li>
									<li class="nav-item"><a id="{{bsttab_his}}" class="nav-link py-1" data-toggle="tab" href="#bsthistory">History <span ng-show="builds.length > 0" class="badge badge-pill badge-secondary">{{builds.length}}</span></a></li>
								</ul>
								<div class="tab-content">
									<div id="batches" class="tab-pane active">
										<textarea id="bstbatches" class="form-control form-control-sm" rows="28" ng-disabled="!canChangeDefect()" ng-model="defect.BSTBATCHES"></textarea>
									</div>
									<div id="commands" class="tab-pane fade">
										<textarea id="bstcommands" class="form-control form-control-sm" rows="28" ng-disabled="!canChangeDefect()" ng-model="defect.BSTCOMMANDS"></textarea>
									</div>
									<div id="bsthistory" class="tab-pane fade">
										<ul class="list-group">
											<li class="list-group-item py-1" ng-repeat="b in builds">
												<div class="btn-group d-flex py-0">
													<a class="btn btn-outline-primary flex-fill" href ng-click="showTests(b.TESTGUID)">History for {{b.DATEUP}}</a>
													<a class="btn btn-outline-primary flex-fill" href="getinstall.ashx?type=devfip&version={{b.TESTGUID}}"><i class="fas fa-cloud-download-alt"></i>Fieldpro</a>
													<a class="btn btn-outline-primary flex-fill" href="getinstall.ashx?type=devmx&version={{b.TESTGUID}}"><i class="fas fa-cloud-download-alt"></i>Modules</a>
												</div>
											</li>
										</ul>
									</div>
								</div>
							</div>
						</div>
					</div>
					<div id="workflow" class="tab-pane fade">
						<label ng-show="!events">loading...</label>
						<div class="list-group">
							<a href="#" class="list-group-item list-group-item-action" ng-repeat="h in events | orderBy : 'ORDER'">
								<div class="media">
									<div class="media-left">
										<img class="rounded-circle" ng-src="{{'getUserImg.ashx?ttid=' + h.IDUSER}}" alt="Smile" height="30" width="30">
									</div>
									<div class="media-body">
										<h7 class="media-heading"><b>{{h.IDUSER | getUserById:this}}</b> <small style="float: right"><i>{{h.DATE}}</i></small></h7>
										<p>{{h.NOTES}}</p>
									</div>
								</div>
								{{h.EVENT}}:
								<img ng-show="h.ASSIGNUSERID > 0" class="rounded-circle" ng-src="{{'getUserImg.ashx?ttid=' + h.ASSIGNUSERID}}" alt="Smile" height="20" width="20" />
								{{h.ASSIGNUSERID | getUserById:this}} <span class="badge badge-secondary">{{h.TIME}}</span>
								{{h.NOTES}}	&nbsp;
							</a>
						</div>
					</div>
					<div id="history" class="tab-pane fade">
						<label ng-show="!history">loading...</label>
						<div class="list-group">
							<a href class="list-group-item list-group-item-action" ng-repeat="h in history">
								<div class="media">
									<div class="media-left">
										<img class="media-object rounded-circle" ng-src="{{'getUserImg.ashx?ttid=' + h.IDUSER}}" alt="Smile" height="30" width="30">
									</div>
									<div class="media-body">
										<b>
											<h7 class="media-heading">
											{{h.IDUSER | getUserById:this}}</b> <small style="float: right"><i>{{h.DATE}}</i></small></h7>
										<p>{{h.NOTES}}</p>
									</div>
								</div>
							</a>
						</div>
					</div>
					<div id="attachments" class="tab-pane fade">
						<label ng-show="!attachs">loading...</label>
						<button type="button" ng-disabled="!canChangeDefect()" ng-click="addFile()" id="button" class="btn btn-primary btn-xs">Add File...</button>
						<ul>
							<li ng-style="a.deleted ? {'text-decoration':'line-through'} : ''" ng-repeat="a in attachs">
								<a target="_blank" href="getattach.aspx?idrecord={{a.ID+'&ext='+getfileext(a.FILENAME)}}">
									<span>{{a.FILENAME}}</span>
									<img ng-src="getAttachImg.ashx?idrecord={{a.ID+'&ext='+getfileext(a.FILENAME)}}" style="max-width: 100%" />
									<img ng-src="{{a.newblob}}" style="max-width: 100%" />
								</a>&nbsp
								<button ng-click="deleteAttach(a.ID)" type="button" class="btn btn-danger btn-xs">Delete</button>
							</li>
						</ul>
					</div>
					<div id="lockinfo" class="tab-pane fade">
						<div class="list-group">
							<a href="#" class="list-group-item list-group-item-action">
								<img class="rounded-circle" ng-src="{{'getUserImg.ashx?id=' + lockedby}}" alt="Smile" height="42" width="42">
								<span class="badge badge-info">{{getMPSUserName(lockedby)}}</span>
							</a>
							<a href="#" class="list-group-item list-group-item-action list-group-item-primary">
								<button type="button" ng-click="releaseRequest()" class="btn btn-block btn-primary btn-lg">Request to release!</button>
							</a>
							<a href="#" class="list-group-item list-group-item-action list-group-item-danger">
								<button type="button" ng-click="releaseForce()" class="btn btn-block btn-danger btn-lg">Force Task Unlock</button>
							</a>
						</div>
					</div>
					<div id="taskgit" class="tab-pane fade">
						<div class="card">
							<div class="card-header bg-info">
								<div class="row">
									<div class="col-md-3">
										<label>Git branch (default is TT ID):</label>
									</div>
									<div class="col-md-3">
										<input type="text" class="form-control" ng-disabled="!canChangeDefect()" ng-model="defect.BRANCH">
									</div>
									<div class="col-md-3">
										<a href="builder.aspx" ng-show="isrelease()" class="btn btn-info btn-right-align" role="button">Release Marker</a>
									</div>
									<div class="col-md-3">
										<button type="button" class="btn btn-sm btn-danger btn-right-align" ng-disabled="!commits||commits.length<1" ng-click="deleteBranch()">Delete Branch</button>
										<button type="button" class="btn btn-sm btn-success btn-right-align" ng-click="loadCommits()">Scan Branch</button>
									</div>
								</div>
							</div>
							<div class="card-body">
								<uc:commits runat="server" />
							</div>
						</div>
					</div>
					<div id="taskbuilds" class="tab-pane fade">
						<div class="card">
							<div class="card-header bg-info">
								<div class="row">
									<div class="col-md-4">
										<span class="glyphicon glyphicon-time"></span>
										<label>Builds history (last 5 builds)</label>
									</div>
									<div class="col-md-4 row">
										<label class="col-sm-6 control-label text-right">Test Priority:</label>
										<div class="col-sm-6">
											<select class="form-control form-control-sm" ng-disabled="!canChangeDefect()" ng-model="defect.TESTPRIORITY">
												<option value="{{t.ID}}" ng-repeat="t in buildpriorities">{{t.DESCR}}</option>
											</select>
										</div>
									</div>
									<div class="col-md-2">
										<button type="button" class="btn btn-sm btn-success btn-right-align" ng-disabled="!canBuild()" ng-click="testTask()">Build Version</button>
									</div>
									<div class="col-md-2">
										<button type="button" class="btn btn-sm btn-danger btn-right-align" ng-disabled="!canBuild()" ng-click="abortTest()">Abort Building</button>
									</div>
								</div>
							</div>
							<div class="card-body">
								<label ng-show="!builds">loading...</label>
								<div class="list-group">
									<a href="getBuildLog.ashx?id={{b.ID}}" class="list-group-item" ng-repeat="b in builds" target="_blank" style="background-color: {{b.COLOR}}">
										<div class="row">
											<div class="col-sm-2">
												<span class="glyphicon glyphicon-time"></span><span>{{b.DATE}}</span>
											</div>
											<div class="col-sm-3">
												<div class="progress" ng-show="b.STATUS.includes('Building')==true">
													<div class="progress-bar progress-bar-striped active" role="progressbar" style="width: {{b.PERCENT}}%">
														{{b.STATUSTXT?b.STATUSTXT:b.STATUS}}
													</div>
												</div>
												<div class="progress" ng-show="b.STATUS.includes('wait')==true">
													<div class="progress-bar progress-bar-striped active progress-bar-warning" role="progressbar" style="width: 100%">
														{{b.STATUS}}...
													</div>
												</div>
												<span style="color: red" class="glyphicon glyphicon-remove-circle" ng-show="b.STATUS.includes('FAILED')==true"></span>
												<span class="glyphicon glyphicon-pushpin" ng-show="b.STATUS.includes('Cancel')==true"></span>
												<span style="color: green" class="glyphicon glyphicon-ok-circle" ng-show="b.STATUS.includes('OK')==true"></span>
												<span ng-show="b.STATUS.includes('Building')==false&&b.STATUS.includes('wait')==false">{{b.STATUS}}</span>
											</div>
											<div class="col-sm-2">
												<span class="glyphicon glyphicon-comment"></span><span>{{b.NOTES}}</span>
											</div>
											<div class="col-sm-2">
												<span class="glyphicon glyphicon-blackboard"></span><span>{{b.MACHINE}}&nbsp({{b.DURATION}} min)</span>
											</div>
											<div class="col-sm-3">
												<span class="glyphicon glyphicon-fire"></span>
												<span>{{b.DATEUP}}</span>
											</div>
										</div>
									</a>
								</div>
							</div>
							<div ng-show="!commits||commits.length<1" class="panel-footer"><strong>Info!</strong> Please commit your changes to git and push your branch named with TTxxxxxx where xxxxxx is the task number.</div>
						</div>
					</div>
					<div id="alarm" class="tab-pane fade">
						<div class="jumbotron">
							<h3>Email will alarm all the persons indicated below</h3>
							<h4 class="{{changed ? 'blink_me' : ''}}">Please save the task indicating your questions in the top of details section</h4>
							<label for="emailaddr">Addresses (comma separated):</label>
							<input type="text" class="form-control" id="emailaddr" ng-model="addresses">
							<button ng-click="sendEmail()" ng-disabled="changed" type="button" class="btn btn-primary">Send Alarm Email</button>
						</div>
					</div>
				</div>
			</div>
			<div class="col-lg-2 hidden-md">
				<div class="alert alert-warning" style="text-align: center">
					<a data-toggle="tooltip" title="Click to see full plan for the person" target="_blank" href="editplan.aspx?userid={{defect.CREATEDBY | getUserTRIDById:this}}">
						<img class="rounded-circle" ng-src="{{'getUserImg.ashx?ttid=' + defect.CREATEDBY}}" alt="Smile" height="60" width="60" />
						<div>
							<strong>{{defect.CREATEDBY | getUserById:this}}</strong>
						</div>
					</a>
					<i class="fas fa-long-arrow-alt-down"></i>
				</div>
				<div class="alert alert-info" style="text-align: center">
					<a data-toggle="tooltip" title="Click to see full plan for the person" target="_blank" href="editplan.aspx?userid={{defect.AUSER | getUserTRIDById:this}}">
						<img class="rounded-circle" ng-src="{{'getUserImg.ashx?ttid=' + defect.AUSER}}" alt="Smile" height="60" width="60" />
						<div>
							<strong>{{defect.AUSER | getUserById:this}}</strong>
						</div>
					</a>
				</div>
			</div>
		</div>
	</div>
</asp:Content>
