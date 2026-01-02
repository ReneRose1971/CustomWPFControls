using System.Reflection;
using Common.Bootstrap;
using CustomWPFControls.Services.Dialogs;
using Microsoft.Extensions.DependencyInjection;

namespace CustomWPFControls.Bootstrap
{
    /// <summary>
    /// Bootstrap-Decorator für CustomWPFControls.
    /// Erweitert den Bootstrap-Prozess um automatische Dialog-View- und ViewModel-Registrierung.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Decorator-Pattern:</b> Dieser Decorator folgt dem gleichen Pattern wie DataStoresBootstrapDecorator.
    /// Er wraps einen inneren IBootstrapWrapper und erweitert dessen Funktionalität.
    /// </para>
    /// <para>
    /// <b>Ausführungsreihenfolge:</b>
    /// </para>
    /// <list type="number">
    /// <item>Basis-Registrierungen durch inneren Wrapper (IServiceModule, EqualityComparer)</item>
    /// <item>DialogService-Registrierung</item>
    /// <item>Assembly-Scanning für IDialogView&lt;TViewModel&gt;</item>
    /// <item>Assembly-Scanning für IDialogViewModelMarker</item>
    /// </list>
    /// <para>
    /// <b>Verwendung:</b>
    /// </para>
    /// <code>
    /// var bootstrap = new CustomWPFControlsBootstrapDecorator(
    ///     new DataStoresBootstrapDecorator(
    ///         new DefaultBootstrapWrapper()));
    /// 
    /// bootstrap.RegisterServices(
    ///     builder.Services,
    ///     typeof(Program).Assembly);
    /// </code>
    /// </remarks>
    public sealed class CustomWPFControlsBootstrapDecorator : IBootstrapWrapper
    {
        private readonly IBootstrapWrapper _innerWrapper;

        /// <summary>
        /// Erstellt eine neue Instanz des CustomWPFControlsBootstrapDecorator.
        /// </summary>
        /// <param name="innerWrapper">Der innere Bootstrap-Wrapper (z.B. DefaultBootstrapWrapper oder DataStoresBootstrapDecorator)</param>
        /// <exception cref="System.ArgumentNullException">Wenn <paramref name="innerWrapper"/> null ist</exception>
        public CustomWPFControlsBootstrapDecorator(IBootstrapWrapper innerWrapper)
        {
            _innerWrapper = innerWrapper ?? throw new System.ArgumentNullException(nameof(innerWrapper));
        }

        /// <summary>
        /// Registriert Services aus den angegebenen Assemblies.
        /// </summary>
        /// <param name="services">ServiceCollection für DI-Registrierungen</param>
        /// <param name="assemblies">Zu scannende Assemblies</param>
        /// <remarks>
        /// <para>
        /// <b>Registrierungsprozess:</b>
        /// </para>
        /// <list type="number">
        /// <item><description>Basis-Registrierungen durch inneren Wrapper</description></item>
        /// <item><description>DialogService-Registrierung (IDialogService ? DialogService)</description></item>
        /// <item><description>Scan nach IDialogView&lt;TViewModel&gt;-Implementierungen</description></item>
        /// <item><description>Scan nach IDialogViewModelMarker-Implementierungen</description></item>
        /// </list>
        /// <para>
        /// <b>Beispiel-Registrierungen:</b>
        /// </para>
        /// <list type="bullet">
        /// <item>IDialogView&lt;CustomerEditViewModel&gt; ? CustomerEditDialog (via Scanning)</item>
        /// <item>CustomerEditViewModel ? CustomerEditViewModel (via Scanning)</item>
        /// </list>
        /// </remarks>
        public void RegisterServices(IServiceCollection services, params Assembly[] assemblies)
        {
            // 1. Basis-Registrierungen durch inneren Wrapper
            _innerWrapper.RegisterServices(services, assemblies);

            // 2. DialogService registrieren
            services.AddDialogService();

            // 3. Dialog-Views scannen und registrieren
            services.AddDialogViewsFromAssemblies(assemblies);

            // 4. Dialog-ViewModels scannen und registrieren
            services.AddDialogViewModelsFromAssemblies(assemblies);
        }
    }
}
