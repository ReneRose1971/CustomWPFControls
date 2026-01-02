using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Properties;

/// <summary>
/// Tests für SelectedItem Property - Set Operation.
/// </summary>
public sealed class SelectedItem_CanBeSet : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;

    public SelectedItem_CanBeSet(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
    }

    [Fact]
    public void ShouldSetSelectedItem()
    {
        // Arrange
        _fixture.TestDtoStore.Add(new TestDto { Name = "Test" });

        var sut = new CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        var viewModel = sut.Items.Single();

        // Act
        sut.SelectedItem = viewModel;

        // Assert
        sut.SelectedItem.Should().BeSameAs(viewModel);

        // Cleanup
        _fixture.ClearTestData();
        sut.Dispose();
    }
}
