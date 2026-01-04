using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Properties;

public sealed class SelectedItems_AddNull_HandledCorrectly : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;

    public SelectedItems_AddNull_HandledCorrectly(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Test_SelectedItems_AddNull_HandledCorrectly()
    {
        // Arrange
        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        // Act & Assert
        var act = () => sut.SelectedItems.Add(null!);
        act.Should().Throw<ArgumentNullException>();

        // Cleanup
        _fixture.ClearTestData();
        sut.Dispose();
    }
}
