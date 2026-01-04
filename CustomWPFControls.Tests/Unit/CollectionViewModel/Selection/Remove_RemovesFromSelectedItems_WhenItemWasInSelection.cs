using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
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
    private readonly ViewModels.CollectionViewModel<TestDto, TestViewModel> _sut;

    public Remove_RemovesFromSelectedItems_WhenItemWasInSelection(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);
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
