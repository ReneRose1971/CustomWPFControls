using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Selection;

/// <summary>
/// Test: Remove() invalidiert SelectedItem wenn das entfernte Item selektiert war.
/// </summary>
public sealed class Remove_InvalidatesSelectedItem_WhenItemWasSelected : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;

    public Remove_InvalidatesSelectedItem_WhenItemWasSelected(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
        
        // Setup: 2 Items zum Store hinzufügen
        _fixture.Sut.ModelStore.AddRange(new[]
        {
            new TestDto { Name = "First" },
            new TestDto { Name = "Second" }
        });
        
        // Setup: SelectedItem setzen
        _fixture.Sut.SelectedItem = _fixture.Sut.Items[0];
    }

    [Fact]
    public void Remove_InvalidatesSelectedItem()
    {
        // Arrange
        var itemToRemove = _fixture.Sut.SelectedItem!;

        // Act
        _fixture.Sut.Remove(itemToRemove);

        // Assert
        Assert.Null(_fixture.Sut.SelectedItem);
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
