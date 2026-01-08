using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Properties;

/// <summary>
/// Tests für Count Property - Nach Add-Operation.
/// </summary>
/// <remarks>
/// Setup: 3 Items
/// Act: 1 Item hinzufügen
/// Assert: Count == 4
/// </remarks>
public sealed class CountProperty_AfterAdd : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;

    public CountProperty_AfterAdd(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
        
        // Setup: 3 Items zum Store hinzufügen
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
        _fixture.Sut.ModelStore.Add(new TestDto { Name = "Fourth" });

        // Assert
        _fixture.Sut.Count.Should().Be(4);
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
