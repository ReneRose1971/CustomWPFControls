using System;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.CustomWPFControls.Extensions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.Extensions;

/// <summary>
/// Tests für LoadModelsAndWait Extension.
/// </summary>
public sealed class LoadModelsAndWait_SynchronizesCorrectly : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;

    public LoadModelsAndWait_SynchronizesCorrectly(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
    }

    [Fact]
    public void Test_LoadModelsAndWait_SynchronizesCorrectly()
    {
        // Arrange
        var models = new[]
        {
            new TestDto { Name = "Model1" },
            new TestDto { Name = "Model2" },
            new TestDto { Name = "Model3" }
        };

        // Act
        _fixture.Sut.LoadModelsAndWait(models);

        // Assert - Garantiert synchronisiert
        _fixture.Sut.Items.Count.Should().Be(3);
        _fixture.Sut.Items[0].Name.Should().Be("Model1");
        _fixture.Sut.Items[1].Name.Should().Be("Model2");
        _fixture.Sut.Items[2].Name.Should().Be("Model3");
    }

    [Fact]
    public void Test_LoadModelsAndWait_WithEmptyList_ClearsCollection()
    {
        // Arrange
        _fixture.Sut.ModelStore.Add(new TestDto { Name = "Existing" });

        // Act
        _fixture.Sut.LoadModelsAndWait(Array.Empty<TestDto>());

        // Assert
        _fixture.Sut.Items.Should().BeEmpty();
    }

    [Fact]
    public void Test_LoadModelsAndWait_WithNull_TreatsAsEmptyList()
    {
        // Arrange
        _fixture.Sut.ModelStore.Add(new TestDto { Name = "Existing" });

        // Act
        _fixture.Sut.LoadModelsAndWait(null);

        // Assert
        _fixture.Sut.Items.Should().BeEmpty();
    }

    [Fact]
    public void Test_LoadModelsAndWait_InvalidatesSelection()
    {
        // Arrange
        _fixture.Sut.ModelStore.Add(new TestDto { Name = "Old" });
        System.Threading.Thread.Sleep(50); // Warten auf Sync
        _fixture.Sut.SelectedItem = _fixture.Sut.Items[0];

        var newModels = new[]
        {
            new TestDto { Name = "New1" },
            new TestDto { Name = "New2" }
        };

        // Act
        _fixture.Sut.LoadModelsAndWait(newModels);

        // Assert
        _fixture.Sut.SelectedItem.Should().BeNull("Selection should be invalidated");
        _fixture.Sut.SelectedItems.Should().BeEmpty();
    }

    [Fact]
    public void Test_LoadModelsAndWait_WithNullViewModel_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => CollectionViewModelExtensions.LoadModelsAndWait<TestDto, TestViewModel>(
            null!, new[] { new TestDto { Name = "Test" } });

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("viewModel");
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
