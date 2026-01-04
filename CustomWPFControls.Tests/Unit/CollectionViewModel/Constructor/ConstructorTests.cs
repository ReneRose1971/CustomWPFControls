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
            _fixture.Services,
            _fixture.ViewModelFactory);

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
            _fixture.Services,
            _fixture.ViewModelFactory);

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
            _fixture.Services,
            _fixture.ViewModelFactory);

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
            _fixture.Services,
            _fixture.ViewModelFactory);

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
            _fixture.Services,
            _fixture.ViewModelFactory);

        // Assert
        sut.SelectedItem.Should().BeNull();
        
        // Cleanup
        sut.Dispose();
    }

    #endregion

    #region Null Validation Tests

    [Fact]
    public void Constructor_WithNullServices_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            null!,
            _fixture.ViewModelFactory);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("services");
    }

    [Fact]
    public void Constructor_WithNullViewModelFactory_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("viewModelFactory");
    }

    #endregion
}
