using System;
using System.Linq;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.RemoveAPI;

public sealed class RemoveRangeTests : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly ViewModels.CollectionViewModel<TestDto, TestViewModel> _sut;

    public RemoveRangeTests(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);
        
        // Setup: 5 Items hinzufügen
        _fixture.TestDtoStore.AddRange(new[]
        {
            new TestDto { Name = "First" },
            new TestDto { Name = "Second" },
            new TestDto { Name = "Third" },
            new TestDto { Name = "Fourth" },
            new TestDto { Name = "Fifth" }
        });
    }

    [Fact]
    public void RemovesMultipleViewModels()
    {
        // Arrange
        var itemsToRemove = _sut.Items.Take(2).ToList();

        // Act
        _sut.RemoveRange(itemsToRemove);

        // Assert
        _sut.Items.Should().NotContain(itemsToRemove);
    }

    [Fact]
    public void CountIsThree()
    {
        // Arrange
        var itemsToRemove = _sut.Items.Take(2).ToList();

        // Act
        _sut.RemoveRange(itemsToRemove);

        // Assert
        _sut.Count.Should().Be(3);
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
        _sut?.Dispose();
    }
}
