using System;
using System.ComponentModel;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.PropertyChanged;

/// <summary>
/// Test: CollectionViewModel feuert PropertyChanged für Count wenn ein Item hinzugefügt wird.
/// </summary>
public sealed class Count_RaisesPropertyChanged_OnAdd : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private int _propertyChangedCount = 0;

    public Count_RaisesPropertyChanged_OnAdd(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_fixture.Sut.Count))
            _propertyChangedCount++;
    }

    [Fact]
    public void Test_Count_RaisesPropertyChanged_OnAdd()
    {
        // Arrange
        _fixture.Sut.PropertyChanged += OnPropertyChanged;

        // Act
        _fixture.Sut.ModelStore.Add(new TestDto { Name = "Test" });

        // Assert
        _propertyChangedCount.Should().BeGreaterThan(0);
        
        // Cleanup
        _fixture.Sut.PropertyChanged -= OnPropertyChanged;
        _fixture.ClearTestData();
    }

    public void Dispose()
    {
        _fixture.Sut.PropertyChanged -= OnPropertyChanged;
    }
}
