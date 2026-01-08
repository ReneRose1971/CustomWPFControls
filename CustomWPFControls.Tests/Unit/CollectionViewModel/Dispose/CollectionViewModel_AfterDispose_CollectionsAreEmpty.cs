using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Dispose;

/// <summary>
/// Szenario: CollectionViewModel wurde disposed.
/// </summary>
/// <remarks>
/// Testet den Zustand der Collections nach Dispose.
/// Shared Setup: Neue Instanz mit Daten erstellt und disposed.
/// Alle Tests prüfen verschiedene Aspekte des Zustands nach Dispose.
/// </remarks>
public sealed class CollectionViewModel_AfterDispose_CollectionsAreEmpty : IClassFixture<TestHelperCustomWPFControlsTestFixture>
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;
    private readonly CollectionViewModel<TestDto, TestViewModel> _sut;

    public CollectionViewModel_AfterDispose_CollectionsAreEmpty(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        
        // Shared Setup: Neue Instanz erstellen mit Daten
        _sut = _fixture.CreateCollectionViewModel();
        _sut.ModelStore.AddRange(new[]
        {
            new TestDto { Name = "First" },
            new TestDto { Name = "Second" }
        });
        
        // Item zu SelectedItems hinzufügen
        _sut.SelectedItems.Add(_sut.Items[0]);
        
        // Dispose aufrufen
        _sut.Dispose();
    }

    [Fact]
    public void Items_IsEmpty()
    {
        // Assert: Items wurde geleert
        _sut.Items.Should().BeEmpty();
    }

    [Fact]
    public void SelectedItems_IsEmpty()
    {
        // Assert: SelectedItems wurde geleert
        _sut.SelectedItems.Should().BeEmpty();
    }

    [Fact]
    public void Count_IsZero()
    {
        // Assert: Count ist 0
        _sut.Count.Should().Be(0);
    }
}
