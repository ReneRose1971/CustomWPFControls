using System;
using System.ComponentModel;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.PropertyChanged;

/// <summary>
/// Test: CollectionViewModel feuert PropertyChanged für Count wenn ein Item entfernt wird.
/// </summary>
public sealed class Count_RaisesPropertyChanged_OnRemove : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;
    private bool _countPropertyChanged;

    public Count_RaisesPropertyChanged_OnRemove(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_fixture.Sut.Count))
            _countPropertyChanged = true;
    }

    [Fact]
    public void Test_Count_RaisesPropertyChanged_OnRemove()
    {
        // Arrange
        var dto = new TestDto { Name = "Test" };
        _fixture.Sut.ModelStore.Add(dto);
        
        _fixture.Sut.PropertyChanged += OnPropertyChanged;

        // Act
        _fixture.Sut.ModelStore.Remove(dto);

        // Assert
        Assert.True(_countPropertyChanged, "PropertyChanged für Count wurde nicht gefeuert");
        
        // Cleanup
        _fixture.Sut.PropertyChanged -= OnPropertyChanged;
        _fixture.ClearTestData();
    }

    public void Dispose()
    {
        _fixture.Sut.PropertyChanged -= OnPropertyChanged;
    }
}
