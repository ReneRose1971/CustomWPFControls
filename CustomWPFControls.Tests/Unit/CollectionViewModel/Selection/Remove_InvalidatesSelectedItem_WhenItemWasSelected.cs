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
    private readonly ViewModels.CollectionViewModel<TestDto, TestViewModel> _sut;

    public Remove_InvalidatesSelectedItem_WhenItemWasSelected(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);
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
