using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Dispose;

/// <summary>
/// Test: Dispose() kann mehrfach aufgerufen werden ohne Fehler.
/// </summary>
public sealed class Dispose_CanBeCalledMultipleTimes : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly CollectionViewModel<TestDto, TestViewModel> _sut;

    public Dispose_CanBeCalledMultipleTimes(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _sut = new CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes_WithoutException()
    {
        // Act & Assert (sollte nicht werfen)
        _sut.Dispose();
        _sut.Dispose();
        _sut.Dispose();
    }
}
