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
public sealed class Count_RaisesPropertyChanged_OnRemove : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly ViewModels.CollectionViewModel<TestDto, TestViewModel> _sut;
    private bool _countPropertyChanged;

    public Count_RaisesPropertyChanged_OnRemove(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_sut.Count))
            _countPropertyChanged = true;
    }

    [Fact]
    public void Test_Count_RaisesPropertyChanged_OnRemove()
    {
        // Arrange
        var dto = new TestDto { Name = "Test" };
        _fixture.TestDtoStore.Add(dto);
        
        _sut.PropertyChanged += OnPropertyChanged;

        // Act
        _fixture.TestDtoStore.Remove(dto);

        // Assert
        Assert.True(_countPropertyChanged, "PropertyChanged für Count wurde nicht gefeuert");
    }

    public void Dispose()
    {
        _sut.PropertyChanged -= OnPropertyChanged;
        _fixture.ClearTestData();
        _sut?.Dispose();
    }
}
