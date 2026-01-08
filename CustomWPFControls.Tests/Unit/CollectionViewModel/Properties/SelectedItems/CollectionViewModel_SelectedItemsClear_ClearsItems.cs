using System;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Properties.SelectedItems;

/// <summary>
/// Szenario: SelectedItems wurde geleert.
/// </summary>
/// <remarks>
/// Testet die Clear-Funktionalität der SelectedItems-Collection.
/// Shared Setup: 3 Models im Store, 2 Items zu SelectedItems hinzugefügt, dann geleert.
/// Alle Tests prüfen verschiedene Aspekte nach dem Leeren.
/// </remarks>
public sealed class CollectionViewModel_SelectedItemsClear_ClearsItems : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;

    public CollectionViewModel_SelectedItemsClear_ClearsItems(TestHelperCustomWPFControlsTestFixture fixture)
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
        _fixture.Sut.SelectedItems.Add(_fixture.Sut.Items[1]);

        // SelectedItems leeren
        _fixture.Sut.SelectedItems.Clear();
    }

    [Fact]
    public void SelectedItems_IsEmpty()
    {
        // Assert: SelectedItems ist leer
        _fixture.Sut.SelectedItems.Should().BeEmpty();
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
