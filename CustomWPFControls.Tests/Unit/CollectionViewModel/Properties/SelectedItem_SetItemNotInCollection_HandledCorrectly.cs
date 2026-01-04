using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Properties;

public sealed class SelectedItem_SetItemNotInCollection_HandledCorrectly : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;

    public SelectedItem_SetItemNotInCollection_HandledCorrectly(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Test_SelectedItem_SetItemNotInCollection_HandledCorrectly()
    {
        // Arrange
        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);
        
        var itemNotInCollection = _fixture.ViewModelFactory.Create(new TestDto { Name = "NotInCollection" });

        // Act
        sut.SelectedItem = itemNotInCollection;

        // Assert
        sut.SelectedItem.Should().Be(itemNotInCollection);

        // Cleanup
        _fixture.ClearTestData();
        sut.Dispose();
    }
}
