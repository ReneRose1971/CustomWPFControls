using System;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Selection;

/// <summary>
/// Test: Remove() entfernt Item aus SelectedItems wenn es dort vorhanden war.
/// </summary>
public sealed class Remove_RemovesFromSelectedItems_WhenItemWasInSelection : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;

    public Remove_RemovesFromSelectedItems_WhenItemWasInSelection(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
        
        // Setup: 2 Items zum Store hinzufügen
        _fixture.Sut.ModelStore.AddRange(new[]
        {
            new TestDto { Name = "First" },
            new TestDto { Name = "Second" }
        });
        
        // Setup: Item zu SelectedItems hinzufügen
        _fixture.Sut.SelectedItems.Add(_fixture.Sut.Items[0]);
    }

    [Fact]
    public void Remove_RemovesFromSelectedItems()
    {
        // Arrange
        var itemToRemove = _fixture.Sut.Items[0];
        Assert.Single(_fixture.Sut.SelectedItems); // Verify Precondition

        // Act
        _fixture.Sut.Remove(itemToRemove);

        // Assert
        Assert.Empty(_fixture.Sut.SelectedItems);
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
