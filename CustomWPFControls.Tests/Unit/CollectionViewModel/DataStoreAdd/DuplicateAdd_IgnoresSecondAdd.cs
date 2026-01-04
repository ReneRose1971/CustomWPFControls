using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.DataStoreAdd;

public sealed class DuplicateAdd_IgnoresSecondAdd : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;

    public DuplicateAdd_IgnoresSecondAdd(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Test_DuplicateAdd_IgnoresSecondAdd()
    {
        // Arrange
        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);
        
        var dto = new TestDto { Name = "Test" };

        // Act
        _fixture.TestDtoStore.Add(dto);
        _fixture.TestDtoStore.Add(dto); // Duplicate

        // Assert
        sut.Count.Should().Be(1);

        // Cleanup
        _fixture.ClearTestData();
        sut.Dispose();
    }
}
