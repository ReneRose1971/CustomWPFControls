using System;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.LoadModelsAPI;

/// <summary>
/// Test: LoadModels ersetzt alle vorhandenen Items durch neue Items.
/// </summary>
public sealed class LoadModels_ReplacesAllItems : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;

    public LoadModels_ReplacesAllItems(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
        
        // Setup: Initiale Daten
        _fixture.Sut.ModelStore.AddRange(new[]
        {
            new TestDto { Name = "Old1" },
            new TestDto { Name = "Old2" }
        });
    }

    [Fact]
    public void Test_LoadModels_ReplacesAllItems()
    {
        // Arrange
        var newModels = new[]
        {
            new TestDto { Name = "New1" },
            new TestDto { Name = "New2" },
            new TestDto { Name = "New3" }
        };

        // Act
        _fixture.Sut.LoadModels(newModels);

        // Assert
        _fixture.Sut.Count.Should().Be(3);
        _fixture.Sut.Items.Should().HaveCount(3);
        _fixture.Sut.Items[0].Name.Should().Be("New1");
        _fixture.Sut.Items[1].Name.Should().Be("New2");
        _fixture.Sut.Items[2].Name.Should().Be("New3");
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
