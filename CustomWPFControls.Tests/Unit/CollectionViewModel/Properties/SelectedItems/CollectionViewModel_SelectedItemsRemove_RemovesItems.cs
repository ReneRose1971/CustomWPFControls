using System;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Properties.SelectedItems;

/// <summary>
/// Szenario: Ein Item wurde aus SelectedItems entfernt.
/// </summary>
/// <remarks>
/// Testet die Remove-Funktionalität der SelectedItems-Collection.
/// Shared Setup: 3 Models im Store, 2 Items zu SelectedItems hinzugefügt, dann 1 Item entfernt.
/// Alle Tests prüfen verschiedene Aspekte nach dem Entfernen.
/// </remarks>
public sealed class CollectionViewModel_SelectedItemsRemove_RemovesItems : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;
    private readonly TestViewModel _remainingItem;

    public CollectionViewModel_SelectedItemsRemove_RemovesItems(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();

        // Shared Setup: 3 Items zum Store hinzufügen
        _fixture.Sut.ModelStore.AddRange(new[]
        {
            new TestDto { Name = "First" },
            new TestDto { Name = "Second" },
            new TestDto { Name = "Third" }
        });

        // 2 Items zu SelectedItems hinzufügen
        _fixture.Sut.SelectedItems.Add(_fixture.Sut.Items[0]);
        _fixture.Sut.SelectedItems.Add(_fixture.Sut.Items[1]);

        // Erstes Item entfernen
        _remainingItem = _fixture.Sut.Items[1];
        _fixture.Sut.SelectedItems.Remove(_fixture.Sut.Items[0]);
    }

    [Fact]
    public void SelectedItems_ContainsSingleItem()
    {
        // Assert: Nur noch 1 Item in SelectedItems
        _fixture.Sut.SelectedItems.Should().ContainSingle();
    }

    [Fact]
    public void SelectedItems_ContainsRemainingItem()
    {
        // Assert: Das verbliebene Item ist das Richtige
        _fixture.Sut.SelectedItems.Should().Contain(_remainingItem);
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
