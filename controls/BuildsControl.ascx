<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BuildsControl.ascx.cs" Inherits="BuildsControl" %>

<%@ Register Src="~/controls/PagerControl.ascx" TagName="pagercontrol" TagPrefix="uc" %>

<label ng-show="!builds">Loading build information, please wait...</label>
<div class="card" ng-cloak>
	<div class="card-header p-1 d-flex">
		<div class="mt-1 mr-auto">
			<h5 class="ml-2" style="display: inline"><i class="fab fa-linode"></i></h5>
			<button type="button" class="btn btn-outline-light text-dark btn-sm" onclick="window.location='builds.aspx'">Builds History</button>
		</div>
		<div>
			<uc:pagercontrol runat="server" statename="buildsstate" />
		</div>
	</div>
	<div class="card-body">
		<div class="list-group">
			<div class="list-group-item p-1" ng-repeat="b in builds" target="_blank" style="background-color: {{b.COLOR}}">
				<div class="d-flex">
					<div class="flex-grow-1">
						<div class="d-flex">
							<span class="mr-1 badge badge-pill badge-secondary"><a class="text-white" href="showtask.aspx?ttid={{b.TTID}}" target="_blank">{{b.TTID}}</a></span>
							<small class="mr-auto" data-toggle="tooltip" title="{{b.NOTES}}">{{b.NOTES | limitTo: 100}}</small>
							<small>{{b.DATEUP}}</small><span class="badge badge-light">{{b.STATUS}}</span>
						</div>
						<div class="d-flex">
							<span><i class="fas fa-desktop"></i></span>
							<small>{{b.MACHINE}}:&nbsp;&nbsp;&nbsp;<span class="badge badge-pill badge-primary">{{b.DURATION}} min</span>&nbsp;&nbsp;&nbsp;</small>
							<div class="mr-auto">
								<div class="progress">
									<div class="progress-bar progress-bar-animated progress-bar-striped active" role="progressbar" style="width: {{ b.STATUS.includes('Building')==true ? b.PERCENT : '100'}}%">
										{{b.STATUS.includes('Building')==true ? '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' + b.PERCENT + '&nbsp;&nbsp;&nbsp;%&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' : ''}}
						
									</div>
								</div>
							</div>
							<a href="getBuildLog.ashx?id={{b.ID}}"><small data-toggle="tooltip" title="{{b.STATUSTXT}}">{{b.STATUSTXT | limitTo: 100}}</small></a>
						</div>
						<div class="d-flex">
							<small class="mr-auto">Sent: {{b.DATE}}</small>
						</div>
					</div>
					<img class="rounded-circle ml-auto" ng-src="{{'getUserImg.ashx?sz=40&ttid=' + b.TTUSERID}}" alt="Smile" height="40" width="40">
				</div>
			</div>
		</div>
	</div>
</div>
