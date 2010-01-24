<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Routing Demo
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Routing Demo</h2>
    <h3>Current Culture: <%= System.Threading.Thread.CurrentThread.CurrentUICulture.Name %></h3>
    <p>
        Try:
        <a id="HyperLink1" href="<%= Url.Action ("RoutingDemo", new { culture = "bg" })%>">
            <%= Url.Action ("RoutingDemo", new { culture = "bg" })%></a>
        <br />
        or:  <a id="A1" href="<%= Url.Action ("RoutingDemo", new { culture = "en-US" })%>">
            <%= Url.Action ("RoutingDemo", new { culture = "en-US" })%></a>           
    </p>

<p style="font-weight: bold">Note that when you clicked on the "Routing Demo" menu the language cookie was cleared, 
because it has higher precedence than the routing parameter.
</p>
</asp:Content>
