using System;
using System.ComponentModel;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Dispose;

/// <summary>
/// Test: Dispose() unsubscribed von Store-Events (keine PropertyChanged-Events mehr).
/// </summary>
public sealed class Dispose_UnsubscribesFromStoreEvents : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;
    private readonly ViewModels.CollectionViewModel<TestDto, TestViewModel> _sut;
    private int _propertyChangedCount = 0;

    public Dispose_UnsubscribesFromStoreEvents(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        _sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        _propertyChangedCount++;
    }

    [Fact]
    public void Test_Dispose_UnsubscribesFromStoreEvents()
    {
        // Arrange
        _sut.PropertyChanged += OnPropertyChanged;

        // Act - Dispose und dann Store ändern
        _sut.Dispose();
        _fixture.TestDtoStore.Add(new TestDto { Name = "AfterDispose" });

        // Assert - Event sollte nicht gefeuert werden
        _propertyChangedCount.Should().Be(0);

        // Cleanup
        _fixture.ClearTestData();
    }

    public void Dispose()
    {
        _sut.PropertyChanged -= OnPropertyChanged;
        _fixture.ClearTestData();
        _sut?.Dispose();
    }
}
