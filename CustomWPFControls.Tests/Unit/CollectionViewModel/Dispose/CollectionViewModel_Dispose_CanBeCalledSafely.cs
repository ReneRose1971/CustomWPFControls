using System;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Dispose;

/// <summary>
/// Szenario: CollectionViewModel wurde einmalig disposed.
/// </summary>
/// <remarks>
/// Testet, dass einmaliges Dispose ohne Exception ausgeführt werden kann.
/// Shared Setup: Neue Instanz erstellt und disposed.
/// </remarks>
public sealed class CollectionViewModel_Dispose_CanBeCalledSafely : IClassFixture<TestHelperCustomWPFControlsTestFixture>
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;
    private readonly Exception? _exception;

    public CollectionViewModel_Dispose_CanBeCalledSafely(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        
        // Shared Setup: Neue Instanz erstellen und disposen
        var sut = _fixture.CreateCollectionViewModel();
        
        try
        {
            sut.Dispose();
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }

    [Fact]
    public void Dispose_DoesNotThrow()
    {
        // Assert: Keine Exception wurde geworfen
        _exception.Should().BeNull();
    }
}
