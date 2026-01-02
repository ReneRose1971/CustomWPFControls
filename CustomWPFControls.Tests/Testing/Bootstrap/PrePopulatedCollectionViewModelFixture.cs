using TestHelper.DataStores.Models;

namespace CustomWPFControls.Tests.Testing.Bootstrap;

/// <summary>
/// Test-Fixture für CollectionViewModel-Tests mit vorausgefüllten Testdaten.
/// </summary>
/// <remarks>
/// Diese Fixture erbt von <see cref="CollectionViewModelFixture"/> und initialisiert
/// den TestDtoStore mit vorgefertigten Testdaten während des Bootstrap-Prozesses.
/// 
/// Vorausgefüllte Daten:
/// - 3 TestDto-Objekte mit unterschiedlichen Namen
/// - Zugriff über TestData Property
/// </remarks>
public sealed class PrePopulatedCollectionViewModelFixture : CollectionViewModelFixture
{
    /// <summary>
    /// Die vorausgefüllten Testdaten (readonly).
    /// </summary>
    public IReadOnlyList<TestDto> TestData { get; private set; } = null!;

    /// <summary>
    /// Erstellt die Fixture und führt den Bootstrap-Prozess mit Testdaten aus.
    /// </summary>
    public PrePopulatedCollectionViewModelFixture()
    {
        // Basisklasse führt Bootstrap aus und ruft InitializeData() auf
    }

    /// <summary>
    /// Initialisiert die Testdaten nach der Service-Initialisierung.
    /// </summary>
    /// <remarks>
    /// Füllt den TestDtoStore mit 3 vordefinierten TestDto-Objekten.
    /// </remarks>
    protected override void InitializeData()
    {
        // 3 Testdaten-Objekte erstellen
        var testData = new[]
        {
            new TestDto { Name = "FirstItem" },
            new TestDto { Name = "SecondItem" },
            new TestDto { Name = "ThirdItem" }
        };

        // In den Store einfügen
        TestDtoStore.AddRange(testData);

        // Für externen Zugriff speichern
        TestData = testData;
    }
}
