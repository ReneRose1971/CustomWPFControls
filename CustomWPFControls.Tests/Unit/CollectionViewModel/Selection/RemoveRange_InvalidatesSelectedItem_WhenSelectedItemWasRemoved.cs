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
    private readonly ViewModels.CollectionViewModel<TestDto, TestViewModel> _sut;

    public RemoveRange_InvalidatesSelectedItem_WhenSelectedItemWasRemoved(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);
        
        // Setup: Add test data
        _fixture.TestDtoStore.AddRange(new[]
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
        _sut.SelectedItem = _sut.Items.First();
        var itemsToRemove = _sut.Items.Take(2).ToList();

        // Act
        _sut.RemoveRange(itemsToRemove);

        // Assert
        _sut.SelectedItem.Should().BeNull();
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
        _sut?.Dispose();
    }
}
