using System;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.RemovalAPI;

/// <summary>
/// Szenario: Alle ViewModels wurden per Clear()-Methode entfernt.
/// </summary>
/// <remarks>
/// Testet die Clear()-API-Methode des CollectionViewModel.
/// Shared Setup: 2 Models hinzugefügt, dann per Clear() alle entfernt.
/// Alle Tests prüfen, dass alle ViewModels und Models entfernt wurden.
/// </remarks>
public sealed class CollectionViewModel_Clear_RemovesAllItems : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;

    public CollectionViewModel_Clear_RemovesAllItems(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();

        // Shared Setup: 2 Models hinzufügen
        var models = new[]
        {
            new TestDto { Name = "Model1" },
            new TestDto { Name = "Model2" }
        };
        _fixture.Sut.ModelStore.AddRange(models);

        // Alle per Clear() entfernen
        _fixture.Sut.Clear();
    }

    [Fact]
    public void Count_IsZero()
    {
        // Assert: Count ist 0
        _fixture.Sut.Count.Should().Be(0);
    }

    [Fact]
    public void Items_IsEmpty()
    {
        // Assert: Items ist leer
        _fixture.Sut.Items.Should().BeEmpty();
    }

    [Fact]
    public void ModelStore_IsEmpty()
    {
        // Assert: ModelStore ist leer
        _fixture.Sut.ModelStore.Items.Should().BeEmpty();
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
