using System;
using System.Linq;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.RemoveAPI;

public sealed class RemoveRangeTests : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;

    public RemoveRangeTests(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
        
        // Setup: 5 Items hinzufügen
        _fixture.Sut.ModelStore.AddRange(new[]
        {
            new TestDto { Name = "First" },
            new TestDto { Name = "Second" },
            new TestDto { Name = "Third" },
            new TestDto { Name = "Fourth" },
            new TestDto { Name = "Fifth" }
        });
    }

    [Fact]
    public void RemovesMultipleViewModels()
    {
        // Arrange
        var itemsToRemove = _fixture.Sut.Items.Take(2).ToList();

        // Act
        _fixture.Sut.RemoveRange(itemsToRemove);

        // Assert
        _fixture.Sut.Items.Should().NotContain(itemsToRemove);
    }

    [Fact]
    public void CountIsThree()
    {
        // Arrange
        var itemsToRemove = _fixture.Sut.Items.Take(2).ToList();

        // Act
        _fixture.Sut.RemoveRange(itemsToRemove);

        // Assert
        _fixture.Sut.Count.Should().Be(3);
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
