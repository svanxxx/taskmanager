<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DefectNumControl.ascx.cs" Inherits="DefectNumControl" %>
<a href="showtask.aspx?ttid={{d.ID}}" target="_blank">
	<span class="badge badge-pill badge-secondary" style="background-image: url({{d.FIRE ? 'images/sfire.gif' : ''}})";>{{d.ID}}</span>
</a>