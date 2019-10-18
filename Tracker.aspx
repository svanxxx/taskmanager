<%@ Page Title="Tracker" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeFile="tracker.aspx.cs" Inherits="Tracker" %>

<%@ Register Src="~/controls/DefectSpentControl.ascx" TagName="defSpent" TagPrefix="uc" %>
<%@ Register Src="~/controls/DefectNumControl.ascx" TagName="defNum" TagPrefix="uc" %>
<%@ Register Src="~/controls/DefectEstControl.ascx" TagName="defEst" TagPrefix="uc" %>

<asp:Content ID="HeadContentData" ContentPlaceHolderID="HeaddContent" runat="server">
    <%=System.Web.Optimization.Styles.Render("~/bundles/tracker_css")%>
    <%=System.Web.Optimization.Scripts.Render("~/bundles/tracker_js")%>
    <script src="<%=Settings.CurrentSettings.ANGULARCDN.ToString()%>angular.min.js"></script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="false">
    <div ng-app="mpsapplication" ng-controller="mpscontroller">
        <h2 class="text-center">Task Tracker</h2>
        <div class="row">
            <div class="col-3">
                <h5 class="text-center">Legend:</h5>
                <div class="card">
                    <div class="card-body p-1">Basic card</div>
                </div>
            </div>
            <div class="col-6">
                <ul class="list-group">
                    <li class="list-group-item d-flex justify-content-between align-items-center p-1">Inbox
                    </li>
                </ul>
                <ul class="pagination pagination-sm">
                    <li class="page-item"><a class="page-link" href="#">Previous</a></li>
                    <li class="page-item"><a class="page-link" href="#">1</a></li>
                    <li class="page-item"><a class="page-link" href="#">2</a></li>
                    <li class="page-item"><a class="page-link" href="#">3</a></li>
                    <li class="page-item"><a class="page-link" href="#">Next</a></li>
                </ul>
            </div>
            <div class="col-3"></div>
        </div>
    </div>
</asp:Content>
