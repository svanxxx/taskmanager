<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CommitsControl.ascx.cs" Inherits="CommitsControl" %>

<%@ Register Src="~/controls/PagerControl.ascx" TagName="pagercontrol" TagPrefix="uc" %>

<label ng-show="!commits">Loading commits, please wait...</label>
<div class="card" ng-cloak>
	<div class="card-header p-1 d-flex" ng-hide="<%=this.Hidden() %>"">
		<div class="mt-1 mr-auto">
			<h5 class="ml-2" style="display: inline"><i class="fas fa-code-branch"></i></h5>
			<button type="button" class="btn btn-outline-light text-dark btn-sm" onclick="window.location='branches.aspx'">Current Branch: {{branch}}</button>
		</div>
		<div>
			<uc:pagercontrol id="Pager" runat="server" />
		</div>
	</div>
	<div class="card-body">
		<div class="list-group">
			<div class="list-group-item commit-item" ng-repeat="c in <%= this.DatasetName() %>">
				<div class="d-flex">
					<div class="mr-auto">
						<div class="d-flex" ng-show="c.TTID < 0">
							<span data-toggle="tooltip" title="{{c.NOTES}}">{{c.NOTES | limitTo: 100}}</span>
						</div>
						<div class="d-flex" ng-show="c.TTID > 0">
							{{b.NOTES}}
							<a href="showtask.aspx?ttid={{c.TTID}}" target="_blank"><span class="badge badge-pill badge-secondary ng-binding">{{c.TTID}}</span></a>{{c.NOTES.replace('TT' + c.TTID, '') | limitTo: 100}}
						</div>
						<div class="d-flex">
							<small>Commit: {{c.DATE}}</small><small>&nbsp;&nbsp;&nbsp;Author: {{c.AUTHOR_DATE}}</small>
							<button data-toggle="tooltip" title="Click to see changes..." type="button" class="btn btn-outline-light text-dark btn-sm p-0" ng-click="loadCommit(c, '<%= this.DatasetName() %>')">&nbsp;&nbsp;<i class="fas fa-ellipsis-h"></i></button>
						</div>
					</div>
					<div>
						<span>{{c.AUTHORNAME}}</span>
						<br />
						<span data-toggle="tooltip" title="{{c.COMMIT}}" class="badge badge-warning ng-binding">{{c.COMMIT | limitTo: 20}}</span>
					</div>
					<img class="rep-img rounded-circle" ng-src="<%=CurrentContext.ImagesUrl("e")%>{{c.AUTHOREML}}" alt="Smile" height="40" width="40">
				</div>
				<div class="row">
					<pre ng-show="c.DIFF"><code ng-bind-html="c.DIFF | rawHtml"></code></pre>
				</div>
			</div>
		</div>
	</div>
</div>

