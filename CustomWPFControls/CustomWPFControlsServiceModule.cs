using Common.Bootstrap;
using DataStores.Bootstrap;
using Microsoft.Extensions.DependencyInjection;
using CustomWPFControls.Services;
using CustomWPFControls.Bootstrap;

namespace CustomWPFControls;

/// <summary>
/// Service-Modul für CustomWPFControls.
/// Registriert Services und DataStore-Registrar.
/// </summary>
/// <remarks>
/// <para>
/// <b>DataStore-Initialisierung:</b> DataStores werden NICHT hier initialisiert,
/// sondern durch <see cref="DataStoreBootstrap"/> nach dem Build des Containers.
/// </para>
/// </remarks>
public sealed class CustomWPFControlsServiceModule : IServiceModule
{
    public void Register(IServiceCollection services)
    {
        // DataStoreRegistrar für WindowLayoutData registrieren
        services.AddDataStoreRegistrar<WindowLayoutDataStoreRegistrar>();

        // WindowLayoutService als Singleton
        services.AddSingleton<WindowLayoutService>();
    }
}
