using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Host.Data;
using Host.Models;
using Host.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;
using System.IO;

namespace Host
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();
        
            //X509Certificate2 signingCertificate = FindCertificateByThumbprint("45823A6F0133E057C84D24C6FCFE4A1650028591");
            //var certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                //.AddSigningCredential(signingCertificate)
                .AddInMemoryIdentityResources(Resources.GetIdentityResources())
                .AddInMemoryApiResources(Resources.GetApiResources())
                .AddInMemoryClients(Clients.Get())
                .AddAspNetIdentity<ApplicationUser>();

            //services.AddAuthentication()
            //    .AddGoogle(options =>
            //    {
            //        options.ClientId = "998042782978-s07498t8i8jas7npj4crve1skpromf37.apps.googleusercontent.com";
            //        options.ClientSecret = "HsnwJri_53zn7VcO1Fm7THBb";
            //    });

            // for testing security stamp claims generation
            //services.Configure<SecurityStampValidatorOptions>(options =>
            //{
            //    options.ValidationInterval = TimeSpan.FromSeconds(30);
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            //app.UseAuthentication(); // UseAuthentication not needed -- UseIdentityServer add this
            app.UseIdentityServer();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        public static X509Certificate2 FindCertificateByThumbprint(string findValue)
        {
            //if (env.IsDevelopment())
            //{
            //    var certPath = Path.Combine(".", "certs", "IdentityServer4Auth.pfx");

            //    return new X509Certificate2(certPath, "secret");
            //}
            //else
            //{
                X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);

                try
                {
                    store.Open(OpenFlags.ReadOnly);
                    X509Certificate2Collection col = store.Certificates.Find(X509FindType.FindByThumbprint,
                        findValue, false); // Don't validate certs, since the test root isn't installed.
                    if (col == null || col.Count == 0)
                        return null;
                    return col[0];
                }
                finally
                {

                    store = null;
                }
            //}
        }
    }
}
