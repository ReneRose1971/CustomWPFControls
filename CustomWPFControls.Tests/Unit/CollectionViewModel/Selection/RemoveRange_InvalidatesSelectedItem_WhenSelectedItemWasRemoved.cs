using System;
using System.Linq;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Selection;

/// <summary>
/// Test: RemoveRange() invalidiert SelectedItem wenn das selektierte Item entfernt wurde.
/// </summary>
public sealed class RemoveRange_InvalidatesSelectedItem_WhenSelectedItemWasRemoved : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;

    public RemoveRange_InvalidatesSelectedItem_WhenSelectedItemWasRemoved(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
        
        // Setup: Add test data
        _fixture.Sut.ModelStore.AddRange(new[]
        {
            new TestDto { Name = "First" },
            new TestDto { Name = "Second" },
            new TestDto { Name = "Third" }
        });
    }

    [Fact]
    public void Test_RemoveRange_InvalidatesSelectedItem()
    {
        // Arrange
        _fixture.Sut.SelectedItem = _fixture.Sut.Items.First();
        var itemsToRemove = _fixture.Sut.Items.Take(2).ToList();

        // Act
        _fixture.Sut.RemoveRange(itemsToRemove);

        // Assert
        _fixture.Sut.SelectedItem.Should().BeNull();
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
