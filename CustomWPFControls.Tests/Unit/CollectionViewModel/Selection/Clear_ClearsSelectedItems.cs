using System;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Selection;

/// <summary>
/// Test: Clear() leert SelectedItems Collection.
/// </summary>
public sealed class Clear_ClearsSelectedItems : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;

    public Clear_ClearsSelectedItems(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();

        // Setup: Items hinzufügen
        _fixture.Sut.ModelStore.AddRange(new[]
        {
            new TestDto { Name = "Item1" },
            new TestDto { Name = "Item2" }
        });

        // Setup: Items zu SelectedItems hinzufügen
        _fixture.Sut.SelectedItems.Add(_fixture.Sut.Items[0]);
        _fixture.Sut.SelectedItems.Add(_fixture.Sut.Items[1]);
    }

    [Fact]
    public void Test_Clear_ClearsSelectedItems()
    {
        // Arrange: Verify SelectedItems ist nicht leer
        Assert.Equal(2, _fixture.Sut.SelectedItems.Count);

        // Act
        _fixture.Sut.Clear();

        // Assert
        Assert.Empty(_fixture.Sut.SelectedItems);
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
