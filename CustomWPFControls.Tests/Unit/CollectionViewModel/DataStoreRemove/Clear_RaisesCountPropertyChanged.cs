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
public sealed class Clear_RaisesCountPropertyChanged : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;

    public Clear_RaisesCountPropertyChanged(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Test_Clear_RaisesCountPropertyChanged()
    {
        // Arrange
        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);
        
        _fixture.TestDtoStore.AddRange(new[]
        {
            new TestDto { Name = "First" },
            new TestDto { Name = "Second" }
        });

        bool propertyChangedRaised = false;
        sut.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(sut.Count))
                propertyChangedRaised = true;
        };

        // Act
        _fixture.TestDtoStore.Clear();

        // Assert
        propertyChangedRaised.Should().BeTrue();

        // Cleanup
        _fixture.ClearTestData();
        sut.Dispose();
    }
}
