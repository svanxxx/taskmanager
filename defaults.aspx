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
				<div class="form-group">
					<label class="col-sm-2 control-label">Product</label>
					<div class="col-sm-10">
						<select class="form-control input-sm" ng-disabled="readonly" ng-model="deffObj.PRODUCT">
							<option ng-repeat="p in products" ng-value="p.ID">{{p.DESCR}}</option>
						</select>
					</div>
					<label class="col-sm-2 control-label">Type</label>
					<div class="col-sm-10">
						<select class="form-control input-sm" ng-disabled="readonly" ng-model="deffObj.TYPE">
							<option ng-repeat="t in types" ng-value="t.ID">{{t.DESCR}}</option>
						</select>
					</div>
					<label class="col-sm-2 control-label">Disposition</label>
					<div class="col-sm-10">
						<select class="form-control input-sm" ng-disabled="readonly" ng-model="deffObj.DISP">
							<option ng-repeat="t in dispos" ng-value="t.ID">{{t.DESCR}}</option>
						</select>
					</div>
					<label class="col-sm-2 control-label">Priority</label>
					<div class="col-sm-10">
						<select class="form-control input-sm" ng-disabled="readonly" ng-model="deffObj.PRIO">
							<option ng-repeat="t in prios" ng-value="t.ID">{{t.DESCR}}</option>
						</select>
					</div>
					<label class="col-sm-2 control-label">Component</label>
					<div class="col-sm-10">
						<select class="form-control input-sm" ng-disabled="readonly" ng-model="deffObj.COMP">
							<option ng-repeat="t in comps" ng-value="t.ID">{{t.DESCR}}</option>
						</select>
					</div>
					<label class="col-sm-2 control-label">Severity</label>
					<div class="col-sm-10">
						<select class="form-control input-sm" ng-disabled="readonly" ng-model="deffObj.SEVR">
							<option ng-repeat="t in severs" ng-value="t.ID">{{t.DESCR}}</option>
						</select>
					</div>
				</div>
			</div>
			<div class="col-sm-3"></div>
		</div>
	</div>
</asp:Content>
