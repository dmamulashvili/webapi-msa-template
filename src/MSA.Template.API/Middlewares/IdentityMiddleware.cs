using SharedKernel.Interfaces;
using System.Security.Claims;

namespace MSA.Template.API.Middlewares;

public class IdentityMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<IdentityMiddleware> _logger;

    public IdentityMiddleware(RequestDelegate next, ILogger<IdentityMiddleware> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _next = next;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var identityProviderService = context.RequestServices.GetRequiredService<IIdentityServiceProvider>();
        if (Guid.TryParse(_httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                out var userIdentity))
        {
            identityProviderService.SetIdentity(userIdentity);
        }

        await _next(context);
    }
}