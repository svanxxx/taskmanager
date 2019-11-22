<%@ Control Language="C#" AutoEventWireup="true" Inherits="DefectControl" %>
<div class="dropdown <%= this.Class() %>" <%= this.Attrs() %>>
	<img ttid="{{<%=this.Member() %>.ID}}" onclick="assignDefect(this)" style="cursor: pointer" class="rounded-circle" height="20" width="20" ng-src="{{'getUserImg.ashx?sz=20&ttid='+<%=this.Member() %>.AUSER}}" />
</div>
