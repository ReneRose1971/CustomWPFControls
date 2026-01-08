using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.EdgeCases;

public sealed class RemoveNonExisting_DoesNotThrow : IClassFixture<TestHelperCustomWPFControlsTestFixture>
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;

    public RemoveNonExisting_DoesNotThrow(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Test_RemoveNonExisting_DoesNotThrow()
    {
        // Arrange
        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);
        
        var nonExistingDto = new TestDto { Name = "NonExisting" };
        var nonExistingVm = _fixture.ViewModelFactory.Create(nonExistingDto);

        // Act & Assert
        var act = () => sut.Remove(nonExistingVm);
        act.Should().NotThrow();

        // Cleanup
        _fixture.ClearTestData();
        sut.Dispose();
    }
}
