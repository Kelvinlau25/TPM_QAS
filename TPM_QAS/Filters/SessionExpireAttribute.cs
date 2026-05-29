using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TPM_QAS.Filters
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class SessionExpireAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var context = filterContext.HttpContext;

            string redirectTo = "~/Home/Index";

            if (context.Session.GetString("AclUser") == null)
            {
                var rawUrl = context.Request.Path + context.Request.QueryString;
                if (!string.IsNullOrEmpty(rawUrl))
                {
                    redirectTo = $"{redirectTo}?ReturnUrl={Uri.EscapeDataString(rawUrl)}";
                }

                filterContext.Result = new RedirectResult(redirectTo);
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
