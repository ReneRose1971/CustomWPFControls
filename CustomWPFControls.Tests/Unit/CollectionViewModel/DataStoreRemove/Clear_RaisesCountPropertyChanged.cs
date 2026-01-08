using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.DataStoreRemove;

/// <summary>
/// Tests für PropertyChanged Event beim Clear.
/// </summary>
public sealed class Clear_RaisesCountPropertyChanged : IClassFixture<TestHelperCustomWPFControlsTestFixture>
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;

    public Clear_RaisesCountPropertyChanged(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Test_Clear_RaisesCountPropertyChanged()
    {
        // Arrange
        _fixture.ClearTestData();
        
        _fixture.Sut.ModelStore.AddRange(new[]
        {
            new TestDto { Name = "First" },
            new TestDto { Name = "Second" }
        });

        bool propertyChangedRaised = false;
        _fixture.Sut.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(_fixture.Sut.Count))
                propertyChangedRaised = true;
        };

        // Act
        _fixture.Sut.ModelStore.Clear();

        // Assert
        propertyChangedRaised.Should().BeTrue();

        // Cleanup
        _fixture.ClearTestData();
    }
}
