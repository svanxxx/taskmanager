<%@ Page Title="Machines" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="machines.aspx.cs" Inherits="Machines" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
	<%=System.Web.Optimization.Styles.Render("~/bundles/machines_css")%>
	<%=System.Web.Optimization.Scripts.Render("~/bundles/machines_js")%>
	<script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
	<div ng-app="mpsapplication" ng-controller="mpscontroller">
		<div class="row">
			<div class="col-lg-2"></div>
			<div class="col-lg-8">
				<div ng-show="!searchMachine && !hasMachine()">
					<h1 class="label-center">Select machine to process</h1>
					<div class="row" ng-repeat="m in machines | orderBy : 'NAME'">
						<button ng-click="setMachine(m.NAME)" type='button' class="btn btn-block btn-lg machinebutton" ng-style="m | mcol:this">{{m.NAME}}<img ng-show="unpinged(m)" class="ping" src="images/process.gif" /></button>
					</div>
					<button ng-click="searchMachine=true" type='button' class='btn btn-block btn-danger btn-lg machinebutton'>Add machine</button>
				</div>
				<div ng-show="searchMachine && !hasMachine()">
					<h1 class="label-center">Select domain computer</h1>
					<div class="row" ng-repeat="c in domainComputers">
						<button ng-click="setMachine(c)" type='button' class='btn btn-block btn-info btn-lg machinebutton'>{{c}}</button>
					</div>
					<button ng-click="searchMachine=false" type='button' class='btn btn-block btn-primary btn-lg'>Back</button>
				</div>
				<div ng-show="hasMachine()">
					<h1 class="label-center">{{workmachine}}</h1>
					<button ng-click="wakeMachine()" type='button' class='btn btn-block btn-success btn-lg' id='wake'>Wake Up</button>
					<button ng-click="shutMachine()" type='button' class='btn btn-block btn-warning btn-lg' id='shut'>Shut Down</button>
					<button ng-click="scanMachine()" type='button' class='btn btn-block btn-primary btn-lg' id='scan'>Scan for MAC</button>
					<button ng-click="remMachine()" type='button' class='btn btn-block btn-danger btn-lg' id='remove'>Remove</button>
					<button ng-click="setMachine('')" type='button' class='btn btn-block btn-primary btn-lg'>Back</button>
				</div>
			</div>
			<div class="col-lg-2"></div>
		</div>
	</div>
</asp:Content>
