using ArcheOne.Helper.CommonHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ArcheOne.Filters
{
    public class ActionFilters : ActionFilterAttribute
    {
        private readonly CommonHelper _commonHelper;
        private IConfiguration _configuration { get; }
        public ActionFilters(CommonHelper commonHelper, IConfiguration configuration)
        {
            _configuration = configuration;
            _commonHelper = commonHelper;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Convert.ToBoolean(_configuration.GetSection("Switches:EnableAuthorization").Value))
            {
                // This code will execute before the Index method is called
                string controllerName = filterContext.ActionDescriptor.RouteValues["controller"] ?? "";
                string actionName = filterContext.ActionDescriptor.RouteValues["action"] ?? "";

                string[] validControllers = new string[] { "LogIn", "Dashboard", "Error", "Profile" };

                if (!validControllers.Any(x => x == controllerName))
                {
                    bool isAjaxRequest = filterContext.HttpContext.Request.Headers.ContainsKey("X-Requested-With");
                    var permissionList = _commonHelper.GetPermissionList();
                    if (permissionList.Count == 0)
                    {
                        var routeValues = new RouteValueDictionary(new
                        {
                            controller = "Login",
                            action = "Login",
                            isAjax = isAjaxRequest // Set the value of isAjax parameter
                        });

                        // Redirect to the new controller, action, and pass the route values
                        filterContext.Result = new RedirectToActionResult("LogIn", "LogIn", routeValues);
                    }
                    else if (!permissionList.Any(x => x.PermissionRoute.Contains($"{controllerName}/{actionName}")))
                    {
                        // Create a RouteValueDictionary to store the route values
                        var routeValues = new RouteValueDictionary(new
                        {
                            controller = "Error",
                            action = "Forbidden",
                            isAjax = isAjaxRequest // Set the value of isAjax parameter
                        });

                        // Redirect to the new controller, action, and pass the route values
                        filterContext.Result = new RedirectToActionResult("Forbidden", "Error", routeValues);
                    }
                }
                base.OnActionExecuting(filterContext);
            }
        }
    }
}
