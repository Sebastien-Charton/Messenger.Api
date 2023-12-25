using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace Messenger.Api.Web.Infrastructure.Logging;

public class Serilogger
{
    public static Action<HostBuilderContext, LoggerConfiguration> Configure =>
        (context, configuration) =>
        {
            var elasticUri = context.Configuration.GetValue<string>("ElasticConfiguration:Uri");

            configuration
                .Enrich.FromLogContext()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(
                    new ElasticsearchSinkOptions(new Uri(elasticUri ?? throw new InvalidOperationException()))
                    {
                        IndexFormat =
                            $"applogs-{context.HostingEnvironment.ApplicationName?.ToLower().Replace(".", "-")}-{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
                        AutoRegisterTemplate = true,
                        NumberOfShards = 2,
                        NumberOfReplicas = 1
                    })
                .Enrich.WithProperty("Environment",
                    context.HostingEnvironment.EnvironmentName ?? throw new InvalidOperationException())
                .Enrich.WithProperty("Application",
                    context.HostingEnvironment.ApplicationName ?? throw new InvalidOperationException())
                .Enrich.WithMachineName()
                .Enrich.WithProcessId()
                .Enrich.WithProcessName()
                .Enrich.WithThreadId()
                .Enrich.WithThreadName()
                .Enrich.WithEnvironmentUserName()
                .ReadFrom.Configuration(context.Configuration);
        };
}
