using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Dispose;

/// <summary>
/// Test: Dispose() leert SelectedItems Collection.
/// </summary>
public sealed class Dispose_ClearsSelectedItems : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly ViewModels.CollectionViewModel<TestDto, TestViewModel> _sut;

    public Dispose_ClearsSelectedItems(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);
    }

    [Fact]
    public void Test_Dispose_ClearsSelectedItems()
    {
        // Arrange: Verify SelectedItems ist nicht leer
        Assert.Equal(2, _sut.SelectedItems.Count);

        // Act
        _sut.Dispose();

        // Assert
        Assert.Empty(_sut.SelectedItems);
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
