using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Selection;

/// <summary>
/// Test: Remove() invalidiert SelectedItem wenn das entfernte Item selektiert war.
/// </summary>
public sealed class Remove_InvalidatesSelectedItem_WhenItemWasSelected : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly CollectionViewModel<TestDto, TestViewModel> _sut;

    public Remove_InvalidatesSelectedItem_WhenItemWasSelected(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _sut = new CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        // Setup: Item hinzufügen und selektieren
        _fixture.TestDtoStore.Add(new TestDto { Name = "SelectedItem" });
        _sut.SelectedItem = _sut.Items[0];
    }

    [Fact]
    public void Remove_InvalidatesSelectedItem()
    {
        // Arrange
        var itemToRemove = _sut.SelectedItem!;

        // Act
        _sut.Remove(itemToRemove);

        // Assert
        Assert.Null(_sut.SelectedItem);
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
        _sut?.Dispose();
    }
}
