using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using System.Collections.Specialized;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Properties;

/// <summary>
/// Tests für CollectionChanged Event von SelectedItems.
/// </summary>
/// <remarks>
/// SelectedItems ist ObservableCollection und sollte CollectionChanged feuern.
/// </remarks>
public sealed class SelectedItems_RaisesCollectionChanged : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;

    public SelectedItems_RaisesCollectionChanged(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
    }

    [Fact]
    public void ShouldRaiseCollectionChangedOnAdd()
    {
        // Arrange
        _fixture.TestDtoStore.Add(new TestDto { Name = "Test" });

        var sut = new CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        NotifyCollectionChangedAction? capturedAction = null;
        sut.SelectedItems.CollectionChanged += (_, e) =>
        {
            capturedAction = e.Action;
        };

        // Act
        sut.SelectedItems.Add(sut.Items.Single());

        // Assert
        capturedAction.Should().Be(NotifyCollectionChangedAction.Add);

        // Cleanup
        _fixture.ClearTestData();
        sut.Dispose();
    }

    [Fact]
    public void ShouldRaiseCollectionChangedOnRemove()
    {
        // Arrange
        _fixture.TestDtoStore.Add(new TestDto { Name = "Test" });

        var sut = new CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        var item = sut.Items.Single();
        sut.SelectedItems.Add(item);

        NotifyCollectionChangedAction? capturedAction = null;
        sut.SelectedItems.CollectionChanged += (_, e) =>
        {
            capturedAction = e.Action;
        };

        // Act
        sut.SelectedItems.Remove(item);

        // Assert
        capturedAction.Should().Be(NotifyCollectionChangedAction.Remove);

        // Cleanup
        _fixture.ClearTestData();
        sut.Dispose();
    }

    [Fact]
    public void ShouldRaiseCollectionChangedOnClear()
    {
        // Arrange
        _fixture.TestDtoStore.Add(new TestDto { Name = "Test" });

        var sut = new CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        sut.SelectedItems.Add(sut.Items.Single());

        NotifyCollectionChangedAction? capturedAction = null;
        sut.SelectedItems.CollectionChanged += (_, e) =>
        {
            capturedAction = e.Action;
        };

        // Act
        sut.SelectedItems.Clear();

        // Assert
        capturedAction.Should().Be(NotifyCollectionChangedAction.Reset);

        // Cleanup
        _fixture.ClearTestData();
        sut.Dispose();
    }
}
