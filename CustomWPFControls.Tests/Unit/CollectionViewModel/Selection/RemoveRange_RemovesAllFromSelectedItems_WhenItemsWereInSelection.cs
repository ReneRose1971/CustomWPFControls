using System;
using System.Linq;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Selection;

/// <summary>
/// Test: RemoveRange() entfernt alle betroffenen Items aus SelectedItems.
/// </summary>
public sealed class RemoveRange_RemovesAllFromSelectedItems_WhenItemsWereInSelection : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly CollectionViewModel<TestDto, TestViewModel> _sut;

    public RemoveRange_RemovesAllFromSelectedItems_WhenItemsWereInSelection(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _sut = new CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        // Setup: Items hinzufügen
        _fixture.TestDtoStore.AddRange(new[]
        {
            new TestDto { Name = "Item1" },
            new TestDto { Name = "Item2" },
            new TestDto { Name = "Item3" }
        });

        // Setup: Item1 und Item2 zu SelectedItems hinzufügen
        _sut.SelectedItems.Add(_sut.Items[0]);
        _sut.SelectedItems.Add(_sut.Items[1]);
    }

    [Fact]
    public void RemoveRange_RemovesAllFromSelectedItems()
    {
        // Arrange
        var itemsToRemove = _sut.Items.Take(2).ToList(); // Item1 und Item2
        Assert.Equal(2, _sut.SelectedItems.Count); // Verify Precondition

        // Act
        _sut.RemoveRange(itemsToRemove);

        // Assert
        Assert.Empty(_sut.SelectedItems);
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
        _sut?.Dispose();
    }
}
