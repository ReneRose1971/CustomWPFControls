using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Properties;

/// <summary>
/// Tests für Count Property - Initialer Wert (kein Act, nur Setup).
/// </summary>
/// <remarks>
/// Shared Setup: 3 Items im Store
/// Alle Tests prüfen den initialen Count-Wert nach dem Setup.
/// </remarks>
public sealed class CountProperty_InitialValue : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly ViewModels.CollectionViewModel<TestDto, TestViewModel> _sut;

    public CountProperty_InitialValue(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);
    }

    [Fact]
    public void ReturnsCorrectValue()
    {
        // Assert
        _sut.Count.Should().Be(3);
    }

    [Fact]
    public void MatchesItemsCount()
    {
        // Assert
        _sut.Count.Should().Be(_sut.Items.Count);
    }

    public void Dispose()
    {
        _sut?.Dispose();
        _fixture?.Dispose();
    }
}
