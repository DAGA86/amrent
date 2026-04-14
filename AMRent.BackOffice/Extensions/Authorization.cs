using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace AMRent.BackOffice.Extensions
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public Data.Enums.Permissions Permission { get; }

        public PermissionRequirement(Data.Enums.Permissions permission)
        {
            Permission = permission;
        }
    }

    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPermissionService _permissionService;

        public PermissionHandler(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            // Check if the user has the required permission
            if (await _permissionService.UserHasPermissionAsync(context.User, requirement.Permission))
            {
                context.Succeed(requirement);
            }
        }
    }

    public interface IPermissionService
    {
        Task<bool> UserHasPermissionAsync(ClaimsPrincipal user, Data.Enums.Permissions permission);
    }

    public class PermissionService : IPermissionService
    {
        private readonly Data.Contexts.FullDatabaseContext _dbContext;

        public PermissionService(Data.Contexts.FullDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> UserHasPermissionAsync(ClaimsPrincipal user, Data.Enums.Permissions permission)
        {
            Guid userId = Guid.Parse(user.FindFirst(ClaimTypes.UserData)?.Value);
            List<int> rolesUsers = await _dbContext.RolesUsers
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            return await _dbContext.RolePermissions.AnyAsync(rp => rolesUsers.Contains(rp.RoleId) && rp.Permission == permission);
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class AuthorizePermissionAttribute : TypeFilterAttribute
    {
        public AuthorizePermissionAttribute(Data.Enums.Permissions permission) : base(typeof(PermissionAuthorizationFilter))
        {
            Arguments = new object[] { permission };
        }
    }

    public class PermissionAuthorizationFilter : IAuthorizationFilter
    {
        private readonly Data.Enums.Permissions _permission;

        public PermissionAuthorizationFilter(Data.Enums.Permissions permission)
        {
            _permission = permission;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authorizationService = context.HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();
            var authorizationResult = authorizationService.AuthorizeAsync(context.HttpContext.User, null, new PermissionRequirement(_permission)).Result;

            if (!authorizationResult.Succeeded)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
