using Common.Bootstrap;
using CustomWPFControls.Services;
using CustomWPFControls.Services.Dialogs;
using CustomWPFControls.Services.MessageBoxes;
using Microsoft.Extensions.DependencyInjection;

namespace CustomWPFControls.Bootstrap;

/// <summary>
/// Service-Modul für CustomWPFControls.
/// Registriert Services.
/// </summary>
/// <remarks>
/// <para>
/// <b>Registrierte Services:</b>
/// </para>
/// <list type="bullet">
/// <item><see cref="WindowLayoutService"/> - Singleton für Window-Layout-Persistierung</item>
/// <item><see cref="IDialogService"/> - Singleton für ViewModel-basierte Dialoge (mit WindowLayoutService-Integration)</item>
/// <item><see cref="IMessageBoxService"/> - Singleton für testbare MessageBox-Anzeige</item>
/// </list>
/// <para>
/// <b>WindowLayoutService-Integration:</b> Der DialogService nutzt automatisch den WindowLayoutService
/// um Position und Größe aller Dialoge zu persistieren. Der Layout-Key wird aus dem ViewModel-Typ generiert
/// (z.B. "Dialog_CustomerEditViewModel").
/// </para>
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
        
        // MessageBoxService als Singleton
        services.AddSingleton<IMessageBoxService, MessageBoxService>();
        
        // DialogService mit WindowLayoutService-Integration
        // DialogService erhält WindowLayoutService via Constructor-Injection
        services.AddDialogService();
    }
}
