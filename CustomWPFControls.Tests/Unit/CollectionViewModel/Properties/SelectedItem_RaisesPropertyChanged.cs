using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Properties;

public sealed class SelectedItem_RaisesPropertyChanged : IClassFixture<TestHelperCustomWPFControlsTestFixture>
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;

    public SelectedItem_RaisesPropertyChanged(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Test_SelectedItem_RaisesPropertyChanged()
    {
        // Arrange
        _fixture.ClearTestData();
        _fixture.Sut.ModelStore.Add(new TestDto { Name = "Test" });

        bool propertyChangedRaised = false;
        _fixture.Sut.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(_fixture.Sut.SelectedItem))
                propertyChangedRaised = true;
        };

        // Act
        _fixture.Sut.SelectedItem = _fixture.Sut.Items.First();

        // Assert
        propertyChangedRaised.Should().BeTrue();

        // Cleanup
        _fixture.ClearTestData();
    }
}
