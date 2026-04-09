using Microsoft.AspNetCore.Builder;

namespace Mgb.AppRegistration.Extensions;

public static class AppRegistration
{
    public static WebApplicationBuilder RegisterApp(this WebApplicationBuilder builder, string appId)
    {
        builder.Configuration["AppId"] = appId;
        return builder;
    }
}