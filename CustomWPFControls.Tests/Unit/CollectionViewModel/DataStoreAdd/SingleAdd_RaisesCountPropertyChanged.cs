using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.DataStoreAdd;

public sealed class SingleAdd_RaisesCountPropertyChanged : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;

    public SingleAdd_RaisesCountPropertyChanged(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Test_SingleAdd_RaisesCountPropertyChanged()
    {
        // Arrange
        _fixture.ClearTestData();

        bool propertyChangedRaised = false;
        _fixture.Sut.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(_fixture.Sut.Count))
                propertyChangedRaised = true;
        };

        // Act
        _fixture.Sut.ModelStore.Add(new TestDto { Name = "Test" });

        // Assert
        propertyChangedRaised.Should().BeTrue();

        // Cleanup
        _fixture.ClearTestData();
    }
}
