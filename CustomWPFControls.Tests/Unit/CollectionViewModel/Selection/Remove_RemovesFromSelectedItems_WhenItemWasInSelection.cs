using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Selection;

/// <summary>
/// Test: Remove() entfernt Item aus SelectedItems wenn es dort vorhanden war.
/// </summary>
public sealed class Remove_RemovesFromSelectedItems_WhenItemWasInSelection : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly CollectionViewModel<TestDto, TestViewModel> _sut;

    public Remove_RemovesFromSelectedItems_WhenItemWasInSelection(CollectionViewModelFixture fixture)
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
            new TestDto { Name = "Item2" }
        });

        // Setup: Item zu SelectedItems hinzufügen
        _sut.SelectedItems.Add(_sut.Items[0]);
    }

    [Fact]
    public void Remove_RemovesFromSelectedItems()
    {
        // Arrange
        var itemToRemove = _sut.Items[0];
        Assert.Single(_sut.SelectedItems); // Verify Precondition

        // Act
        _sut.Remove(itemToRemove);

        // Assert
        Assert.Empty(_sut.SelectedItems);
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
        _sut?.Dispose();
    }
}
