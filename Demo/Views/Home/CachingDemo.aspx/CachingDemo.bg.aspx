<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="aboutTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Демо на Кеширането
</asp:Content>

<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <h1>CachingDemo.bg.aspx (Bulgarian)</h1>
    <h2>Демо на кеширането</h2>
    <p>
        В момента часът е: <%= DateTime.Now.ToString () %>
    </p>
    <p>
        <% Html.RenderPartial ("CultureSwitcherControl", ViewContext.RouteData.Values["Action"]); %>
    </p>
</asp:Content>
