using Microsoft.AspNetCore.Mvc;

namespace AtendimentoOuvidoria.UI.Web.Helpers
{
    public static class NavigationIndicator
    {
        /// <summary>
        /// Marca os itens de menu como ativos
        /// </summary>
        /// <param name="urlHelper"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static string MakeActiveClass(this IUrlHelper urlHelper, string controller, string action)
        {
            try
            {
                string result = "active";
                string controllerName = urlHelper.ActionContext.RouteData.Values["controller"].ToString();
                string methodName = urlHelper.ActionContext.RouteData.Values["action"].ToString();
                if (string.IsNullOrEmpty(controllerName)) return null;
                if (controllerName.Equals(controller, StringComparison.OrdinalIgnoreCase))
                {
                    if (methodName.Equals(action, StringComparison.OrdinalIgnoreCase))
                    {
                        return result;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}
