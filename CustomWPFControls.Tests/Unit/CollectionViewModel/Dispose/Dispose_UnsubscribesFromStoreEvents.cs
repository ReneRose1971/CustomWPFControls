using System;
using System.ComponentModel;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Dispose;

/// <summary>
/// Test: Dispose() unsubscribed von Store-Events (keine PropertyChanged-Events mehr).
/// </summary>
public sealed class Dispose_UnsubscribesFromStoreEvents : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly CollectionViewModel<TestDto, TestViewModel> _sut;
    private bool _propertyChangedAfterDispose;

    public Dispose_UnsubscribesFromStoreEvents(CollectionViewModelFixture fixture)
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
        _propertyChangedAfterDispose = true;
    }

    [Fact]
    public void Dispose_UnsubscribesFromStoreEvents_NoMorePropertyChangedEvents()
    {
        // Arrange
        _sut.Dispose();
        _propertyChangedAfterDispose = false;

        // Act: Versuche Store-Änderung
        _fixture.TestDtoStore.Add(new TestDto { Name = "Test" });

        // Assert: Kein PropertyChanged sollte gefeuert werden
        Assert.False(_propertyChangedAfterDispose, "PropertyChanged wurde nach Dispose gefeuert");
    }

    public void Dispose()
    {
        _sut.PropertyChanged -= OnPropertyChanged;
        _fixture.ClearTestData();
    }
}
