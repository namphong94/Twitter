using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(KMS.TwitterAPI.Startup))]
namespace KMS.TwitterAPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            
        }
    }
}
