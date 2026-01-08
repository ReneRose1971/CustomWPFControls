using System;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Synchronization;

/// <summary>
/// Szenario: Ein Model wurde zum ModelStore hinzugefügt und anschließend wieder entfernt.
/// </summary>
/// <remarks>
/// Testet die automatische Synchronisation via TransformTo bei Remove-Operation.
/// Shared Setup: 1 Model hinzugefügt, dann entfernt.
/// Alle Tests prüfen, dass das ViewModel automatisch entfernt wurde.
/// </remarks>
public sealed class CollectionViewModel_ModelStoreRemove_SynchronizesItems : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;

    public CollectionViewModel_ModelStoreRemove_SynchronizesItems(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();

        // Shared Setup: Model hinzufügen
        var model = new TestDto { Name = "Test1" };
        _fixture.Sut.ModelStore.Add(model);

        // Model wieder entfernen
        _fixture.Sut.ModelStore.Remove(model);
    }

    [Fact]
    public void Count_IsZero()
    {
        // Assert: Count wurde aktualisiert
        _fixture.Sut.Count.Should().Be(0);
    }

    [Fact]
    public void Items_IsEmpty()
    {
        // Assert: ViewModel wurde automatisch entfernt
        _fixture.Sut.Items.Should().BeEmpty();
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
