using Bogus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SuperPanel.App.Data;
using SuperPanel.App.Infrastructure;
using SuperPanel.App.Models;
using System;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Polly;
using SuperPanel.App.Helpers;

namespace SuperPanel.App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Uncomment to generate new batch of bogus data
            // GenerateFakeData();

            services.AddControllersWithViews();
            services.AddSpaStaticFiles(options => options.RootPath = "client-app/dist");
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
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Users/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSpaStaticFiles();
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "client-app";
                if (env.IsDevelopment())
                {
                    // Launch development server for Nuxt
                    spa.UseNuxtDevelopmentServer();
                }
            });



        }

        private void GenerateFakeData()
        {
            //Set the randomizer seed if you wish to generate repeatable data sets.
            Randomizer.Seed = new Random(8675309);

            var userIds = 10000;
            var faker = new Faker<User>()
                .CustomInstantiator(f => new User(userIds++))
                .RuleFor(u => u.Login, (f, u) => f.Internet.UserName())
                .RuleFor(u => u.FirstName, (f, u) => f.Name.FirstName())
                .RuleFor(u => u.LastName, (f, u) => f.Name.LastName())
                .RuleFor(u => u.Phone, (f, u) => f.Phone.PhoneNumber())
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                .RuleFor(u => u.CreatedAt, (f, u) => f.Date.Past(3));

            var users = faker.Generate(5000)
                    .OrderBy(_ => Randomizer.Seed.Next())
                    .ToList();

            string json = JsonSerializer.Serialize(users, new JsonSerializerOptions() { WriteIndented = false });
            System.IO.File.WriteAllText("./../data/users.json", json);

        }
    }
}
