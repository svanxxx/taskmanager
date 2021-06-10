<%@ Control Language="C#" AutoEventWireup="true" Inherits="DefectControl" %>
<span style="cursor:pointer" ttid="{{<%= this.Member() %>.ID}}" onclick="estimateDefect(this)" data-toggle="tooltip" title="Estimated: {{<%= this.Member() %>.ESTIM}} hrs {{<%= this.Member() %>.ESTIM < <%= this.Member() %>.SPENT ? 'Getting out of time!!!' : ''}}" class="badge badge-danger {{<%= this.Member() %>.ESTIM < <%= this.Member() %>.SPENT ? 'red-alarm' : ''}}">{{<%= this.Member() %>.ESTIM}}</span>
<span 
ng-show="<%= this.Member() %>.SPENT > 0" 
data-toggle="tooltip" 
title="Worked on: {{<%= this.Member() %>.SPENT}} hrs {{<%= this.Member() %>.PRIMARYHOURS ? ' (Primary: ' + <%= this.Member() %>.PRIMARYHOURS + ', Bug Fixes: '+ (<%= this.Member() %>.SPENT - <%= this.Member() %>.PRIMARYHOURS) + ')': ''}}" 
class="badge badge-warning">{{<%= this.Member() %>.SPENT}}
</span>