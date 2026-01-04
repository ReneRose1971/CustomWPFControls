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
        _fixture.ClearTestData();
        _fixture.Sut.ModelStore.Add(new TestDto { Name = "Test" });

        // Act
        _fixture.Sut.SelectedItem = _fixture.Sut.Items.First();

        // Assert
        _fixture.Sut.SelectedItem.Should().NotBeNull();

        // Cleanup
        _fixture.ClearTestData();
    }
}
