<%@ Page Title="Dispositions" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="dispositions.aspx.cs" Inherits="Dispositions" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/dispositions_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/dispositions_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="alert alert-danger savebutton btn-group-vertical" ng-cloak ng-show="changed">
			<button type="button" class="btn btn-lg btn-info" ng-click="save()">Save</button>
			<button type="button" class="btn btn-lg btn-danger" ng-click="discard()">Discard</button>
		</div>
		<div class="row">
			<div class="col-md-1">
				<button ng-click="addRef('<%=RefType.disposition.ToString()%>')" ng-disabled="readonly()" type="button" class="btn btn-outline-secondary btn-block"><i class="fas fa-plus"></i>&nbsp;Add</button>
			</div>
			<div class="col-md-11">
				<div class="table-responsive">
					<table class="table table-hover table-bordered">
						<thead class="thead-dark">
							<tr class="info">
								<th>Disposition name</th>
								<th>Color</th>
								<th>Order</th>
								<th>Requires Work</th>
								<th>Working Now</th>
								<th>Cannot Start</th>
								<th>TESTS Passed</th>
								<th>TESTS Failed</th>
								<th>TESTS Started</th>
							</tr>
						</thead>
						<tbody>
							<tr ng-repeat="r in refs" class="{{r.changed?'data-changed':''}}">
								<td>
									<input ng-disabled="readonly()" class="intable-data-input" type="text" ng-model="r.DESCR" ng-change="itemchanged(r)">
								</td>
								<td>
									<input ng-disabled="readonly()" class="intable-data-input" type="color" ng-model="r.COLOR" ng-change="itemchanged(r)">
								</td>
								<td>
									<input ng-disabled="readonly()" class="intable-data-input" type="number" min="1" max="999" ng-model="r.FORDER" ng-change="itemchanged(r)">
								</td>
								<td align="center">
									<input ng-disabled="readonly()" type="checkbox" ng-model="r.REQUIREWORK" ng-change="itemchanged(r)">
								</td>
								<td align="center">
									<input ng-disabled="readonly()" type="checkbox" ng-model="r.WORKING" ng-change="itemchanged(r)">
								</td>
								<td align="center">
									<input ng-disabled="readonly()" type="checkbox" ng-model="r.CANNOTSTART" ng-change="itemchanged(r)">
								</td>
								<td align="center">
									<input ng-disabled="readonly()" type="checkbox" ng-model="r.TESTSPASS" ng-change="itemchanged(r)">
								</td>
								<td align="center">
									<input ng-disabled="readonly()" type="checkbox" ng-model="r.TESTSFAIL" ng-change="itemchanged(r)">
								</td>
								<td align="center">
									<input ng-disabled="readonly()" type="checkbox" ng-model="r.TESTSSTART" ng-change="itemchanged(r)">
								</td>
							</tr>
						</tbody>
					</table>
				</div>
			</div>
		</div>
	</div>
</asp:Content>
