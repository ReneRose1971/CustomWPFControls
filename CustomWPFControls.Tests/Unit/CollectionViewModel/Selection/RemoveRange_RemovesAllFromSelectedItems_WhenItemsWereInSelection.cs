using System;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Selection;

/// <summary>
/// Test: RemoveRange() entfernt alle betroffenen Items aus SelectedItems.
/// </summary>
public sealed class RemoveRange_RemovesAllFromSelectedItems_WhenItemsWereInSelection : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;

    public RemoveRange_RemovesAllFromSelectedItems_WhenItemsWereInSelection(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();

        // Setup: Items hinzufügen
        _fixture.Sut.ModelStore.AddRange(new[]
        {
            new TestDto { Name = "Item1" },
            new TestDto { Name = "Item2" },
            new TestDto { Name = "Item3" }
        });

        // Setup: Item1 und Item2 zu SelectedItems hinzufügen
        _fixture.Sut.SelectedItems.Add(_fixture.Sut.Items[0]);
        _fixture.Sut.SelectedItems.Add(_fixture.Sut.Items[1]);
    }

    [Fact]
    public void RemoveRange_RemovesAllFromSelectedItems()
    {
        // Arrange
        var itemsToRemove = _fixture.Sut.Items.Take(2).ToList(); // Item1 und Item2
        Assert.Equal(2, _fixture.Sut.SelectedItems.Count); // Verify Precondition

        // Act
        _fixture.Sut.RemoveRange(itemsToRemove);

        // Assert
        Assert.Empty(_fixture.Sut.SelectedItems);
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
