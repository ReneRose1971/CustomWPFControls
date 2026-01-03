using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.EdgeCases;

/// <summary>
/// Test: RemoveRange() mit null-Parameter wirft ArgumentNullException.
/// </summary>
public sealed class RemoveRange_WithNull_ThrowsArgumentNullException : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly CollectionViewModel<TestDto, TestViewModel> _sut;

    public RemoveRange_WithNull_ThrowsArgumentNullException(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _sut = new CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);
    }

    [Fact]
    public void RemoveRange_NullParameter_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _sut.RemoveRange(null!));
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
        _sut?.Dispose();
    }
}
