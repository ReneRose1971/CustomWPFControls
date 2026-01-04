using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Selection;

/// <summary>
/// Test: Clear() leert SelectedItems Collection.
/// </summary>
public sealed class Clear_ClearsSelectedItems : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly ViewModels.CollectionViewModel<TestDto, TestViewModel> _sut;

    public Clear_ClearsSelectedItems(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        // Setup: Items hinzufügen
        _fixture.TestDtoStore.AddRange(new[]
        {
            new TestDto { Name = "Item1" },
            new TestDto { Name = "Item2" }
        });

        // Setup: Items zu SelectedItems hinzufügen
        _sut.SelectedItems.Add(_sut.Items[0]);
        _sut.SelectedItems.Add(_sut.Items[1]);
    }

    [Fact]
    public void Test_Clear_ClearsSelectedItems()
    {
        // Arrange: Verify SelectedItems ist nicht leer
        Assert.Equal(2, _sut.SelectedItems.Count);

        // Act
        _sut.Clear();

        // Assert
        Assert.Empty(_sut.SelectedItems);
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
        _sut?.Dispose();
    }
}
