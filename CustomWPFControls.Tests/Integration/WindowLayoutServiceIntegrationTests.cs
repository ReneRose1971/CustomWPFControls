using System;
using System.Linq;
using System.Windows;
using CustomWPFControls.Services;
using DataStores.Abstractions;
using Xunit;
using FluentAssertions;

namespace CustomWPFControls.Tests.Integration;

/// <summary>
/// Integrationstests für <see cref="WindowLayoutService"/> mit DataStores.
/// HINWEIS: Diese Tests sind vorübergehend deaktiviert bis die vollständige DataStores-Integration abgeschlossen ist.
/// </summary>
public sealed class WindowLayoutServiceIntegrationTests : IDisposable
{
    // TODO: Tests für WindowLayoutService mit DataStores vollständig implementieren
    // Die ursprünglichen Tests verwendeten DataToolKit-spezifische Konzepte
    // und müssen für die neue DataStores-Architektur neu geschrieben werden.

    [Fact(Skip = "Muss für DataStores-Integration neu implementiert werden")]
    public void Placeholder_Test()
    {
        // Diese Tests werden im nächsten Schritt implementiert
        Assert.True(true);
    }

    public void Dispose()
    {
        // Cleanup wird in der vollständigen Implementierung hinzugefügt
    }
}
