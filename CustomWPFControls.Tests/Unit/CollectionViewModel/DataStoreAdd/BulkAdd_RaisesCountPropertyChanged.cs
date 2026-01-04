using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.DataStoreAdd;

/// <summary>
/// Tests für PropertyChanged Event beim BulkAdd.
/// </summary>
public sealed class BulkAdd_RaisesCountPropertyChanged : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;

    public BulkAdd_RaisesCountPropertyChanged(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Test_BulkAdd_RaisesCountPropertyChanged()
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
        _fixture.Sut.ModelStore.AddRange(new[]
        {
            new TestDto { Name = "First" },
            new TestDto { Name = "Second" }
        });

        // Assert
        propertyChangedRaised.Should().BeTrue();

        // Cleanup
        _fixture.ClearTestData();
    }
}
