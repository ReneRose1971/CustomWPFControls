using Common.Bootstrap;
using CustomWPFControls.Factories;
using Microsoft.Extensions.DependencyInjection;
using TestHelper.DataStores.Models;
using CustomWPFControls.Bootstrap;

namespace CustomWPFControls.Tests.Testing.Bootstrap;

/// <summary>
/// Service-Modul für CustomWPFControls.Tests mit automatischer Service-Registrierung.
/// </summary>
/// <remarks>
/// Dieses Modul wird automatisch durch <see cref="DefaultBootstrapWrapper"/> 
/// gefunden und ausgeführt, wenn es über RegisterServices gescannt wird.
/// Benötigt einen parameterlosen Konstruktor für das automatische Scanning.
/// </remarks>
public sealed class CustomWPFControlsTestServiceModule : IServiceModule
{
    /// <summary>
    /// Erstellt ein neues CustomWPFControlsTestServiceModule.
    /// </summary>
    /// <remarks>
    /// Parameterloser Konstruktor ist erforderlich für das automatische Assembly-Scanning
    /// durch den DefaultBootstrapWrapper.
    /// </remarks>
    public CustomWPFControlsTestServiceModule()
    {
    }

    /// <summary>
    /// Registriert Test-Services in der ServiceCollection.
    /// </summary>
    /// <param name="services">Die ServiceCollection für Dependency Injection.</param>
    public void Register(IServiceCollection services)
    {
        // ViewModelFactory für TestDto/TestViewModel registrieren
        services.AddViewModelPackage<TestDto, TestViewModel>();
    }
}
