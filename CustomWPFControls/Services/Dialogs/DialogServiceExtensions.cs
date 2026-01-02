using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace CustomWPFControls.Services.Dialogs
{
    /// <summary>
    /// Extension-Methods für die DI-Registrierung des DialogService und Assembly-Scanning.
    /// </summary>
    public static class DialogServiceExtensions
    {
        /// <summary>
        /// Registriert IDialogService im ServiceCollection.
        /// </summary>
        public static IServiceCollection AddDialogService(this IServiceCollection services)
        {
            services.AddSingleton<IDialogService>(provider =>
            {
                var layoutService = provider.GetService<WindowLayoutService>();
                return new DialogService(provider, layoutService);
            });

            return services;
        }

        /// <summary>
        /// Scannt die angegebenen Assemblies nach IDialogView&lt;TViewModel&gt;-Implementierungen
        /// und registriert diese automatisch im DI-Container.
        /// </summary>
        /// <param name="services">ServiceCollection</param>
        /// <param name="assemblies">Zu scannende Assemblies</param>
        /// <returns>ServiceCollection für Fluent-API</returns>
        /// <remarks>
        /// <para>
        /// Findet alle Window-Typen, die IDialogView&lt;TViewModel&gt; implementieren
        /// und registriert sie als: services.AddTransient&lt;IDialogView&lt;TViewModel&gt;, ConcreteView&gt;()
        /// </para>
        /// <para>
        /// <b>Beispiel:</b> CustomerEditDialog : IDialogView&lt;CustomerEditViewModel&gt;
        /// wird registriert als: IDialogView&lt;CustomerEditViewModel&gt; ? CustomerEditDialog
        /// </para>
        /// </remarks>
        public static IServiceCollection AddDialogViewsFromAssemblies(
            this IServiceCollection services,
            params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                var dialogViews = assembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract)
                    .Where(t => typeof(Window).IsAssignableFrom(t))
                    .Where(t => t.GetInterfaces().Any(i =>
                        i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IDialogView<>)));

                foreach (var viewType in dialogViews)
                {
                    var dialogViewInterface = viewType.GetInterfaces()
                        .FirstOrDefault(i =>
                            i.IsGenericType &&
                            i.GetGenericTypeDefinition() == typeof(IDialogView<>));

                    if (dialogViewInterface != null)
                    {
                        // Registrierung: IDialogView<CustomerEditViewModel> ? CustomerEditDialog
                        services.AddTransient(dialogViewInterface, viewType);
                    }
                }
            }

            return services;
        }

        /// <summary>
        /// Scannt die angegebenen Assemblies nach IDialogViewModelMarker-Implementierungen
        /// und registriert diese automatisch als Transient im DI-Container.
        /// </summary>
        /// <param name="services">ServiceCollection</param>
        /// <param name="assemblies">Zu scannende Assemblies</param>
        /// <returns>ServiceCollection für Fluent-API</returns>
        /// <remarks>
        /// <para>
        /// Findet alle Klassen, die IDialogViewModelMarker implementieren
        /// und registriert sie als: services.AddTransient&lt;ConcreteViewModel&gt;()
        /// </para>
        /// <para>
        /// <b>Warum Transient?</b> Jeder Dialog-Aufruf erstellt eine neue ViewModel-Instanz.
        /// </para>
        /// <para>
        /// <b>Beispiel:</b> CustomerEditViewModel : IDialogViewModelMarker
        /// wird registriert als: services.AddTransient&lt;CustomerEditViewModel&gt;()
        /// </para>
        /// </remarks>
        public static IServiceCollection AddDialogViewModelsFromAssemblies(
            this IServiceCollection services,
            params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                var dialogViewModels = assembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract)
                    .Where(t => typeof(IDialogViewModelMarker).IsAssignableFrom(t));

                foreach (var viewModelType in dialogViewModels)
                {
                    // Registrierung: CustomerEditViewModel als Transient
                    services.AddTransient(viewModelType);
                }
            }

            return services;
        }
    }
}
