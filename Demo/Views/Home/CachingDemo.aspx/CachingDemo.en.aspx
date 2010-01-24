<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="aboutTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Caching Demo
</asp:Content>

<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <h1>CachingDemo.en.aspx (English)</h1>
    <h2>Caching Demo</h2>
    <p> 
        The current time is:  <%= DateTime.Now.ToString() %>
    </p>
    </p>
    <p>
        <% Html.RenderPartial ("CultureSwitcherControl", ViewContext.RouteData.Values["Action"]); %>
    </p>
</asp:Content>
