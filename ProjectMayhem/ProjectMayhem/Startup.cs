using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProjectMayhem.Startup))]
namespace ProjectMayhem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
