using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

public static class MyHtml
{
    public static IHtmlContent BackButton(this IHtmlHelper html, string url = "back")
    {
        var jsact = "window.location.href ='" + url + "'; return false;";
        var htmlContent = $"<button class=\"btn btn-primary\" id=\"back\" onclick=\"{jsact}\">Exit</button>";
        return new HtmlString(htmlContent);
    }

    public static IHtmlContent ResetButton(this IHtmlHelper html, string url = "reset")
    {
        var jsact = "window.location.href ='" + url + "'; return false;";
        var htmlContent = $"<button class=\"btn btn-primary\" id=\"reset\" onclick=\"{jsact}\">Reset</button>";
        return new HtmlString(htmlContent);
    }

    public static IHtmlContent WebAlert(this IHtmlHelper html, string message)
    {
        var htmlContent = $"<script>alert('{message}');</script>";
        return new HtmlString(htmlContent);
    }
}
