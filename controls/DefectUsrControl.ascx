<%@ Control Language="C#" AutoEventWireup="true" Inherits="DefectControl" %>
<div class="<%= this.Class() %>" <%= this.Attrs() %>>
	<img ttid="{{<%=this.Member() %>.ID}}" userid="{{<%=this.Member() %>.AUSER}}" onmousemove="tooltipImg(event)" onmouseout="tooltipImgOut()" onclick="assignDefect(this)" style="cursor: pointer" class="rounded-circle" height="20" width="20" ng-src="{{'getUserImg.ashx?sz=20&ttid='+<%=this.Member() %>.AUSER}}" />
</div>
