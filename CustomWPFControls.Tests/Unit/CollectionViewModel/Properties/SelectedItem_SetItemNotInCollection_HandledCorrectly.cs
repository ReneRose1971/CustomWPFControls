using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Properties;

/// <summary>
/// Tests für SelectedItem mit Item, das nicht in der Collection ist.
/// </summary>
public sealed class SelectedItem_SetItemNotInCollection_HandledCorrectly : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;

    public SelectedItem_SetItemNotInCollection_HandledCorrectly(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
    }

    [Fact]
    public void ShouldThrowOrIgnoreItemNotInCollection()
    {
        // Arrange
        _fixture.TestDtoStore.Add(new TestDto { Name = "InCollection" });

        var sut = new CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        // Erstelle ein ViewModel, das NICHT in Items ist
        var notInCollectionDto = new TestDto { Name = "NotInCollection" };
        var notInCollectionViewModel = _fixture.ViewModelFactory.Create(notInCollectionDto);

        // Act & Assert - Entweder Exception oder wird ignoriert
        var act = () => sut.SelectedItem = notInCollectionViewModel;
        
        // Test prüft, dass es keine unerwartete Exception gibt
        act.Should().NotThrow<InvalidOperationException>();

        // Cleanup
        _fixture.ClearTestData();
        sut.Dispose();
    }
}
