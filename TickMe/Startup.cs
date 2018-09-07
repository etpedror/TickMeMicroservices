using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace TickMe
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
            //ConfigureCookieAuthentication(services);
            ConfigureAzureB2CAuthentication(services);



            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        private void ConfigureCookieAuthentication(IServiceCollection services)
        {
            services.AddAuthentication("TickMeSecurityScheme")
                    .AddCookie("TickMeSecurityScheme", options =>
                    {
                        options.AccessDeniedPath = new PathString("/Security/Access");
                        options.LoginPath = new PathString("/Security/Login");
                    });
        }

        private void ConfigureAzureB2CAuthentication(IServiceCollection services)
        {
            var signUpPolicy = "B2C_1_sign_up";
            var signInPolicy = "B2C_1_sign_in";
            var signUpInPolicy = "B2C_1_sign_up_in";
            var editProfilePolicy = "B2C_1_edit_profile";


            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = signUpInPolicy;
                })
                .AddOpenIdConnect(signUpPolicy, GetOpenIdConnectOptions(signUpPolicy))
                .AddOpenIdConnect(signInPolicy, GetOpenIdConnectOptions(signInPolicy))
                .AddOpenIdConnect(signUpInPolicy, GetOpenIdConnectOptions(signUpInPolicy))
                .AddOpenIdConnect(editProfilePolicy, GetOpenIdConnectOptions(editProfilePolicy))
                .AddCookie();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseAuthentication();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private Action<OpenIdConnectOptions> GetOpenIdConnectOptions(string policy)
            => options =>
            {
                options.MetadataAddress = $"https://login.microsoftonline.com/{Configuration["Auth-TenantName"]}/v2.0/.well-known/openid-configuration?p={policy}";
                options.ClientId = Configuration["Auth-ClientId"];
                options.ResponseType = OpenIdConnectResponseType.IdToken;
                options.CallbackPath = "/signin/" + policy;
                options.SignedOutCallbackPath = "/signout/" + policy;
                options.SignedOutRedirectUri = "/";
            };
    }
}
