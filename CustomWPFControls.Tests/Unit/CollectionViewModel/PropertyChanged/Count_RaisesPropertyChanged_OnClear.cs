using System;
using System.ComponentModel;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.PropertyChanged;

public sealed class Count_RaisesPropertyChanged_OnClear : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly ViewModels.CollectionViewModel<TestDto, TestViewModel> _sut;
    private int _propertyChangedCount = 0;

    public Count_RaisesPropertyChanged_OnClear(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);
        
        // Setup
        _fixture.TestDtoStore.AddRange(new[]
        {
            new TestDto { Name = "First" },
            new TestDto { Name = "Second" }
        });
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_sut.Count))
            _propertyChangedCount++;
    }

    [Fact]
    public void Test_Count_RaisesPropertyChanged_OnClear()
    {
        // Arrange
        _sut.PropertyChanged += OnPropertyChanged;

        // Act
        _fixture.TestDtoStore.Clear();

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
