using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.EdgeCases;

public sealed class RemoveNonExisting_ReturnsFalse : IClassFixture<TestHelperCustomWPFControlsTestFixture>
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;

    public RemoveNonExisting_ReturnsFalse(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Test_RemoveNonExisting_ReturnsFalse()
    {
        // Arrange
        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);
        
        var nonExistingDto = new TestDto { Name = "NonExisting" };
        var nonExistingVm = _fixture.ViewModelFactory.Create(nonExistingDto);

        // Act
        var result = sut.Remove(nonExistingVm);

        // Assert
        result.Should().BeFalse();

        // Cleanup
        _fixture.ClearTestData();
        sut.Dispose();
    }
}
