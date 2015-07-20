using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OsisModel.Startup))]
namespace OsisModel
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
