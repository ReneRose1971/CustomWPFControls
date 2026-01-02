using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Properties;

/// <summary>
/// Tests für SelectedItem Property - Set to Null.
/// </summary>
public sealed class SelectedItem_CanBeSetToNull : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;

    public SelectedItem_CanBeSetToNull(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
    }

    [Fact]
    public void ShouldSetToNull()
    {
        // Arrange
        _fixture.TestDtoStore.Add(new TestDto { Name = "Test" });

        var sut = new CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        sut.SelectedItem = sut.Items.Single();

        // Act
        sut.SelectedItem = null;

        // Assert
        sut.SelectedItem.Should().BeNull();

        // Cleanup
        _fixture.ClearTestData();
        sut.Dispose();
    }
}
