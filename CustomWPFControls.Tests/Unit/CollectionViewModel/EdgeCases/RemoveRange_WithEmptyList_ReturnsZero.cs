using System;
using System.Linq;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.EdgeCases;

/// <summary>
/// Test: RemoveRange() mit leerer Liste gibt 0 zurück.
/// </summary>
public sealed class RemoveRange_WithEmptyList_ReturnsZero : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly CollectionViewModel<TestDto, TestViewModel> _sut;

    public RemoveRange_WithEmptyList_ReturnsZero(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _sut = new CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        // Setup: Items hinzufügen
        _fixture.TestDtoStore.Add(new TestDto { Name = "Test" });
    }

    [Fact]
    public void RemoveRange_EmptyList_ReturnsZero()
    {
        // Arrange
        var emptyList = Enumerable.Empty<TestViewModel>();

        // Act
        var result = _sut.RemoveRange(emptyList);

        // Assert
        Assert.Equal(0, result);
        Assert.Equal(1, _sut.Count); // Original Item bleibt
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
        _sut?.Dispose();
    }
}
