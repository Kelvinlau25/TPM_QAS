using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TPM_QAS.Filters
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class SessionExpireAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext context = HttpContext.Current;

            string redirectTo = "~/Home/Index";

            if (context.Session["AclUser"] == null)
            {
                if (!string.IsNullOrEmpty(context.Request.RawUrl))
                {
                    redirectTo = $"{redirectTo}?ReturnUrl={HttpUtility.UrlEncode(context.Request.RawUrl)}";
                }

                filterContext.Result = new RedirectResult(redirectTo);
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}