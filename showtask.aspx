<%@ Page Title="Task" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="showtask.aspx.cs" Inherits="Showtask" %>

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

	<div ng-app="mpsapplication" ng-controller="mpscontroller" ng-cloak>
		<div class="alert alert-danger savebutton btn-group-vertical" ng-cloak ng-show="changed">
			<button type="button" class="btn btn-lg btn-info" ng-click="saveDefect()">Save</button>
			<button type="button" class="btn btn-lg btn-danger" ng-click="discardDefect()">Discard</button>
		</div>
		<div class="row">
			<div class="col-lg-2 hidden-md">
			</div>
			<div class="col-lg-8 col-md-12">
				<label for="summary">TT{{defect.ID}} {{defect.SUMMARY}}</label>
				<button title="Copy task label to clipboard" data-toggle="tooltip" type="button" class="btn btn-default btn-sm" style="float: right" ng-click="cliplabl()">
					<span class="glyphicon glyphicon-copy"></span>
				</button>
				<input type="text" class="form-control" id="summary" ng-disabled="!canChangeDefect()" ng-model="defect.SUMMARY">
				<div class="hidden-xs row toolbar" ng-style="defect.DISPO | getDispoColorById:this">
					<div class="col-sm-3">
						<div class="row">
							<div class="col-sm-3">
								<h6><a href="types.aspx" target="_blank">Type:</a></h6>
							</div>
							<div class="col-sm-9">
								<select class="form-control input-sm" id="type" ng-disabled="!canChangeDefect()" ng-model="defect.TYPE">
									<option value="{{t.ID}}" ng-repeat="t in types">{{t.DESCR}}</option>
								</select>
							</div>
						</div>
						<div class="row">
							<div class="col-sm-3">
								<h6><a href="products.aspx" target="_blank">Product:</a></h6>
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
								<h6><a href="dispositions.aspx" target="_blank">Disposition:</a></h6>
							</div>
							<div class="col-sm-9">
								<select class="form-control input-sm" id="dispo" ng-disabled="!canChangeDefect()" ng-model="defect.DISPO">
									<option ng-repeat="d in dispos" value="{{d.ID}}" style="background-color: {{d .COLOR}}">{{d.DESCR}}</option>
								</select>
							</div>
						</div>
						<div class="row">
							<div class="col-sm-3">
								<h6><a href="priorities.aspx" target="_blank">Priorities:</a></h6>
							</div>
							<div class="col-sm-9">
								<select class="form-control input-sm" id="prio" ng-disabled="!canChangeDefect()" ng-model="defect.PRIO">
									<option ng-repeat="p in priorities" value="{{p.ID}}">{{p.DESCR}}</option>
								</select>
							</div>
						</div>
						<div class="row">
							<div class="col-sm-3">
								<h6><a href="components.aspx" target="_blank">Component:</a></h6>
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
								<h6><a href="severities.aspx" target="_blank">Severity:</a></h6>
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
									<option ng-repeat="u in users" ng-show="u.ACTIVE" value="{{u.ID}}">{{u.FULLNAME}}</option>
								</select>
							</div>
						</div>
					</div>
					<div class="col-sm-3">
						<div class="row">
							<div class="col-sm-3">
								<h6><a href="editplan.aspx?userid={{defect.AUSER | getUserTRIDById:this}}" target="_blank">Assigned:</a></h6>
							</div>
							<div class="col-sm-9">
								<select class="form-control input-sm" id="auser" ng-disabled="!canChangeDefect()" ng-model="defect.AUSER">
									<option ng-repeat="u in users" ng-show="u.ACTIVE" value="{{u.ID}}">{{u.FULLNAME}}</option>
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
				<ul id="tasktabs" class="nav nav-pills">
					<li class="{{specsStyle()}} small active"><a data-toggle="pill" href="#specification"><span class="glyphicon glyphicon-list-alt"></span>{{tab_specs}}</a></li>
					<li><a class="small" data-toggle="pill" href="#detail"><span class="glyphicon glyphicon-zoom-in"></span>&nbsp;Details</a></li>
					<li ng-click="changetab($event)"><a class="small" data-toggle="pill" href="#bst"><span class="glyphicon glyphicon-link"></span>{{tab_bst}}</a></li>
					<li ng-click="changetab($event)"><a class="small" data-toggle="pill" href="#workflow"><span class="glyphicon glyphicon-refresh"></span>{{tab_workflow}}</a></li>
					<li ng-click="changetab($event)"><a class="small" data-toggle="pill" href="#history"><span class="glyphicon glyphicon-book"></span>{{tab_history}}</a></li>
					<li ng-click="changetab($event)"><a class="small" data-toggle="pill" href="#attachments"><span class="glyphicon glyphicon-paperclip"></span>{{tab_attachs}}</a></li>
					<li><a class="small" data-toggle="pill" href="#lockinfo"><span class="glyphicon glyphicon-lock"></span>&nbsp;Lock Info</a></li>
					<li ng-click="changetab($event)" id="buildstab"><a class="small" data-toggle="pill" href="#taskbuilds"><span class="glyphicon glyphicon-wrench"></span>{{tab_builds}}</a></li>
					<li><a class="small" data-toggle="pill" href="#alarm"><span class="glyphicon glyphicon-envelope"></span>&nbsp;Alarm</a></li>
				</ul>
				<div class="tab-content">
					<div id="specification" class="tab-pane fade in active">
						<textarea class="form-control" id="spec" rows="30" ng-disabled="!canChangeDefect()" ng-model="defect.SPECS"></textarea>
					</div>
					<div id="detail" class="tab-pane fade">
						<textarea class="form-control" id="Description" rows="30" ng-disabled="!canChangeDefect()" ng-model="defect.DESCR"></textarea>
					</div>
					<div id="bst" class="tab-pane fade">
						<div class="row">
							<div class="col-md-3">
								<input class="form-control" ng-model="batchsearch" type="text"/>
								<ul class="nav nav-pills nav-justified">
									<li class="{{$index==0?'active':''}}" ng-repeat="s in batchesslots"><a class="tab-small" data-toggle="tab" href="#batches{{$index}}">{{$index+1}}</a></li>
								</ul>
								<div class="tab-content">
									<div id="batches{{$index}}" class="tab-pane fade {{$index==0?'in active':''}}" ng-repeat="s in batchesslots">
										<div class="list-group">
											<a href="#" ng-click="add2Bst(batch)" class="batch-item list-group-item" ng-repeat="batch in s">{{batch}}</a>
										</div>
									</div>
								</div>
							</div>
							<div class="col-md-9">
								<ul id="bsttabs" class="nav nav-pills">
									<li id="{{bsttab_bat}}" class="active"><a class="tab-small" data-toggle="tab" href="#batches">Batches<span ng-show="defect.BSTBATCHES !== ''" class="badge">{{defect.BSTBATCHES.split("\n").length}}</span></a></li>
									<li id="{{bsttab_com}}"><a class="tab-small" data-toggle="tab" href="#commands">Commands<span ng-show="defect.BSTCOMMANDS !== ''" class="badge">{{defect.BSTCOMMANDS.split("\n").length}}</span></a></li>
									<li id="{{bsttab_his}}"><a class="tab-small" data-toggle="tab" href="#bsthistory">History<span ng-show="builds.length > 0" class="badge">{{builds.length}}</span></a></li>
								</ul>
								<div class="tab-content">
									<div id="batches" class="tab-pane fade in active">
										<textarea id="bstbatches" class="form-control" rows="28" ng-disabled="!canChangeDefect()" ng-model="defect.BSTBATCHES"></textarea>
									</div>
									<div id="commands" class="tab-pane fade">
										<textarea id="bstcommands" class="form-control" rows="28" ng-disabled="!canChangeDefect()" ng-model="defect.BSTCOMMANDS"></textarea>
									</div>
									<div id="bsthistory" class="tab-pane fade">
										<a ng-repeat="b in builds" href ng-click="showTests(b.TESTGUID)" class="btn btn-block btn-info">{{b.DATEUP}}</a>
									</div>
								</div>
							</div>
						</div>
					</div>
					<div id="workflow" class="tab-pane fade">
						<label ng-show="!events">loading...</label>
						<div class="list-group">
							<a href="#" class="list-group-item" ng-repeat="h in events | orderBy : 'ORDER'">
								<div class="col-sm-3">
									<img class="img-circle" ng-src="{{'getUserImg.ashx?ttid=' + h.IDUSER}}" alt="Smile" height="20" width="20">
									<b>{{h.IDUSER | getUserById:this}}</b>
								</div>
								<div class="col-sm-2">
									<span class="label label-info">{{h.DATE}}</span>
								</div>
								<div class="col-sm-1">
									{{h.EVENT}}
								</div>
								<div class="col-sm-3">
									<img class="img-circle" ng-show="h.ASSIGNUSERID > 0" ng-src="{{'getUserImg.ashx?ttid=' + h.ASSIGNUSERID}}" alt="Smile" height="20" width="20"></img>
									<b>{{h.ASSIGNUSERID | getUserById:this}}</b> <span class="badge">{{h.TIME}}</span>
								</div>
								{{h.NOTES}}	&nbsp;
							</a>
						</div>
					</div>
					<div id="history" class="tab-pane fade">
						<label ng-show="!history">loading...</label>
						<div class="list-group">
							<a href="#" class="list-group-item" ng-repeat="h in history">
								<div class="col-sm-3">
									<img class="img-circle" ng-src="{{'getUserImg.ashx?ttid=' + h.IDUSER}}" alt="Smile" height="20" width="20">
									<b>{{h.IDUSER | getUserById:this}}</b>
								</div>
								<div class="col-sm-2">
									<span class="label label-info">{{h.DATE}}</span>
								</div>
								<div class="col-sm-6">
									<span>{{h.NOTES}}</span>
								</div>
								<span>&nbsp</span>
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
									<img src="getAttachImg.ashx?idrecord={{a.ID+'&ext='+getfileext(a.FILENAME)}}" style="max-width: 100%" />
								</a>&nbsp
						<button ng-click="deleteAttach(a.ID)" type="button" class="btn btn-danger btn-xs">Delete</button>
							</li>
						</ul>
					</div>
					<div id="lockinfo" class="tab-pane fade">
						<div class="row">
							<span class="label label-info">{{getMPSUserName(lockedby)}}</span>
							<img class="img-circle" ng-src="{{'getUserImg.ashx?id=' + lockedby}}" alt="Smile" height="42" width="42">
						</div>
						<div class="row">
							<button type="button" ng-click="releaseRequest()" class="btn btn-primary btn-xs">Request to release!</button>
						</div>
					</div>
					<div id="taskbuilds" class="tab-pane fade">
						<div class="panel panel-info">
							<div class="panel-heading">
								<div class="row">
									<div class="col-md-4">
										<span class="glyphicon glyphicon-time"></span>
										<label>Builds history (last 5 builds)</label>
									</div>
									<div class="col-md-4">
										<label class="col-sm-6 control-label text-right">Test Priority:</label>
										<div class="col-sm-6">
											<select class="form-control input-sm" ng-disabled="!canChangeDefect()" ng-model="defect.TESTPRIORITY">
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
							<div class="panel-body">
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
						<div class="panel panel-primary">
							<div class="panel-heading">
								<div class="row">
									<div class="col-md-3">
										<label>Git branch (default is TT ID):</label>
									</div>
									<div class="col-md-2">
										<input type="text" class="form-control" ng-disabled="!canChangeDefect()" ng-model="defect.BRANCH">
									</div>
									<div class="col-md-3">
										<a href="builder.aspx" ng-show="isrelease()" class="btn btn-info" role="button">Release Marker</a>
									</div>
									<div class="col-md-3">
										<button type="button" class="btn btn-sm btn-danger btn-right-align" ng-disabled="!commits||commits.length<1" ng-click="deleteBranch()">Delete Branch</button>
									</div>
								</div>
							</div>
							<div class="panel-body">
								<label ng-show="!commits">loading...</label>
								<div class="list-group">
									<a href="#" class="list-group-item" ng-repeat="c in commits">
										<div class="row">
											<div class="col-sm-3">
												<span>{{c.DATE}}</span>
											</div>
											<div class="col-sm-2">
												<span>{{c.AUTHOR}}</span>
											</div>
											<div class="col-sm-4">
												<span>{{c.COMMIT}}</span>
											</div>
											<div class="col-sm-3">
												<span>{{c.NOTES}}</span>
											</div>
										</div>
									</a>
								</div>
							</div>
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
					<img class="img-circle" ng-src="{{'getUserImg.ashx?ttid=' + defect.CREATEDBY}}" alt="Smile" height="60" width="60" />
					<div>
						<strong>{{defect.CREATEDBY | getUserById:this}}</strong>
					</div>
					<div>
						<span class="glyphicon glyphicon-arrow-down"></span>
					</div>
				</div>
				<div class="alert alert-info" style="text-align: center">
					<img class="img-circle" ng-src="{{'getUserImg.ashx?ttid=' + defect.AUSER}}" alt="Smile" height="60" width="60" />
					<div>
						<strong>{{defect.AUSER | getUserById:this}}</strong>
					</div>
				</div>
			</div>
		</div>
	</div>
</asp:Content>
