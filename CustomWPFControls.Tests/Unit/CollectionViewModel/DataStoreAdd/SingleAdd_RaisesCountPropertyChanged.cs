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
        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        bool propertyChangedRaised = false;
        sut.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(sut.Count))
                propertyChangedRaised = true;
        };

        // Act
        _fixture.TestDtoStore.Add(new TestDto { Name = "Test" });

        // Assert
        propertyChangedRaised.Should().BeTrue();

        // Cleanup
        _fixture.ClearTestData();
        sut.Dispose();
    }
}
