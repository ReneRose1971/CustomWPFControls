using System;
using System.Linq;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.EdgeCases;

/// <summary>
/// Test: RemoveRange() mit leerer Liste gibt 0 zurück.
/// </summary>
public sealed class RemoveRange_WithEmptyList_ReturnsZero : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;

    public RemoveRange_WithEmptyList_ReturnsZero(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
        
        // Setup: Ein "Original Item" zum Store hinzufügen
        _fixture.Sut.ModelStore.Add(new TestDto { Name = "OriginalItem" });
    }

    [Fact]
    public void RemoveRange_EmptyList_ReturnsZero()
    {
        // Arrange
        var emptyList = Enumerable.Empty<TestViewModel>();

        // Act
        var result = _fixture.Sut.RemoveRange(emptyList);

        // Assert
        Assert.Equal(0, result);
        Assert.Equal(1, _fixture.Sut.Count); // Original Item bleibt
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
