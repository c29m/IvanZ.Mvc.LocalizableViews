<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<h2>The current culture is:
    <%= System.Threading.Thread.CurrentThread.CurrentUICulture.Name %>
</h2>
<% using (Html.BeginForm ("SwitchCulture", "Home")) { %>
    <label for="culture">Switch Culture:</label>
    <%= Html.DropDownList ("culture", new SelectList (LocalizationConfig.SupportedCultures, 
                                                      System.Threading.Thread.CurrentThread.CurrentUICulture.Name)) %>
    <input type="hidden" name="returnAction" id="returnAction" value="<%= (string)this.Model %>" />
    <input type="submit" value="Apply" id="submit" />
<% } %>