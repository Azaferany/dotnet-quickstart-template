using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;

namespace QuickstartTemplate.Infrastructure.Common;

/// <summary>
/// add all handlers of GlobalMessageHandlerConfigure client to all clients
/// https://github.com/dotnet/AspNetCore.Docs/issues/18162
/// </summary>
public class GlobalHttpMessageHandlerBuilderFilter : IHttpMessageHandlerBuilderFilter
{
    public const string GlobalMessageHandlerConfigure = nameof(GlobalMessageHandlerConfigure);

    public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next) =>
        handlerBuilder =>
        {
            next(handlerBuilder);

            if (handlerBuilder.Name != GlobalMessageHandlerConfigure)
            {
                var clientFactoryOptions = handlerBuilder.Services
                    .GetRequiredService<IOptionsMonitor<HttpClientFactoryOptions>>().Get(GlobalMessageHandlerConfigure);

                if (clientFactoryOptions is not null)
                    foreach (var builderAction in clientFactoryOptions.HttpMessageHandlerBuilderActions)
                        builderAction(handlerBuilder);
            }
        };
}
