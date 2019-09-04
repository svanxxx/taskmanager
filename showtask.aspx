<%@ Page Title="Task" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="showtask.aspx.cs" Inherits="Showtask" %>

<%@ Register Src="~/controls/CommitsControl.ascx" TagName="commits" TagPrefix="uc" %>
<%@ Register Src="~/controls/BuildsControl.ascx" TagName="builds" TagPrefix="uc" %>

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
		<button ng-show="false" type="submit">for autocomplete</button>
		<div class="row">
			<div class="col-lg-2">
				<div class="btn-group-vertical btn-block" ng-cloak ng-show="changed">
					<button type="button" class="btn btn-success" ng-click="saveDefect()">Save</button>
					<button type="button" class="btn btn-danger" ng-click="discardDefect()">Discard</button>
				</div>
				<button onclick="copyurl()" type="button" class="btn btn-outline-secondary btn-block btn-sm">Copy link&nbsp;<i class="fas fa-link"></i></button>
				<button ng-click="duplicate()" type="button" class="btn btn-outline-secondary btn-block btn-sm">Duplicate&nbsp;<i class="fas fa-clone"></i></button>
				<button ng-click="resettask()" ng-disabled="!canChangeDefect()" type="button" class="btn btn-outline-secondary btn-block btn-sm">Reset To Re-Use&nbsp;<i class="fas fa-recycle"></i></button>
				<button ng-click="normtext()" ng-disabled="!canChangeDefect()" type="button" class="btn btn-outline-secondary btn-block btn-sm">Normalize Details Text&nbsp;<i class="fab fa-wpforms"></i></button>
				<button ng-click="adddesc('Tested', 'taskokay.png')" ng-disabled="!canChangeDefect()" type="button" class="btn btn-outline-secondary btn-block btn-sm">Add Tested Comment&nbsp;<i class="fas fa-check"></i></button>
				<button ng-click="adddesc('Rejected', 'taskfail.png')" ng-disabled="!canChangeDefect()" type="button" class="btn btn-outline-secondary btn-block btn-sm">Add Rejected Comment&nbsp;<i class="fas fa-window-close"></i></button>
				<div class="alert alert-info mt-2" style="text-align: center">
					<button data-toggle="tooltip" title="Ask to release!" type="button" class="btn btn-light btn-sm float-left" ng-click="releaseRequest()"><i class="fas fa-bell text-info"></i></button>
					<button data-toggle="tooltip" title="FORCE to release!" type="button" class="btn btn-light btn-sm float-right" ng-click="releaseForce()"><i class="fas fa-jedi text-danger"></i></button>
					<a data-toggle="tooltip" title="Click to see full plan for the person" target="_blank" href="editplan.aspx?userid={{lockedby}}">
						<img class="rounded-circle" ng-src="{{'getUserImg.ashx?sz=60&id=' + lockedby}}" alt="Smile" height="60" width="60" />
						<div>
							<strong>Locked by: {{getMPSUserName(lockedby)}}</strong>
						</div>
					</a>
					<i class="fas fa-unlock-alt"></i>
				</div>
				<div ng-show="defect.FIRE" id="firealarm" class="alert alert-info mt-2" style="text-align: center; cursor: pointer" ng-click="gotoAlarm()">
					<strong>The task is on fire<br />
						It is urgently requested<br />
						You can check alarm tab for deadline</strong>
				</div>
				<div class="alert alert-success mt-2" style="text-align: center" ng-show="commented">
					<div class="custom-control custom-switch">
						<input type="checkbox" class="custom-control-input" id="switchAusr" ng-model="commented_alarmuser">
						<label class="custom-control-label" for="switchAusr">Alarm {{defect.AUSER | getUserById:this}}</label>
					</div>
					<div class="custom-control custom-switch">
						<input type="checkbox" class="custom-control-input" id="switchAGrp" ng-model="commented_alarmgroup">
						<label class="custom-control-label" for="switchAGrp">Alarm Team</label>
					</div>
				</div>
			</div>
			<div class="col-lg-8">
				<div class="input-group input-group-sm mb-1">
					<div class="input-group-prepend">
						<button title="Copy task label to clipboard" data-toggle="tooltip" class="btn btn-secondary btn-sm pr-1" type="button" ng-click="cliplabl()">TT{{defect.ID}}</button>
					</div>
					<input title="Task summary" data-toggle="tooltip" type="text" class="form-control" ng-disabled="!canChangeDefect()" ng-change="updateDefSum()" ng-model="defectsumm">
					<input list="tasksourcelist" type="text" name="tasksource" id="tasksource" autocomplete="on" title="Task source: e.g. email subject" data-toggle="tooltip" class="form-control" ng-disabled="!canChangeDefect()" ng-change="updateDefEml()" ng-model="defecteml">
					<datalist id="tasksourcelist">
						<option ng-repeat="ts in tasksources">{{ts}}</option>
					</datalist>
				</div>
				<div class="hidden-xs row toolbar mb-1" ng-style="defect.DISPO | getDispoColorById:this">
					<div class="col-sm-3 pl-0">
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
							<div class="input-group-prepend w-75">
								<div class="dropdown w-100">
									<button type="button" class="overflow-hidden w-100 btn btn-light btn-sm dropdown-toggle" data-toggle="dropdown" ng-disabled="!canChangeDefect()">
										<img async class="rounded-circle" ng-src="{{'getUserImg.ashx?sz=15&ttid=' + defect.CREATEDBY}}" alt="Smile" height="15" width="15">
										{{defect.CREATEDBY | getUserById:this}}
									</button>
									<div class="dropdown-menu w-100">
										<div ng-show="u.ACTIVE" ng-repeat="u in users | orderBy:'FULLNAME'">
											<button style="text-align: left" type="button" class="btn btn-light btn-block btn-sm" ng-click="defect.CREATEDBY = u.ID">
												<img async class="rounded-circle" ng-src="{{'getUserImg.ashx?sz=30&ttid=' + u.ID}}" alt="Smile" height="30" width="30">
												<span>{{u.FULLNAME}}</span>
											</button>
										</div>
									</div>
								</div>
							</div>
						</div>
					</div>
					<div class="col-sm-3 pr-0">
						<div class="input-group input-group-sm">
							<div class="input-group-prepend w-25">
								<span class="input-group-text w-100"><a href="editplan.aspx?userid={{defect.AUSER | getUserTRIDById:this}}" target="_blank">Assigned</a></span>
							</div>
							<div class="input-group-prepend w-75">
								<div class="dropdown w-100">
									<button type="button" class="overflow-hidden w-100 btn btn-light btn-sm dropdown-toggle" data-toggle="dropdown" ng-disabled="!canChangeDefect()">
										<img async class="rounded-circle" ng-src="{{'getUserImg.ashx?sz=15&ttid=' + defect.AUSER}}" alt="Smile" height="15" width="15">
										{{defect.AUSER | getUserById:this}}
									</button>
									<div class="dropdown-menu w-100">
										<div ng-show="u.ACTIVE" ng-repeat="u in users | orderBy:'FULLNAME'">
											<button style="text-align: left" type="button" class="btn btn-light btn-block btn-sm" ng-click="defect.AUSER = u.ID">
												<img async class="rounded-circle" ng-src="{{'getUserImg.ashx?sz=30&ttid=' + u.ID}}" alt="Smile" height="30" width="30">
												<span>{{u.FULLNAME}}</span>
											</button>
										</div>
									</div>
								</div>
							</div>
						</div>
						<div class="input-group input-group-sm">
							<div class="input-group-prepend w-25">
								<span class="input-group-text w-100">Effort</span>
							</div>
							<input type="number" min="1" max="999" id="estim" class="form-control w-25 pr-0" ng-disabled="!canChangeDefect()" ng-model="defect.ESTIM">
							<div class="input-group-prepend w-50">
								<div class="dropdown w-100">
									<button type="button" class="overflow-hidden w-100 btn btn-light btn-sm dropdown-toggle" data-toggle="dropdown" ng-disabled="!canChangeDefect()">
										<img async class="rounded-circle" ng-src="{{'getUserImg.ashx?sz=15&ttid=' + defect.ESTIMBY}}" alt="Smile" height="15" width="15">
										{{defect.ESTIMBY | getUserById:this}}
									</button>
									<div class="dropdown-menu w-100">
										<div ng-show="u.ACTIVE" ng-repeat="u in users | orderBy:'FULLNAME'">
											<button style="text-align: left" type="button" class="btn btn-light btn-block btn-sm" ng-click="defect.ESTIMBY = u.ID">
												<img async class="rounded-circle" ng-src="{{'getUserImg.ashx?sz=30&ttid=' + u.ID}}" alt="Smile" height="30" width="30">
												<span>{{u.FULLNAME}}</span>
											</button>
										</div>
									</div>
								</div>
							</div>
						</div>
						<div class="input-group input-group-sm">
							<div class="input-group-prepend w-25">
								<span class="input-group-text w-100">Order</span>
							</div>
							<input type="number" min="0" id="order" class="form-control" onkeydown="allowPosNumbers(event)" ng-disabled="!canChangeDefect()" ng-model="defect.ORDER">
							<button ng-disabled="!canChangeDefect()" type="button" class="bnt btn-outline-default" ng-click="chgOrder(false)"><i class="fas fa-arrow-up"></i></button>
							<button ng-disabled="!canChangeDefect()" type="button" class="bnt btn-outline-default" ng-click="chgOrder(true)"><i class="fas fa-arrow-down"></i></button>
							<button ng-disabled="!canChangeDefect()" type="button" class="bnt btn-outline-default" ng-click="chgOrder(undefined)"><i class="fas fa-times"></i></button>
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
						<div class="input-group input-group-sm mb-1">
							<div class="input-group-prepend">
								<span class="input-group-text">BST Branch:</span>
							</div>
							<input type="text" class="form-control" ng-disabled="!canChangeDefect()" ng-model="defect.BRANCHBST">
						</div>
						<div class="row">
							<div class="col-md-3 pr-0">
								<div class="input-group input-group-sm">
									<div class="input-group-prepend">
										<span class="input-group-text"><i class="fas fa-search"></i></span>
									</div>
									<input class="form-control" ng-model="batchsearch" type="text" />
								</div>
								<ul class="nav nav-pills nav-justified">
									<li class="nav-item" ng-repeat="s in batchesslots">
										<a data-toggle="pill" class="nav-link p-0 {{$index==0?'active':''}}" href="#batches{{$index}}">{{$index+1}}</a>
									</li>
								</ul>
								<div class="tab-content">
									<div id="batches{{$index}}" class="tab-pane {{$index==0?'active':'fade'}}" ng-repeat="s in batchesslots">
										<div class="list-group">
											<a href="#" ng-click="add2Bst(batch)" class="list-group-item py-1" ng-repeat="batch in s">{{batch}}</a>
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
										<textarea id="bstbatches" class="form-control form-control-sm" rows="27" ng-disabled="!canChangeDefect()" ng-model="defect.BSTBATCHES"></textarea>
									</div>
									<div id="commands" class="tab-pane fade">
										<textarea id="bstcommands" class="form-control form-control-sm" rows="27" ng-disabled="!canChangeDefect()" ng-model="defect.BSTCOMMANDS"></textarea>
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
										<img class="rounded-circle" ng-src="{{'getUserImg.ashx?sz=30&ttid=' + h.IDUSER}}" alt="Smile" height="30" width="30">
									</div>
									<div class="media-body">
										<h6 class="media-heading"><b>{{h.IDUSER | getUserById:this}}</b> <small style="float: right"><i>{{h.DATE}}</i></small></h6>
										<p>{{h.NOTES}}</p>
									</div>
								</div>
								{{h.EVENT}}:
								<img ng-show="h.ASSIGNUSERID > 0" class="rounded-circle" ng-src="{{'getUserImg.ashx?sz=20&ttid=' + h.ASSIGNUSERID}}" alt="Smile" height="20" width="20" />
								{{h.ASSIGNUSERID | getUserById:this}} <span class="badge {{h.EVENT == 'worked' ? 'badge-warning' : 'badge-danger'}}">{{h.TIME}}</span>
								{{h.NOTES}}	&nbsp;
							</a>
						</div>
					</div>
					<div id="history" class="tab-pane fade">
						<label ng-show="!history">loading...</label>
						<div class="list-group">
							<a href class="p-2 list-group-item list-group-item-action" ng-repeat="h in history">
								<div class="media">
									<div class="media-left">
										<img class="media-object rounded-circle" ng-src="{{'getUserImg.ashx?sz=30&ttid=' + h.IDUSER}}" alt="Smile" height="30" width="30">
									</div>
									<div class="media-body">
										<h6 class="media-heading"><b>{{h.IDUSER | getUserById:this}}</b> <small style="float: right"><i>{{h.DATE}}</i></small></h6>
										<p class="mb-0">{{h.NOTES}}</p>
									</div>
								</div>
							</a>
						</div>
					</div>
					<div id="attachments" class="tab-pane fade">
						<label ng-show="!attachs">loading...</label>
						<div class="d-flex justify-content-end">
							<div class="alert alert-light mb-0 pb-0">
								You can paste images from clipboard right here or click add file button.
							</div>
							<button type="button" ng-disabled="!canChangeDefect()" ng-click="addFile()" id="button" class="btn btn-outline-secondary btn-xs">Add File...</button>
						</div>
						<ul>
							<li ng-style="a.deleted ? {'text-decoration':'line-through'} : ''" ng-repeat="a in attachs">
								<a target="_blank" href="getTaskAttachment.ashx?idrecord={{a.ID+'&ext='+getfileext(a.FILENAME)}}">
									<img ng-src="getAttachImg.ashx?idrecord={{a.ID+'&ext='+getfileext(a.FILENAME)}}" style="max-width: 100%" />
									<img ng-src="{{a.newblob}}" style="max-width: 100%" />
									<span>{{a.FILENAME}}</span>
								</a>&nbsp
								<button ng-disabled="!canChangeDefect()" ng-click="deleteAttach(a.ID)" type="button" class="btn btn-danger btn-xs">Delete</button>
							</li>
						</ul>
					</div>
					<div id="taskgit" class="tab-pane fade">
						<ul class="nav nav-tabs" role="tablist">
							<li class="nav-item">
								<a class="nav-link active small" data-toggle="tab" href="#localbranch">Task branch</a>
							</li>
							<li class="nav-item">
								<a class="nav-link small" data-toggle="tab" href="#masterbranch">Master</a>
							</li>
						</ul>
						<div class="tab-content">
							<div id="localbranch" class="container tab-pane active">
								<div class="d-flex mb-1">
									<div class="input-group input-group-sm">
										<div class="input-group-prepend">
											<span class="input-group-text">Branch:</span>
										</div>
										<input type="text" class="form-control" ng-disabled="!canChangeDefect()" ng-model="defect.BRANCH">
										<div class="input-group-append">
											<button type="button" ng-show="!isrelease()" class="btn btn-success" ng-click="loadCommits()">Scan Branch</button>
											<button type="button" ng-show="!isrelease()" class="btn btn-danger" ng-disabled="!gitbranchhash" ng-click="deleteBranch()">Delete Branch</button>
											<a href="merger.aspx?branch={{defect.BRANCH}}&ttid={{defect.ID}}" ng-show="!isrelease()" class="btn btn-sm btn-outline-dark" ng-disabled="!gitbranchhash"><i class="fas fa-file-export"></i><i class="fab fa-joomla"></i></a>
											<a href="builder.aspx" ng-show="isrelease()" class="btn btn-secondary" role="button">Release Marker</a>
										</div>
									</div>
								</div>
								<uc:commits runat="server" />
							</div>
							<div id="masterbranch" class="container tab-pane fade">
								<button ng-click="loadMasterCommits()" type="button" class="btn btn-outline-dark btn-sm">Query Mater Branch</button>
								<uc:commits dataset="mastercommits" hide="" runat="server" />
							</div>
						</div>
					</div>
					<div id="taskbuilds" class="tab-pane fade">
						<div class="input-group input-group-sm mb-1">
							<div class="input-group-prepend">
								<span class="input-group-text">Test Priority</span>
							</div>
							<select class="form-control form-control-sm" ng-disabled="!canChangeDefect()" ng-model="defect.TESTPRIORITY">
								<option value="{{t.ID}}" ng-repeat="t in buildpriorities">{{t.DESCR}}</option>
							</select>
							<button type="button" class="btn btn-sm btn-success btn-right-align" ng-disabled="!canBuild()" ng-click="testTask()">Build Version</button>
							<button type="button" class="btn btn-sm btn-danger btn-right-align" ng-disabled="!canBuild()" ng-click="abortTest()">Abort Building</button>
						</div>
						<uc:builds runat="server" />
						<div ng-show="!commits||commits.length<1" class="panel-footer"><strong>Info!</strong> Please commit your changes to git and push your branch named with TTxxxxxx where xxxxxx is the task number.</div>
					</div>
					<div id="alarm" class="tab-pane fade">
						<div class="jumbotron">
							<h5><i class="far fa-comment"></i>&nbsp;&nbsp;&nbsp;In order to receive messages from the system you have to:</h5>
							<a class="btn btn-outline-secondary btn-sm" href="<%=Settings.CurrentSettings.TELEGRAMTASKSURL.ToString()%>">Subscribe To Telegram Tasks Bot</a>
							<hr />
							<h5><i class="fas fa-hourglass-start"></i>&nbsp;&nbsp;&nbsp;Set Fire alarm timer:</h5>
							<div class="input-group input-group-sm">
								<div class="input-group-prepend">
									<span class="input-group-text">Date time:</span>
								</div>
								<input type="date" class="form-control" ng-disabled="!canChangeDefect()" ng-model="defect.TIMER">
								<div class="input-group-append">
									<button type="button" class="btn btn-sm btn-outline-secondary" ng-disabled="!canChangeDefect()" ng-click="defect.TIMER = today"><i class="fas fa-check"></i></button>
									<button type="button" class="btn btn-sm btn-outline-secondary" ng-disabled="!canChangeDefect()" ng-click="defect.TIMER = null"><i class="fas fa-times"></i></button>
								</div>
							</div>
							<hr />
							<h3>Email will alarm all the persons indicated below</h3>
							<h4 class="{{changed ? 'blink_me' : ''}}">Please save the task indicating your questions in the top of details section</h4>
							<label for="emailaddr">Addresses (comma separated):</label>
							<input type="text" class="form-control" id="emailaddr" ng-model="addresses">
							<button ng-click="sendEmail()" ng-disabled="changed" type="button" class="btn btn-primary">Send Alarm Email</button>
						</div>
					</div>
				</div>
			</div>
			<div class="col-lg-2">
				<div class="alert alert-warning" style="text-align: center">
					<button data-toggle="tooltip" title="Invite person to see this task." type="button" class="btn btn-light btn-sm float-right" ng-click="invite(defect.CREATEDBY)"><i class="fas fa-bell"></i></button>
					<a data-toggle="tooltip" title="Click to see full plan for the person" target="_blank" href="editplan.aspx?userid={{defect.CREATEDBY | getUserTRIDById:this}}">
						<img class="rounded-circle" ng-src="{{'getUserImg.ashx?sz=60&ttid=' + defect.CREATEDBY}}" alt="Smile" height="60" width="60" />
						<div>
							<strong>{{defect.CREATEDBY | getUserById:this}}</strong>
						</div>
					</a>
					<i class="fas fa-folder-plus"></i>
				</div>
				<div class="alert alert-danger" style="text-align: center">
					<button data-toggle="tooltip" title="Invite person to see this task." type="button" class="btn btn-light btn-sm float-right" ng-click="invite(defect.ESTIMBY)"><i class="fas fa-bell"></i></button>
					<a data-toggle="tooltip" title="Click to see full plan for the person" target="_blank" href="editplan.aspx?userid={{defect.ESTIMBY | getUserTRIDById:this}}">
						<img class="rounded-circle" ng-src="{{'getUserImg.ashx?sz=60&ttid=' + defect.ESTIMBY}}" alt="Smile" height="60" width="60" />
						<div>
							<strong>{{defect.ESTIMBY | getUserById:this}}</strong>
						</div>
					</a>
					<i class="far fa-clock"></i><span>:{{defect.ESTIM}}</span>
				</div>
				<div class="alert alert-info" style="text-align: center">
					<button data-toggle="tooltip" title="Invite person to see this task." type="button" class="btn btn-light btn-sm float-right" ng-click="invite(defect.AUSER)"><i class="fas fa-bell"></i></button>
					<a data-toggle="tooltip" title="Click to see full plan for the person" target="_blank" href="editplan.aspx?userid={{defect.AUSER | getUserTRIDById:this}}">
						<img class="rounded-circle" ng-src="{{'getUserImg.ashx?sz=60&ttid=' + defect.AUSER}}" alt="Smile" height="60" width="60" />
						<div>
							<strong>{{defect.AUSER | getUserById:this}}</strong>
						</div>
					</a>
					<i class="fas fa-tools"></i>
				</div>
				<div class="alert alert-warning" style="text-align: center">
					<span>Worked on: {{defect.SPENT}} hours</span>
				</div>
			</div>
		</div>
	</div>
</asp:Content>
