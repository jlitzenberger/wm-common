using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;

namespace WM.Common.Api.Attributes
{
    public class ControllerAuthorize : AuthorizeAttribute
    {
        public string ActiveDirectoryRoles { get; set; }
        protected override bool IsAuthorized(System.Web.Http.Controllers.HttpActionContext httpContext)
        {
            string environment = WebConfigurationManager.AppSettings["Environment"];

            base.Roles = ActiveDirectoryRoles + ((environment != "PRD") ? "_" + environment : null);

            bool authorized = base.IsAuthorized(httpContext);

            return authorized;
        }
    }
}
