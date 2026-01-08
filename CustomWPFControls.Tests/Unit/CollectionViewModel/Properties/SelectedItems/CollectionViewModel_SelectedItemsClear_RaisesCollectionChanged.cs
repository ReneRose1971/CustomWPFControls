using System;
using System.Collections.Specialized;
using System.Linq;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Properties.SelectedItems;

/// <summary>
/// Szenario: SelectedItems wurde geleert.
/// </summary>
/// <remarks>
/// Testet das CollectionChanged-Event der SelectedItems-Collection bei Clear-Operation.
/// Shared Setup: 1 Model im Store, 1 Item zu SelectedItems hinzugefügt, dann SelectedItems geleert.
/// Alle Tests prüfen verschiedene Aspekte des CollectionChanged-Events.
/// </remarks>
public sealed class CollectionViewModel_SelectedItemsClear_RaisesCollectionChanged : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;
    private NotifyCollectionChangedAction? _capturedAction;

    public CollectionViewModel_SelectedItemsClear_RaisesCollectionChanged(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();

        // Shared Setup: Model zum Store hinzufügen
        _fixture.Sut.ModelStore.Add(new TestDto { Name = "Test" });

        // Item zu SelectedItems hinzufügen
        _fixture.Sut.SelectedItems.Add(_fixture.Sut.Items.Single());

        // CollectionChanged-Event abonnieren (NACH dem Add, um nur Clear zu erfassen)
        _fixture.Sut.SelectedItems.CollectionChanged += (_, e) =>
        {
            _capturedAction = e.Action;
        };

        // SelectedItems leeren
        _fixture.Sut.SelectedItems.Clear();
    }

    [Fact]
    public void CollectionChanged_WasRaised()
    {
        // Assert: Event wurde gefeuert
        _capturedAction.Should().NotBeNull();
    }

    [Fact]
    public void CollectionChanged_ActionIsReset()
    {
        // Assert: Action ist Reset (Clear führt zu Reset)
        _capturedAction.Should().Be(NotifyCollectionChangedAction.Reset);
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
