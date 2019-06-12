<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BranchesControl.ascx.cs" Inherits="BranchesControl" %>

<%@ Register Src="~/controls/PagerControl.ascx" TagName="pagercontrol" TagPrefix="uc" %>

<label ng-show="!state.branches">Loading build information, please wait...</label>
<div class="card" ng-cloak>
	<div class="card-header p-1 d-flex">
		<div class="mt-1 mr-auto">
			<h5 class="ml-2" style="display: inline"><i class="fas fa-code-branch"></i></h5>
			<button type="button" class="btn btn-outline-light text-dark btn-sm" onclick="window.location='branches.aspx'">Branches</button>
		</div>
		<div>
			<uc:pagercontrol runat="server" />
		</div>
	</div>
	<div class="card-body">
		<div class="list-group">
			<div class="list-group-item list-group-item-action pt-1 pb-1" ng-repeat="b in state.branches" style="background-color: {{b.COLOR}}">
				<div class="d-flex">
					<div>
						<div class="d-flex">
							<div ng-show="b.TTID < 0">
								<a href="commits.aspx?branch={{b.NAME}}"><i class="fas fa-tag"></i><span class="badge badge-light">{{b.NAME}}</span></a>
							</div>
							<div ng-show="b.TTID > 0">
								<a href="showtask.aspx?ttid={{b.TTID}}" target="_blank"><span class="badge badge-pill badge-secondary ng-binding">{{b.TTID}}</span></a>{{b.NAME.replace('TT' + b.TTID, '')}}
							</div>
						</div>
						<div class="d-flex">
							<small>Commit Date:&nbsp;{{b.DATE}}</small>
						</div>
						<div class="d-flex">
							<span data-toggle="tooltip" title="{{b.HASH}}" class="badge badge-warning ng-binding">{{b.HASH | limitTo: 20}}</span>
						</div>
					</div>
					<div class="ml-auto">
						<a ng-click="filterby(b.AUTHOR)" href>{{b.AUTHOR}}</a>
						<img class="rep-img rounded-circle" ng-src="{{'getUserImg.ashx?sz=40&eml=' + b.AUTHOREML}}" alt="Smile" height="40" width="40">
					</div>
				</div>
			</div>
		</div>
	</div>
</div>
