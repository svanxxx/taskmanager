<%@ Page Title="Machines" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="machines.aspx.cs" Inherits="Machines" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/machines_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/machines_js")%>
	<script src="http://mps.resnet.com/cdn/angular/angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div ng-show="!hasMachine()">
			<div class="row" ng-repeat="m in machines | orderBy : 'NAME'">
				<button ng-click="setMachine(m.NAME)" type='button' class='btn btn-block btn-info btn-lg machinebutton'>{{m.NAME}}</button>
			</div>
		</div>
		<div ng-show="hasMachine()">
			<button type='button' class='btn btn-block btn-primary btn-lg' id='wake'>Wake Up</button>
			<button type='button' class='btn btn-block btn-primary btn-lg' id='shut'>Shut Down</button>
			<button type='button' class='btn btn-block btn-primary btn-lg' id='scan'>Scan for MAC</button>
			<button type='button' class='btn btn-block btn-primary btn-lg' id='remove'>Remove</button>
			<button ng-click="setMachine('')" type='button' class='btn btn-block btn-primary btn-lg'>Back</button>
		</div>
	</div>
</asp:Content>
