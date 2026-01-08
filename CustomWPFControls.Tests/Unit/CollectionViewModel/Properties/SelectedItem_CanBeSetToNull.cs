using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Properties;

/// <summary>
/// Tests für SelectedItem Property - Set to Null.
/// </summary>
public sealed class SelectedItem_CanBeSetToNull : IClassFixture<TestHelperCustomWPFControlsTestFixture>
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;

    public SelectedItem_CanBeSetToNull(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Test_SelectedItem_CanBeSetToNull()
    {
        // Arrange
        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        // Act
        sut.SelectedItem = null;

        // Assert
        sut.SelectedItem.Should().BeNull();

        // Cleanup
        _fixture.ClearTestData();
        sut.Dispose();
    }
}
