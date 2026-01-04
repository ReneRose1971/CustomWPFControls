using CustomWPFControls.Factories;
using CustomWPFControls.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CustomWPFControls.Bootstrap
{
    /// <summary>
    /// Extension-Methods für IServiceCollection zur Registrierung von ViewModels und Collections.
    /// </summary>
    public static class ViewModelServiceCollectionExtensions
    {
        /// <summary>
        /// Registriert ViewModelFactory, CollectionViewModel und EditableCollectionViewModel für TModel → TViewModel.
        /// </summary>
        /// <typeparam name="TModel">Model-Typ.</typeparam>
        /// <typeparam name="TViewModel">ViewModel-Typ (muss IViewModelWrapper&lt;TModel&gt; implementieren).</typeparam>
        /// <param name="services">Die IServiceCollection.</param>
        /// <returns>Die IServiceCollection für Fluent-API.</returns>
        /// <remarks>
        /// <para>
        /// Diese Extension registriert als Package:
        /// </para>
        /// <list type="bullet">
        /// <item><description>IViewModelFactory&lt;TModel, TViewModel&gt; als Singleton</description></item>
        /// <item><description>CollectionViewModel&lt;TModel, TViewModel&gt; als Transient</description></item>
        /// <item><description>EditableCollectionViewModel&lt;TModel, TViewModel&gt; als Transient</description></item>
        /// </list>
        /// <para>
        /// ViewModels werden als Transient registriert, da jede View-Instanz ihre eigene
        /// ViewModel-Instanz mit eigenem lokalen ModelStore benötigt.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// // Registriert komplettes ViewModel-Package
        /// services.AddViewModelPackage&lt;Customer, CustomerItemViewModel&gt;();
        /// 
        /// // Später auflösen:
        /// var factory = serviceProvider.GetRequiredService&lt;IViewModelFactory&lt;Customer, CustomerItemViewModel&gt;&gt;();
        /// var collectionVM = serviceProvider.GetRequiredService&lt;CollectionViewModel&lt;Customer, CustomerItemViewModel&gt;&gt;();
        /// var editableVM = serviceProvider.GetRequiredService&lt;EditableCollectionViewModel&lt;Customer, CustomerItemViewModel&gt;&gt;();
        /// </code>
        /// </example>
        public static IServiceCollection AddViewModelPackage<TModel, TViewModel>(
            this IServiceCollection services)
            where TModel : class
            where TViewModel : class, IViewModelWrapper<TModel>
        {
            // 1. ViewModelFactory (Singleton - wird von allen ViewModels geteilt)
            services.TryAddSingleton<IViewModelFactory<TModel, TViewModel>, 
                ViewModelFactory<TModel, TViewModel>>();
            
            // 2. CollectionViewModel (Transient - jede View bekommt eigene Instanz)
            services.AddTransient<CollectionViewModel<TModel, TViewModel>>();
            
            // 3. EditableCollectionViewModel (Transient - jede View bekommt eigene Instanz)
            services.AddTransient<EditableCollectionViewModel<TModel, TViewModel>>();
            
            return services;
        }
    }
}
