using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TSQ.Startup))]
namespace TSQ
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
