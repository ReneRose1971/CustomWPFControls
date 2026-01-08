using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Properties;

/// <summary>
/// Tests für Count Property - Nach Clear-Operation.
/// </summary>
/// <remarks>
/// Setup: 3 Items
/// Act: Store clearen
/// Assert: Count == 0
/// </remarks>
public sealed class CountProperty_AfterClear : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;

    public CountProperty_AfterClear(TestHelperCustomWPFControlsTestFixture fixture)
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
    public void BecomesZero()
    {
        // Act
        _fixture.Sut.ModelStore.Clear();

        // Assert
        _fixture.Sut.Count.Should().Be(0);
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
