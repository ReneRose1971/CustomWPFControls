using System;
using System.ComponentModel;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.PropertyChanged;

/// <summary>
/// Test: CollectionViewModel feuert PropertyChanged für Count bei BulkAdd.
/// </summary>
public sealed class Count_RaisesPropertyChanged_OnBulkAdd : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly CollectionViewModel<TestDto, TestViewModel> _sut;
    private bool _countPropertyChanged;

    public Count_RaisesPropertyChanged_OnBulkAdd(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _sut = new CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

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
    public void Count_RaisesPropertyChanged_WhenBulkAddExecuted()
    {
        // Act
        _fixture.TestDtoStore.AddRange(new[]
        {
            new TestDto { Name = "Item1" },
            new TestDto { Name = "Item2" },
            new TestDto { Name = "Item3" }
        });

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
