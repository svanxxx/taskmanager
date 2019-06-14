<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DefectNumControl.ascx.cs" Inherits="DefectNumControl" %>
<a href="showtask.aspx?ttid={{d.ID}}" target="_blank">
	<span data-toggle="tooltip" title="{{d.FIRE ? 'This task is on fire. It is set up with deadline and expected to be finished in time. If you cannot do it in time - please contact task manager to distribute or re-assign!' : ''}}" class="badge badge-pill badge-secondary" style="background-image: url({{d.FIRE ? 'images/sfire.gif' : ''}})";>{{d.ID}}</span>
</a>