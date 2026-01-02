using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Lifecycle;

/// <summary>
/// Tests für den Lifecycle (Dispose) von CollectionViewModel.
/// </summary>
/// <remarks>
/// Testet:
/// - Dispose-Verhalten
/// - Mehrfaches Dispose
/// - Collection-Cleanup
/// KEINE Daten-Tests!
/// </remarks>
public sealed class DisposeTests : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;

    public DisposeTests(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
    }

    #region Basic Dispose Tests

    [Fact]
    public void Dispose_CanBeCalledSafely()
    {
        // Arrange
        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        // Act & Assert - Sollte keine Exception werfen
        var act = () => sut.Dispose();
        act.Should().NotThrow();
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        // Arrange
        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        // Act & Assert - Mehrfaches Dispose sollte keine Exception werfen
        sut.Dispose();
        var act = () => sut.Dispose();
        act.Should().NotThrow();
    }

    [Fact]
    public void Dispose_ClearsItemsCollection()
    {
        // Arrange
        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        // Act
        sut.Dispose();

        // Assert
        sut.Items.Should().BeEmpty();
    }

    [Fact]
    public void Dispose_ClearsSelectedItemsCollection()
    {
        // Arrange
        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        // Act
        sut.Dispose();

        // Assert
        sut.SelectedItems.Should().BeEmpty();
    }

    [Fact]
    public void Dispose_ResetsCountToZero()
    {
        // Arrange
        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        // Act
        sut.Dispose();

        // Assert
        sut.Count.Should().Be(0);
    }

    #endregion
}
