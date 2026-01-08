using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.DataStoreAdd;

public sealed class DuplicateAdd_IgnoresSecondAdd : IClassFixture<TestHelperCustomWPFControlsTestFixture>
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;

    public DuplicateAdd_IgnoresSecondAdd(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Test_DuplicateAdd_IgnoresSecondAdd()
    {
        // Arrange
        _fixture.ClearTestData();
        
        var dto = new TestDto { Name = "Test" };

        // Act
        _fixture.Sut.ModelStore.Add(dto);
        _fixture.Sut.ModelStore.Add(dto); // Duplicate

        // Assert
        _fixture.Sut.Count.Should().Be(1);

        // Cleanup
        _fixture.ClearTestData();
    }
}
