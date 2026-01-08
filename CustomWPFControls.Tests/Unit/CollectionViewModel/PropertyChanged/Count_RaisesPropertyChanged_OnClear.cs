using System;
using System.ComponentModel;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.PropertyChanged;

public sealed class Count_RaisesPropertyChanged_OnClear : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;
    private int _propertyChangedCount = 0;

    public Count_RaisesPropertyChanged_OnClear(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
        
        // Setup
        _fixture.Sut.ModelStore.AddRange(new[]
        {
            new TestDto { Name = "First" },
            new TestDto { Name = "Second" }
        });
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_fixture.Sut.Count))
            _propertyChangedCount++;
    }

    [Fact]
    public void Test_Count_RaisesPropertyChanged_OnClear()
    {
        // Arrange
        _fixture.Sut.PropertyChanged += OnPropertyChanged;

        // Act
        _fixture.Sut.ModelStore.Clear();

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
