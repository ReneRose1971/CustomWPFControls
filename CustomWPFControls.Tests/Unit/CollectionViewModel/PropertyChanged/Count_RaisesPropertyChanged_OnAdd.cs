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
    private readonly ViewModels.CollectionViewModel<TestDto, TestViewModel> _sut;
    private int _propertyChangedCount = 0;

    public Count_RaisesPropertyChanged_OnAdd(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_sut.Count))
            _propertyChangedCount++;
    }

    [Fact]
    public void Test_Count_RaisesPropertyChanged_OnAdd()
    {
        // Arrange
        _sut.PropertyChanged += OnPropertyChanged;

        // Act
        _fixture.TestDtoStore.Add(new TestDto { Name = "Test" });

        // Assert
        _propertyChangedCount.Should().BeGreaterThan(0);
    }

    public void Dispose()
    {
        _sut.PropertyChanged -= OnPropertyChanged;
        _fixture.ClearTestData();
        _sut?.Dispose();
    }
}
