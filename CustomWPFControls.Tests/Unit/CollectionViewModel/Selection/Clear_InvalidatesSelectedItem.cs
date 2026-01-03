using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Selection;

/// <summary>
/// Test: Clear() invalidiert SelectedItem.
/// </summary>
public sealed class Clear_InvalidatesSelectedItem : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly CollectionViewModel<TestDto, TestViewModel> _sut;

    public Clear_InvalidatesSelectedItem(CollectionViewModelFixture fixture)
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
    public void Test_Clear_InvalidatesSelectedItem()
    {
        // Arrange: Verify SelectedItem ist gesetzt
        Assert.NotNull(_sut.SelectedItem);

        // Act
        _sut.Clear();

        // Assert
        Assert.Null(_sut.SelectedItem);
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
        _sut?.Dispose();
    }
}
