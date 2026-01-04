using System;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Dispose;

/// <summary>
/// Szenario: CollectionViewModel wurde mehrfach disposed.
/// </summary>
/// <remarks>
/// Testet, dass mehrfaches Dispose ohne Exception ausgeführt werden kann.
/// Shared Setup: Neue Instanz erstellt und dreimal disposed.
/// </remarks>
public sealed class CollectionViewModel_DisposeMultiple_DoesNotThrow : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly Exception? _exception;

    public CollectionViewModel_DisposeMultiple_DoesNotThrow(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        
        // Shared Setup: Neue Instanz erstellen
        var sut = _fixture.CreateCollectionViewModel();
        
        // Mehrfach disposen
        try
        {
            sut.Dispose();
            sut.Dispose();
            sut.Dispose();
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }

    [Fact]
    public void MultipleDispose_DoesNotThrow()
    {
        // Assert: Keine Exception wurde geworfen
        _exception.Should().BeNull();
    }
}
