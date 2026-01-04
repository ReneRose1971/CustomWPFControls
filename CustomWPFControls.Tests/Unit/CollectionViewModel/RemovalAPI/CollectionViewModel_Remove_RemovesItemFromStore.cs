using System;
using System.Linq;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.RemovalAPI;

/// <summary>
/// Szenario: Ein ViewModel wurde per Remove()-Methode entfernt.
/// </summary>
/// <remarks>
/// Testet die Remove()-API-Methode des CollectionViewModel.
/// Shared Setup: 1 Model hinzugefügt, dann per Remove() entfernt.
/// Alle Tests prüfen, dass sowohl ViewModel als auch Model entfernt wurden.
/// </remarks>
public sealed class CollectionViewModel_Remove_RemovesItemFromStore : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly bool _removeResult;

    public CollectionViewModel_Remove_RemovesItemFromStore(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();

        // Shared Setup: Model hinzufügen
        var model = new TestDto { Name = "Test1" };
        _fixture.Sut.ModelStore.Add(model);

        // ViewModel über CollectionViewModel.Remove() entfernen
        var viewModel = _fixture.Sut.Items.First();
        _removeResult = _fixture.Sut.Remove(viewModel);
    }

    [Fact]
    public void Remove_ReturnsTrue()
    {
        // Assert: Remove() gibt true zurück
        _removeResult.Should().BeTrue();
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
        // Assert: ViewModel wurde entfernt
        _fixture.Sut.Items.Should().BeEmpty();
    }

    [Fact]
    public void ModelStore_IsEmpty()
    {
        // Assert: Model wurde aus Store entfernt
        _fixture.Sut.ModelStore.Items.Should().BeEmpty();
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
