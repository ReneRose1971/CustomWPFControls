using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Constructor;

/// <summary>
/// Tests für den Constructor von CollectionViewModel.
/// </summary>
/// <remarks>
/// Testet:
/// - Gültige Abhängigkeiten
/// - Null-Validierung
/// - Initialisierung der Properties
/// KEINE Daten-Tests!
/// </remarks>
public sealed class ConstructorTests : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;

    public ConstructorTests(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
    }

    #region Valid Dependencies Tests

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        // Act
        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        // Assert
        sut.Should().NotBeNull();
        
        // Cleanup
        sut.Dispose();
    }

    [Fact]
    public void Constructor_InitializesItemsProperty()
    {
        // Act
        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        // Assert
        sut.Items.Should().NotBeNull();
        sut.Items.Should().BeEmpty();
        
        // Cleanup
        sut.Dispose();
    }

    [Fact]
    public void Constructor_InitializesSelectedItemsProperty()
    {
        // Act
        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        // Assert
        sut.SelectedItems.Should().NotBeNull();
        sut.SelectedItems.Should().BeEmpty();
        
        // Cleanup
        sut.Dispose();
    }

    [Fact]
    public void Constructor_InitializesCountToZero()
    {
        // Act
        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        // Assert
        sut.Count.Should().Be(0);
        
        // Cleanup
        sut.Dispose();
    }

    [Fact]
    public void Constructor_SelectedItemIsNull()
    {
        // Act
        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        // Assert
        sut.SelectedItem.Should().BeNull();
        
        // Cleanup
        sut.Dispose();
    }

    #endregion

    #region Null Validation Tests

    [Fact]
    public void Constructor_WithNullDataStores_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            null!,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("dataStores");
    }

    [Fact]
    public void Constructor_WithNullViewModelFactory_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            null!,
            _fixture.ComparerService);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("viewModelFactory");
    }

    [Fact]
    public void Constructor_WithNullComparerService_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("comparerService");
    }

    #endregion
}
