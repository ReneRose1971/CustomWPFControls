using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Properties;

public sealed class SelectedItem_DoesNotRaisePropertyChanged_WhenSetToSameValue : IClassFixture<TestHelperCustomWPFControlsTestFixture>
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;

    public SelectedItem_DoesNotRaisePropertyChanged_WhenSetToSameValue(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Test_SelectedItem_DoesNotRaisePropertyChanged_WhenSetToSameValue()
    {
        // Arrange
        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        int propertyChangedCount = 0;
        sut.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(sut.SelectedItem))
                propertyChangedCount++;
        };

        // Act
        sut.SelectedItem = null;
        sut.SelectedItem = null; // Same value

        // Assert
        propertyChangedCount.Should().Be(0);

        // Cleanup
        _fixture.ClearTestData();
        sut.Dispose();
    }
}
