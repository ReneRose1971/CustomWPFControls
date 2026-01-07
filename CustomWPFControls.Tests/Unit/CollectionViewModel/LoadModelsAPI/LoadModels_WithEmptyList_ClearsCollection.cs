using System;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.LoadModelsAPI;

/// <summary>
/// Test: LoadModels mit leerer Liste leert die Collection.
/// </summary>
public sealed class LoadModels_WithEmptyList_ClearsCollection : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;

    public LoadModels_WithEmptyList_ClearsCollection(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
        
        // Setup: Initiale Daten
        _fixture.Sut.ModelStore.AddRange(new[]
        {
            new TestDto { Name = "Item1" },
            new TestDto { Name = "Item2" }
        });
    }

    [Fact]
    public void Test_LoadModels_WithEmptyList_ClearsCollection()
    {
        // Arrange
        var emptyModels = Array.Empty<TestDto>();

        // Act
        _fixture.Sut.LoadModels(emptyModels);

        // Assert
        _fixture.Sut.Count.Should().Be(0);
        _fixture.Sut.Items.Should().BeEmpty();
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
