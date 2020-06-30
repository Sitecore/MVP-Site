using Microsoft.Extensions.DependencyInjection;
using Mvp.Feature.BasicContent.Services;
using Sitecore.DependencyInjection;


namespace Mvp.Feature.BasicContent
{
    public class ServicesConfigurator : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IContentListBuilder, ContentListBuilder>();
        }
    }
}