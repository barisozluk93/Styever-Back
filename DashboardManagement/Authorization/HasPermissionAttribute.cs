using Microsoft.AspNetCore.Authorization;

namespace DashboardManagement.Authorization
{
    public class HasPermissionAttribute : AuthorizeAttribute
    {

        public HasPermissionAttribute(string permission) : base(policy: permission) 
        {
        
        }
    }
}
