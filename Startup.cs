using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(kidyx.com.Startup))]
namespace kidyx.com
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
