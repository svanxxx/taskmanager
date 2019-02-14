<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PagerControl.ascx.cs" Inherits="PagerControl" %>

<div class="row mt-1">
	<div class="col-sm-6">
		<ul class="pagination pagination-sm mb-1 justify-content-center">
			<li class="page-item"><a style="border-color: lightgray;" class="page-link bg-light text-dark" href ng-click="decPage()">Previous</a></li>
			<li class="page-item"><a class="page-link bg-light text-dark" href="javascript:void(0);">{{state.page}}</a></li>
			<li class="page-item"><a style="border-color: lightgray;" class="page-link bg-light text-dark" href ng-click="incPage()">Next</a></li>
		</ul>
	</div>
	<div class="col-sm-6">
		<div class="input-group input-group-sm">
			<div class="input-group-prepend">
				<span class="input-group-text">Show By: </span>
			</div>
			<select class="form-control" ng-change="changeShowBy()" ng-model="state.showby" ng-options="x for x in showbys"></select>
		</div>
	</div>
</div>
