using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.DataStoreRemove;

public sealed class RemoveSingle_RaisesCountPropertyChanged : IClassFixture<TestHelperCustomWPFControlsTestFixture>
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;

    public RemoveSingle_RaisesCountPropertyChanged(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Test_RemoveSingle_RaisesCountPropertyChanged()
    {
        // Arrange
        _fixture.ClearTestData();
        
        var dto = new TestDto { Name = "Test" };
        _fixture.Sut.ModelStore.Add(dto);

        bool propertyChangedRaised = false;
        _fixture.Sut.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(_fixture.Sut.Count))
                propertyChangedRaised = true;
        };

        // Act
        _fixture.Sut.ModelStore.Remove(dto);

        // Assert
        propertyChangedRaised.Should().BeTrue();

        // Cleanup
        _fixture.ClearTestData();
    }
}
