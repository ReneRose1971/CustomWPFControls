using System;
using System.Linq;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Selection;

/// <summary>
/// Test: RemoveRange() invalidiert SelectedItem wenn das selektierte Item entfernt wurde.
/// </summary>
public sealed class RemoveRange_InvalidatesSelectedItem_WhenSelectedItemWasRemoved : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly CollectionViewModel<TestDto, TestViewModel> _sut;

    public RemoveRange_InvalidatesSelectedItem_WhenSelectedItemWasRemoved(CollectionViewModelFixture fixture)
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

        // Setup: Item2 selektieren
        _sut.SelectedItem = _sut.Items[1];
    }

    [Fact]
    public void RemoveRange_InvalidatesSelectedItem()
    {
        // Arrange
        var itemsToRemove = _sut.Items.Take(2).ToList(); // Item1 und Item2

        // Act
        _sut.RemoveRange(itemsToRemove);

        // Assert: SelectedItem war Item2, sollte jetzt null sein
        Assert.Null(_sut.SelectedItem);
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
        _sut?.Dispose();
    }
}
