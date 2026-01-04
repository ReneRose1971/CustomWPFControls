using System;
using System.Collections.ObjectModel;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Properties.SelectedItems;

/// <summary>
/// Szenario: SelectedItems nach Initialisierung des CollectionViewModel (keine Selektion).
/// </summary>
/// <remarks>
/// Testet den Initialzustand der SelectedItems-Collection.
/// Shared Setup: Keine Items selektiert.
/// Alle Tests prüfen verschiedene Aspekte des initialen Zustands.
/// </remarks>
public sealed class CollectionViewModel_SelectedItemsInitial_IsEmptyObservableCollection : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;

    public CollectionViewModel_SelectedItemsInitial_IsEmptyObservableCollection(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
    }

    [Fact]
    public void SelectedItems_IsNotNull()
    {
        // Assert: SelectedItems ist initialisiert
        _fixture.Sut.SelectedItems.Should().NotBeNull();
    }

    [Fact]
    public void SelectedItems_IsEmptyInitially()
    {
        // Assert: SelectedItems ist initial leer
        _fixture.Sut.SelectedItems.Should().BeEmpty();
    }

    [Fact]
    public void SelectedItems_IsObservableCollection()
    {
        // Assert: SelectedItems ist vom Typ ObservableCollection
        _fixture.Sut.SelectedItems.Should().BeOfType<ObservableCollection<TestViewModel>>();
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
