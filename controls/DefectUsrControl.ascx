<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DefectUsrControl.ascx.cs" Inherits="DefectUsrControl" %>
<div class="<%= this.Class() %>" <%= this.Attrs() %>>
<img data-toggle="tooltip" title="<img src='{{'getUserImg.ashx?sz=150&ttid=' + <%=this.Member() %>.AUSER}}' />" class="rounded-circle" height="20" width="20" ng-src="{{'getUserImg.ashx?sz=20&ttid='+<%=this.Member() %>.AUSER}}"/>
</div>