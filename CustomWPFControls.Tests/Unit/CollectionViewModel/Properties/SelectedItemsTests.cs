using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using System.Collections.ObjectModel;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Properties;

/// <summary>
/// Tests für SelectedItems Collection.
/// </summary>
/// <remarks>
/// Setup: 3 Items hinzufügen
/// </remarks>
public sealed class SelectedItemsTests : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly CollectionViewModel<TestDto, TestViewModel> _sut;

    public SelectedItemsTests(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();

        // Setup: 3 Items hinzufügen
        _fixture.TestDtoStore.AddRange(new[]
        {
            new TestDto { Name = "First" },
            new TestDto { Name = "Second" },
            new TestDto { Name = "Third" }
        });

        _sut = new CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);
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
    public void IsObservableCollection()
    {
        _sut.SelectedItems.Should().BeOfType<ObservableCollection<TestViewModel>>();
    }
}
