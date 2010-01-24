<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    IvanZ.Mvc.LocalizableViews Demo
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <h1>LocalizationDemo.en.aspx (English)</h1>
    <p>
        <% Html.RenderPartial ("CultureSwitcherControl", ViewContext.RouteData.Values["Action"]); %>
    </p>
</asp:Content>
