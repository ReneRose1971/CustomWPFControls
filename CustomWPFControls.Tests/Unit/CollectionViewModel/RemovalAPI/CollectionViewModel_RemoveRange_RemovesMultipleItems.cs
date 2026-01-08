using System;
using System.Linq;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.RemovalAPI;

/// <summary>
/// Szenario: Mehrere ViewModels wurden per RemoveRange()-Methode entfernt.
/// </summary>
/// <remarks>
/// Testet die RemoveRange()-API-Methode des CollectionViewModel.
/// Shared Setup: 3 Models hinzugefügt, dann 2 per RemoveRange() entfernt.
/// Alle Tests prüfen, dass die korrekten ViewModels und Models entfernt wurden.
/// </remarks>
public sealed class CollectionViewModel_RemoveRange_RemovesMultipleItems : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;
    private readonly int _removedCount;

    public CollectionViewModel_RemoveRange_RemovesMultipleItems(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();

        // Shared Setup: 3 Models hinzufügen
        var models = new[]
        {
            new TestDto { Name = "Model1" },
            new TestDto { Name = "Model2" },
            new TestDto { Name = "Model3" }
        };
        _fixture.Sut.ModelStore.AddRange(models);

        // 2 ViewModels per RemoveRange() entfernen
        var viewModelsToRemove = _fixture.Sut.Items.Take(2).ToList();
        _removedCount = _fixture.Sut.RemoveRange(viewModelsToRemove);
    }

    [Fact]
    public void RemoveRange_ReturnsTwoRemoved()
    {
        // Assert: RemoveRange() gibt Anzahl entfernter Items zurück
        _removedCount.Should().Be(2);
    }

    [Fact]
    public void Count_IsOne()
    {
        // Assert: Noch 1 ViewModel übrig
        _fixture.Sut.Count.Should().Be(1);
    }

    [Fact]
    public void Items_ContainsSingleViewModel()
    {
        // Assert: Items enthält noch 1 ViewModel
        _fixture.Sut.Items.Should().ContainSingle();
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
