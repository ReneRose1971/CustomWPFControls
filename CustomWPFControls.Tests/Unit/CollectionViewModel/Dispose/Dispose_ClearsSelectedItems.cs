using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Dispose;

/// <summary>
/// Test: Dispose() leert SelectedItems Collection.
/// </summary>
public sealed class Dispose_ClearsSelectedItems : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly CollectionViewModel<TestDto, TestViewModel> _sut;

    public Dispose_ClearsSelectedItems(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _sut = new CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        // Setup: Items hinzufügen und in SelectedItems einfügen
        _fixture.TestDtoStore.AddRange(new[]
        {
            new TestDto { Name = "Item1" },
            new TestDto { Name = "Item2" }
        });
        _sut.SelectedItems.Add(_sut.Items[0]);
        _sut.SelectedItems.Add(_sut.Items[1]);
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
