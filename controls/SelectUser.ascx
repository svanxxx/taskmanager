<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.UI.UserControl" %>
<div class="modal" id="selectuser">
	<div class="modal-dialog modal-dialog-centered">
		<div class="modal-content">
			<div class="modal-body">
				<div class="list-group">
					<div userid="{{u.ID}}" style="cursor:pointer" onclick="assignDefectUser(this)" href class="list-group-item list-group-item-action p-1" ng-repeat="u in users |  filter : {ACTIVE: true} | orderBy:'FULLNAME'">
						<img class="rounded-circle" height="30" width="30" ng-src="{{'getUserImg.ashx?sz=30&ttid='+u.ID}}" />
						<span>{{u.FULLNAME}}</span>
					</div>
				</div>
			</div>
			<div class="modal-footer p-2">
				<button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
			</div>
		</div>
	</div>
</div>
