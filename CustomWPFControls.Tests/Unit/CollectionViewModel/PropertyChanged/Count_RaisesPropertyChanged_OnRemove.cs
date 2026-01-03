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
    private readonly CollectionViewModel<TestDto, TestViewModel> _sut;
    private bool _countPropertyChanged;

    public Count_RaisesPropertyChanged_OnRemove(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _sut = new CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        // Setup: Item hinzufügen
        _fixture.TestDtoStore.Add(new TestDto { Name = "Test" });

        _sut.PropertyChanged += OnPropertyChanged;
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_sut.Count))
        {
            _countPropertyChanged = true;
        }
    }

    [Fact]
    public void Count_RaisesPropertyChanged_WhenItemRemoved()
    {
        // Arrange
        var item = _sut.Items[0];

        // Act
        _sut.Remove(item);

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
