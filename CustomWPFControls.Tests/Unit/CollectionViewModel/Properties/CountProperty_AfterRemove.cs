using System;
using System.Linq;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Properties;

public sealed class CountProperty_AfterRemove : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;

    public CountProperty_AfterRemove(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
        
        // Setup: 3 Items hinzufügen
        _fixture.Sut.ModelStore.AddRange(new[]
        {
            new TestDto { Name = "First" },
            new TestDto { Name = "Second" },
            new TestDto { Name = "Third" }
        });
    }

    [Fact]
    public void UpdatesCorrectly()
    {
        // Act
        var secondItem = _fixture.Sut.Items.Skip(1).First();
        _fixture.Sut.ModelStore.Remove(secondItem.Model);

        // Assert
        _fixture.Sut.Count.Should().Be(2);
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
