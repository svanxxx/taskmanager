<%@ Page Title="Defaults" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="defaults.aspx.cs" Inherits="defaults" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/defaults_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/defaults_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="alert alert-danger savebutton btn-group-vertical" ng-cloak ng-show="changed">
			<button type="button" class="btn btn-lg btn-info" ng-click="save()">Save</button>
			<button type="button" class="btn btn-lg btn-danger" ng-click="discard()">Discard</button>
		</div>
		<div class="row">
			<div class="col-sm-3"></div>
			<div class="col-sm-6">
				<h2>Default Task Values</h2>

				<div class="table-responsive">
					<table class="table table-hover table-bordered">
						<thead class="thead-dark">
							<tr class="info">
								<th>Property</th>
								<th>Value</th>
							</tr>
						</thead>
						<tbody>
							<tr>
								<td>Product</td>
								<td>
									<select class="form-control form-control-sm w-100" ng-disabled="readonly" ng-model="deffObj.PRODUCT">
										<option ng-repeat="p in products" ng-value="p.ID">{{p.DESCR}}</option>
									</select>
								</td>
							</tr>
							<tr>
								<td>Type</td>
								<td>
									<select class="form-control form-control-sm w-100" ng-disabled="readonly" ng-model="deffObj.TYPE">
										<option ng-repeat="t in types" ng-value="t.ID">{{t.DESCR}}</option>
									</select>
								</td>
							</tr>
							<tr>
								<td>Disposition</td>
								<td>
									<select class="form-control form-control-sm w-100" ng-disabled="readonly" ng-model="deffObj.DISP">
										<option ng-repeat="t in dispos" ng-value="t.ID">{{t.DESCR}}</option>
									</select>
								</td>
							</tr>
							<tr>
								<td>Priority</td>
								<td>
									<select class="form-control form-control-sm w-100" ng-disabled="readonly" ng-model="deffObj.PRIO">
										<option ng-repeat="t in prios" ng-value="t.ID">{{t.DESCR}}</option>
									</select>
								</td>
							</tr>
							<tr>
								<td>Component</td>
								<td>
									<select class="form-control form-control-sm w-100" ng-disabled="readonly" ng-model="deffObj.COMP">
										<option ng-repeat="t in comps" ng-value="t.ID">{{t.DESCR}}</option>
									</select>
								</td>
							</tr>
							<tr>
								<td>Severity</td>
								<td>
									<select class="form-control form-control-sm w-100" ng-disabled="readonly" ng-model="deffObj.SEVR">
										<option ng-repeat="t in severs" ng-value="t.ID">{{t.DESCR}}</option>
									</select>
								</td>
							</tr>
							<tr>
								<td>Estimated</td>
								<td>
									<input type="text" class="form-control form-control-sm w-100" ng-disabled="readonly" ng-model="deffObj.ESTIMATED">
								</td>
							</tr>
						</tbody>
					</table>
				</div>
			</div>
			<div class="col-sm-3"></div>
		</div>
	</div>
</asp:Content>
