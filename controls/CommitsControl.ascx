<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CommitsControl.ascx.cs" Inherits="CommitsControl" %>

<label ng-show="!commits">Loading commits, please wait...</label>
<div class="list-group">
	<div class="list-group-item commit-item" ng-repeat="c in commits" onmouseover="this.style.background='#00bcd426'" onmouseout="this.style.background=''">
		<div style="cursor: pointer;" data-toggle="tooltip" title="Click to see changes..." class="row" ng-click="loadCommit(c)">
			<div class="col-sm-2">
				<span>{{c.DATE}}</span>
			</div>
			<div class="col-sm-8">
				<span>{{c.NOTES}}</span>
			</div>
			<div class="col-sm-1">
				<img class="rep-img rounded-circle" ng-src="{{'getUserImg.ashx?sz=20&eml=' + c.AUTHOREML}}" alt="Smile" height="20" width="20">
				<span>{{c.AUTHORNAME}}</span>
			</div>
			<div class="col-sm-1">
				<span data-toggle="tooltip" title="{{c.COMMIT}}" class="badge badge-warning ng-binding">{{c.COMMIT.substr(0, 7)}}</span>
			</div>
		</div>
		<div class="row">
			<pre ng-show="c.DIFF"><code ng-bind-html="c.DIFF | rawHtml"></code></pre>
		</div>
	</div>
</div>
