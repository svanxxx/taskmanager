<%@ Control Language="C#" AutoEventWireup="true" Inherits="DefectControl" %>
<div class="<%= this.Class() %>" <%= this.Attrs() %>>
	<img uniqueid="{{<%=this.ProxiAttribute("userid") %>}}" userid="{{<%=this.ProxiAttribute("userid") %>}}" onmousemove="tooltipImg(event)" alt="Smile" onmouseout="tooltipImgOut()" class="rounded-circle" height="<%=this.ProxiAttribute("size") %>" width="<%=this.ProxiAttribute("size") %>" ng-src="{{'getUserImg.ashx?sz=<%=this.ProxiAttribute("size") %>&ttid='+<%=this.ProxiAttribute("userid") %>}}" />
</div>
