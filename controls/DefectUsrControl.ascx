<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DefectUsrControl.ascx.cs" Inherits="DefectUsrControl" %>
<div class="dropdown <%= this.Class() %>" <%= this.Attrs() %>>
	<img style="cursor: pointer" data-toggle="dropdown" class="dropdown-toggle rounded-circle" height="20" width="20" ng-src="{{'getUserImg.ashx?sz=20&ttid='+<%=this.Member() %>.AUSER}}" />
<!--	<div class="dropdown-menu">
		<div class="dropdown-item" ng-show="u.ACTIVE" ng-repeat="u in users | orderBy:'FULLNAME'" style="cursor:pointer" onclick="assignDefect(this)" ttid="{{<%= this.Member() %>.ID}}" userid="{{u.ID}}">
			<img class="rounded-circle" height="20" width="20" ng-src="{{'getUserImg.ashx?sz=20&ttid='+u.ID}}" />
			<span>{{u.FULLNAME}}</span>
		</div>
	</div>!-->
</div>
