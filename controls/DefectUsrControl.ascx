<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DefectUsrControl.ascx.cs" Inherits="DefectUsrControl" %>
<img data-toggle="tooltip" title="<img src='{{'getUserImg.ashx?sz=150&ttid=' + <%=this.Member() %>.AUSER}}' />" class="rounded-circle <%= this.Class() %>" <%= this.Attrs() %> height="20" width="20" ng-src="{{'getUserImg.ashx?sz=20&ttid='+<%=this.Member() %>.AUSER}}">
