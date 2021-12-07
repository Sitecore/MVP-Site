using Microsoft.Extensions.DependencyInjection;
using Mvp.Feature.Social.Services;
using Sitecore.DependencyInjection;


namespace Mvp.Feature.Social
{
    public class ServicesConfigurator : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IRssBuilder, RssBuilder>();
        }
    }
}