<%@ Control Language="C#" AutoEventWireup="true" Inherits="DefectControl" %>
<a href="showtask.aspx?ttid={{<%= this.Member() %>.ID}}" target="_blank">
	<span data-toggle="tooltip" title="{{<%= this.Member() %>.FIRE ? 'This task is on fire. It is set up with deadline and expected to be finished in time. If you cannot do it in time - please contact task manager to distribute or re-assign!' : ''}}" class="badge badge-pill badge-secondary" style="background-image: url({{<%= this.Member() %>.FIRE ? 'images/sfire.gif' : ''}})";>{{<%= this.Member() %>.ID}}</span>
</a>