using Common.Bootstrap;
using CustomWPFControls.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CustomWPFControls.Bootstrap;

/// <summary>
/// Service-Modul für CustomWPFControls.
/// Registriert Services.
/// </summary>
/// <remarks>
/// <para>
/// <b>DataStore-Initialisierung:</b> Der WindowLayoutDataStoreRegistrar wird automatisch
/// durch das Assembly-Scanning des DataStoresBootstrapDecorator gefunden und registriert.
/// DataStores werden durch <see cref="DataStores.Bootstrap.DataStoreBootstrap"/> nach dem
/// Build des Containers initialisiert.
/// </para>
/// </remarks>
public sealed class CustomWPFControlsServiceModule : IServiceModule
{
    public void Register(IServiceCollection services)
    {
        // WindowLayoutService als Singleton
        // (Der WindowLayoutDataStoreRegistrar wird automatisch durch Assembly-Scanning gefunden)
        services.AddSingleton<WindowLayoutService>();
    }
}
