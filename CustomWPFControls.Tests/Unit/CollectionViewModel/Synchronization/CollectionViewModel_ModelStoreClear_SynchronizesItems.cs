using System;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Synchronization;

/// <summary>
/// Szenario: Models wurden zum ModelStore hinzugefügt und dann per Clear entfernt.
/// </summary>
/// <remarks>
/// Testet die automatische Synchronisation via TransformTo bei Clear-Operation.
/// Shared Setup: 2 Models hinzugefügt, dann Clear aufgerufen.
/// Alle Tests prüfen, dass alle ViewModels automatisch entfernt wurden.
/// </remarks>
public sealed class CollectionViewModel_ModelStoreClear_SynchronizesItems : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;

    public CollectionViewModel_ModelStoreClear_SynchronizesItems(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();

        // Shared Setup: Models hinzufügen
        var models = new[]
        {
            new TestDto { Name = "Model1" },
            new TestDto { Name = "Model2" }
        };
        _fixture.Sut.ModelStore.AddRange(models);

        // Store leeren
        _fixture.Sut.ModelStore.Clear();
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
        // Assert: Alle ViewModels wurden automatisch entfernt
        _fixture.Sut.Items.Should().BeEmpty();
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
