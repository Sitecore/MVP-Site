using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

namespace Mvp.Foundation.People
{
    public class ServicesConfigurator : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            //serviceCollection.AddTransient<IMVPListBuilder, MVPListBuilder>();
        }
    }
}