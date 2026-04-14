using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace AMRent.Website.Filters
{
    public class PassAlongQueryParamertersFilter : ActionFilterAttribute
    {
        public string Filter { get; set; } = "*"; // Default: Pass all query parameters

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is RedirectToActionResult redirectResult)
            {
                var httpContext = context.HttpContext;
                var routeValues = new RouteValueDictionary(redirectResult.RouteValues);

                // Check request method
                if (httpContext.Request.Method == HttpMethods.Post)
                {
                    var newRouteValues = new RouteValueDictionary();
                    // Extract from form data (POST request)
                    var formParams = httpContext.Request.Form;
                    foreach (var key in formParams.Keys.Where(k => k.StartsWith("queryStringParam_")))
                    {
                        newRouteValues.Add(key.Replace("queryStringParam_", ""), formParams[key].ToString());
                    }
                    routeValues = newRouteValues;
                }
                else
                {
                    // Extract from query string (GET request)
                    var queryParams = httpContext.Request.Query;
                    foreach (var key in queryParams.Keys)
                    {
                        routeValues[key] = queryParams[key].ToString();
                    }
                }

                // Create new RedirectToActionResult with updated parameters
                context.Result = new RedirectToActionResult(
                    redirectResult.ActionName,
                    redirectResult.ControllerName,
                    routeValues,
                    redirectResult.Permanent
                );
                //var queryParams = context.HttpContext.Request.Query;
                //var regex = WildcardToRegex(Filter);
                //var routeValues = new RouteValueDictionary(redirectResult.RouteValues);

                //// Add matching query parameters to the route values
                //foreach (var key in queryParams.Keys.Where(k => Regex.IsMatch(k, regex)))
                //{
                //    routeValues[key] = queryParams[key].ToString();
                //}

                //// Create a new RedirectToActionResult with updated parameters
                //context.Result = new RedirectToActionResult(
                //    redirectResult.ActionName,
                //    redirectResult.ControllerName,
                //    routeValues,
                //    redirectResult.Permanent
                //);
            }
        }

        //private static string WildcardToRegex(string pattern)
        //{
        //    return "^" + Regex.Escape(pattern)
        //        .Replace("\\*", ".*")  // Convert `*` to match anything
        //        .Replace("\\?", ".")   // Convert `?` to match a single character
        //        + "$";
        //}
    }
}
