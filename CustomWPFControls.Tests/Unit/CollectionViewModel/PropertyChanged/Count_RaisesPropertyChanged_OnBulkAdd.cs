using System;
using System.ComponentModel;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.PropertyChanged;

/// <summary>
/// Test: CollectionViewModel feuert PropertyChanged für Count bei BulkAdd.
/// </summary>
public sealed class Count_RaisesPropertyChanged_OnBulkAdd : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;
    private int _propertyChangedCount = 0;

    public Count_RaisesPropertyChanged_OnBulkAdd(TestHelperCustomWPFControlsTestFixture fixture)
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
    public void Test_Count_RaisesPropertyChanged_OnBulkAdd()
    {
        // Arrange
        _fixture.Sut.PropertyChanged += OnPropertyChanged;

        // Act
        _fixture.Sut.ModelStore.AddRange(new[]
        {
            new TestDto { Name = "First" },
            new TestDto { Name = "Second" }
        });

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
