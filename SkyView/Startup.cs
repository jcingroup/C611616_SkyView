using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SkyView.Startup))]
namespace SkyView
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
