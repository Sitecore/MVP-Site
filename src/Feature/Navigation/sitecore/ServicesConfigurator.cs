using Microsoft.Extensions.DependencyInjection;
using Mvp.Feature.Navigation.Services;
using Sitecore.DependencyInjection;

namespace Mvp.Feature.Navigation
{
    public class ServicesConfigurator : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ITopLinksBuilder, TopLinksBuilder>();
            serviceCollection.AddTransient<INavigationBuilder, NavigationBuilder>();
        }
    }
}