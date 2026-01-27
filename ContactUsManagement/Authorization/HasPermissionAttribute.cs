using Microsoft.AspNetCore.Authorization;

namespace ContactUsManagement.Authorization
{
    public class HasPermissionAttribute : AuthorizeAttribute
    {

        public HasPermissionAttribute(string permission) : base(policy: permission) 
        {
        
        }
    }
}
