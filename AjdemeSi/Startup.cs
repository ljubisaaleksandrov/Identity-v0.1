using AjdemeSi.Services.Interfaces.Identity;
using AjdemeSi.Services.Logic.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AjdemeSi.Startup))]
namespace AjdemeSi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IAspNetUsersService>(sp => new AspNetUsersService());
        }
    }

}
