using System;
using System.Collections.ObjectModel;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Properties;

/// <summary>
/// Tests für SelectedItems Collection.
/// </summary>
/// <remarks>
/// Setup: 3 Items hinzufügen
/// </remarks>
public sealed class SelectedItemsTests : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly ViewModels.CollectionViewModel<TestDto, TestViewModel> _sut;

    public SelectedItemsTests(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);
    }

    [Fact]
    public void CanAddItems()
    {
        // Act
        _sut.SelectedItems.Add(_sut.Items[0]);
        _sut.SelectedItems.Add(_sut.Items[2]);

        // Assert
        _sut.SelectedItems.Should().HaveCount(2);
        _sut.SelectedItems.Should().Contain(_sut.Items[0]);
        _sut.SelectedItems.Should().Contain(_sut.Items[2]);
    }

    [Fact]
    public void CanRemoveItems()
    {
        // Arrange
        _sut.SelectedItems.Add(_sut.Items[0]);
        _sut.SelectedItems.Add(_sut.Items[1]);

        // Act
        _sut.SelectedItems.Remove(_sut.Items[0]);

        // Assert
        _sut.SelectedItems.Should().ContainSingle();
        _sut.SelectedItems.Should().Contain(_sut.Items[1]);
    }

    [Fact]
    public void CanClearItems()
    {
        // Arrange
        _sut.SelectedItems.Add(_sut.Items[0]);
        _sut.SelectedItems.Add(_sut.Items[1]);

        // Act
        _sut.SelectedItems.Clear();

        // Assert
        _sut.SelectedItems.Should().BeEmpty();
    }

    [Fact]
    public void SelectedItems_IsNotNull()
    {
        _sut.SelectedItems.Should().NotBeNull();
    }

    [Fact]
    public void SelectedItems_IsEmptyInitially()
    {
        _sut.SelectedItems.Should().BeEmpty();
    }

    [Fact]
    public void SelectedItems_IsObservableCollection()
    {
        _sut.SelectedItems.Should().BeOfType<ObservableCollection<TestViewModel>>();
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
        _sut?.Dispose();
    }
}
