using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Properties;

public sealed class SelectedItem_CanBeSet : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;

    public SelectedItem_CanBeSet(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Test_SelectedItem_CanBeSet()
    {
        // Arrange
        _fixture.TestDtoStore.Add(new TestDto { Name = "Test" });

        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        // Act
        sut.SelectedItem = sut.Items.First();

        // Assert
        sut.SelectedItem.Should().NotBeNull();

        // Cleanup
        _fixture.ClearTestData();
        sut.Dispose();
    }
}
