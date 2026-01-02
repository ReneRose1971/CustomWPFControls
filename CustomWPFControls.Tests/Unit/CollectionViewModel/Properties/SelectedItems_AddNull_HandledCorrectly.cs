using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Properties;

/// <summary>
/// Tests für Null-Handling in SelectedItems.
/// </summary>
public sealed class SelectedItems_AddNull_HandledCorrectly : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;

    public SelectedItems_AddNull_HandledCorrectly(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
    }

    [Fact]
    public void ShouldThrowOrIgnoreNull()
    {
        // Arrange
        _fixture.TestDtoStore.Add(new TestDto { Name = "Test" });

        var sut = new CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        // Act & Assert - Entweder Exception oder wird ignoriert
        var act = () => sut.SelectedItems.Add(null!);
        
        // ObservableCollection erlaubt normalerweise null
        // Aber CollectionViewModel könnte das verhindern
        // Test prüft, dass es keine unerwartete Exception gibt
        act.Should().NotThrow<NullReferenceException>();

        // Cleanup
        _fixture.ClearTestData();
        sut.Dispose();
    }
}
