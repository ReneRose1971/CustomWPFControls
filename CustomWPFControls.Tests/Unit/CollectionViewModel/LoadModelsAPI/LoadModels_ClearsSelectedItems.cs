using System;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.LoadModelsAPI;

/// <summary>
/// Test: LoadModels leert SelectedItems.
/// </summary>
public sealed class LoadModels_ClearsSelectedItems : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;

    public LoadModels_ClearsSelectedItems(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
        
        // Setup: Initiale Daten mit Multi-Selection
        _fixture.Sut.ModelStore.AddRange(new[]
        {
            new TestDto { Name = "Old1" },
            new TestDto { Name = "Old2" },
            new TestDto { Name = "Old3" }
        });
        
        // Warten auf Synchronisation
        System.Threading.Thread.Sleep(50);
        
        // Mehrere Items selektieren
        _fixture.Sut.SelectedItems.Add(_fixture.Sut.Items[0]);
        _fixture.Sut.SelectedItems.Add(_fixture.Sut.Items[1]);
    }

    [Fact]
    public void Test_LoadModels_ClearsSelectedItems()
    {
        // Arrange
        _fixture.Sut.SelectedItems.Should().HaveCount(2, "Setup should have selected 2 items");
        
        var newModels = new[]
        {
            new TestDto { Name = "New1" },
            new TestDto { Name = "New2" }
        };

        // Act
        _fixture.Sut.LoadModels(newModels);

        // Assert
        _fixture.Sut.SelectedItems.Should().BeEmpty("SelectedItems should be cleared by LoadModels");
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
