<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PagerControl.ascx.cs" Inherits="PagerControl" %>

<div class="row mt-2">
	<div class="col-sm-10">
		<ul class="pagination justify-content-center">
			<li class="page-item"><a class="page-link" href ng-click="decPage()">Previous</a></li>
			<li class="page-item"><a class="page-link" href="javascript:void(0);">{{state.page}}</a></li>
			<li class="page-item"><a class="page-link" href ng-click="incPage()">Next</a></li>
		</ul>
	</div>
	<div class="col-sm-2">
		<div class="input-group">
			<div class="input-group-prepend">
				<span class="input-group-text">Show By: </span>
			</div>
			<select class="form-control" ng-change="changeShowBy()" ng-model="state.showby" ng-options="x for x in showbys" />
		</div>
	</div>
</div>

