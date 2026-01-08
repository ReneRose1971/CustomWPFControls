using System;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.LoadModelsAPI;

/// <summary>
/// Test: LoadModels invalidiert SelectedItem.
/// </summary>
public sealed class LoadModels_InvalidatesSelectedItem : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;

    public LoadModels_InvalidatesSelectedItem(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
        
        // Setup: Initiale Daten mit Selection
        _fixture.Sut.ModelStore.AddRange(new[]
        {
            new TestDto { Name = "Old1" },
            new TestDto { Name = "Old2" }
        });
        
        // Warten auf Synchronisation
        System.Threading.Thread.Sleep(50);
        
        // Item selektieren
        _fixture.Sut.SelectedItem = _fixture.Sut.Items[0];
    }

    [Fact]
    public void Test_LoadModels_InvalidatesSelectedItem()
    {
        // Arrange
        _fixture.Sut.SelectedItem.Should().NotBeNull("Setup should have selected an item");
        
        var newModels = new[]
        {
            new TestDto { Name = "New1" },
            new TestDto { Name = "New2" }
        };

        // Act
        _fixture.Sut.LoadModels(newModels);

        // Assert
        _fixture.Sut.SelectedItem.Should().BeNull("SelectedItem should be cleared by LoadModels");
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
