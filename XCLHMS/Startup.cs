using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(XCLHMS.Startup))]
namespace XCLHMS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
