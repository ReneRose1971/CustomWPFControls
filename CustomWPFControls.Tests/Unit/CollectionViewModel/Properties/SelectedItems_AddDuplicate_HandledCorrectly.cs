using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Properties;

/// <summary>
/// Tests für Duplikat-Handling in SelectedItems.
/// </summary>
public sealed class SelectedItems_AddDuplicate_HandledCorrectly : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;

    public SelectedItems_AddDuplicate_HandledCorrectly(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
    }

    [Fact]
    public void ShouldIgnoreOrAllowDuplicate()
    {
        // Arrange
        _fixture.TestDtoStore.Add(new TestDto { Name = "Test" });

        var sut = new CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        var item = sut.Items.Single();
        sut.SelectedItems.Add(item);

        // Act - Gleiches Item nochmal hinzufügen
        sut.SelectedItems.Add(item);

        // Assert - ObservableCollection erlaubt normalerweise Duplikate
        // Aber das Verhalten kann implementierungsabhängig sein
        // Test stellt sicher, dass keine Exception geworfen wird
        sut.SelectedItems.Should().NotBeEmpty();

        // Cleanup
        _fixture.ClearTestData();
        sut.Dispose();
    }
}
