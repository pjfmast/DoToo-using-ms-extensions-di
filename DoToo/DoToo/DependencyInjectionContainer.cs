using DoToo.Repositories;
using DoToo.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace DoToo
{
    public static class DependencyInjectionContainer
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            //services.AddSingleton<IMyService, MyService>
            services.AddSingleton<TodoItemRepository>();
            return services;
        }

        public static IServiceCollection ConfigureViewModels(this IServiceCollection services)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            foreach (var type in currentAssembly.DefinedTypes
                .Where(e => e.IsSubclassOf(typeof(Page))
                         || e.IsSubclassOf(typeof(ViewModel))))
            {
                services.AddTransient(type.AsType());
            }
            return services;
        }
    }
}