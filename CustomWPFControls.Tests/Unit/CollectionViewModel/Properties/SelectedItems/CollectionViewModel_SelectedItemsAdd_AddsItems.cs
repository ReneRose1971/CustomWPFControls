using System;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Properties.SelectedItems;

/// <summary>
/// Szenario: Mehrere Items wurden zu SelectedItems hinzugefügt.
/// </summary>
/// <remarks>
/// Testet die Add-Funktionalität der SelectedItems-Collection.
/// Shared Setup: 3 Models im Store, 2 Items zu SelectedItems hinzugefügt.
/// Alle Tests prüfen verschiedene Aspekte der hinzugefügten Items.
/// </remarks>
public sealed class CollectionViewModel_SelectedItemsAdd_AddsItems : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;

    public CollectionViewModel_SelectedItemsAdd_AddsItems(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();

        // Shared Setup: 3 Items zum Store hinzufügen
        _fixture.Sut.ModelStore.AddRange(new[]
        {
            new TestDto { Name = "First" },
            new TestDto { Name = "Second" },
            new TestDto { Name = "Third" }
        });

        // 2 Items zu SelectedItems hinzufügen
        _fixture.Sut.SelectedItems.Add(_fixture.Sut.Items[0]);
        _fixture.Sut.SelectedItems.Add(_fixture.Sut.Items[2]);
    }

    [Fact]
    public void SelectedItems_ContainsTwoItems()
    {
        // Assert: SelectedItems enthält 2 Items
        _fixture.Sut.SelectedItems.Should().HaveCount(2);
    }

    [Fact]
    public void SelectedItems_ContainsFirstItem()
    {
        // Assert: Erstes hinzugefügtes Item ist enthalten
        _fixture.Sut.SelectedItems.Should().Contain(_fixture.Sut.Items[0]);
    }

    [Fact]
    public void SelectedItems_ContainsThirdItem()
    {
        // Assert: Zweites hinzugefügtes Item ist enthalten
        _fixture.Sut.SelectedItems.Should().Contain(_fixture.Sut.Items[2]);
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
