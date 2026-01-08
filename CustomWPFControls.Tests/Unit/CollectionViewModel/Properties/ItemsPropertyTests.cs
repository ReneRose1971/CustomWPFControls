using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using System.Collections.ObjectModel;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Properties;

/// <summary>
/// Tests für die Items Property von CollectionViewModel.
/// </summary>
/// <remarks>
/// Testet:
/// - Items Collection Type
/// - Struktur und Initialisierung
/// KEINE Daten-Tests!
/// </remarks>
public sealed class ItemsPropertyTests : IClassFixture<TestHelperCustomWPFControlsTestFixture>
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;

    public ItemsPropertyTests(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
    }

    #region Items Collection Type Tests

    [Fact]
    public void Items_IsReadOnlyObservableCollection()
    {
        // Arrange
        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        // Assert
        sut.Items.Should().BeOfType<ReadOnlyObservableCollection<TestViewModel>>();
        
        // Cleanup
        sut.Dispose();
    }

    [Fact]
    public void Items_IsNotNull()
    {
        // Arrange
        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        // Assert
        sut.Items.Should().NotBeNull();
        
        // Cleanup
        sut.Dispose();
    }

    [Fact]
    public void Items_StartsEmpty()
    {
        // Arrange
        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        // Assert
        sut.Items.Should().BeEmpty();
        sut.Items.Count.Should().Be(0);
        
        // Cleanup
        sut.Dispose();
    }

    #endregion
}
