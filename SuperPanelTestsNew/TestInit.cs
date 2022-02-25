using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Polly;
using SuperPanel.App.Data;
using SuperPanel.App.Infrastructure;
using System;
using System.Collections.Generic;

namespace SuperPanelTestsNew
{
    [SetUpFixture]
    public class TestInit
    {
        internal static IServiceScopeFactory _scopeFactory = null!;
        public IConfiguration Configuration { get; set; }

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            var services = new ServiceCollection();

            var testConfiguration = new Dictionary<string, string>
                                    {
                                        {"Data:JsonFilePath", "../../../../data/users.json"},
                                        {"Data:ExternalContactsApiURL", "http://localhost:61695"},
                                    };

            Configuration = new ConfigurationBuilder()
                                .AddInMemoryCollection(testConfiguration)
                                .Build();

            services.AddOptions();

            services.Configure<DataOptions>(options => Configuration.GetSection("Data").Bind(options));

            var sp = services.BuildServiceProvider();
            var cfg = sp.GetRequiredService<IOptions<DataOptions>>();

            // Add External Contacts API named client
            services.AddHttpClient("ExternalContactsApi", c =>
            {
                c.BaseAddress = new Uri(cfg.Value.ExternalContactsApiURL);
            })
            //Config Polly for client's retry
            .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryForeverAsync(retry => TimeSpan.FromSeconds(retry), (exception, timeSpan) =>
            {
                Console.WriteLine(exception);
            }))
            .AddTransientHttpErrorPolicy(policy => policy.OrResult(result => result.StatusCode == System.Net.HttpStatusCode.NotFound)
                                                         .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(5)));

            // Data
            services.AddSingleton<IUserRepository, UserRepository>();
            //External Contacts repository
            services.AddTransient<IExternalContactsRepository, ExternalContactsRepository>();

            _scopeFactory = services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>();
        }
    }
}
