namespace VehicleRegistration.API.Middleware;

public static class SecurityHeadersExtensions
{
    public static IServiceCollection AddSecurityHeaders(this IServiceCollection services)
    {
        services.AddAntiforgery(options =>
        {
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });
        
        return services;
    }

    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
    {
        return app.UseMiddleware<SecurityHeadersMiddleware>();
    }
}