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

    public CountProperty_InitialValue(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
        
        // Shared Setup: 3 Items zum Store hinzufügen
        _fixture.Sut.ModelStore.AddRange(new[]
        {
            new TestDto { Name = "First" },
            new TestDto { Name = "Second" },
            new TestDto { Name = "Third" }
        });
    }

    [Fact]
    public void ReturnsCorrectValue()
    {
        // Assert
        _fixture.Sut.Count.Should().Be(3);
    }

    [Fact]
    public void MatchesItemsCount()
    {
        // Assert
        _fixture.Sut.Count.Should().Be(_fixture.Sut.Items.Count);
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
