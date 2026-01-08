using System;
using System.Collections.Specialized;
using System.Linq;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Properties.SelectedItems;

/// <summary>
/// Szenario: Ein Item wurde aus SelectedItems entfernt.
/// </summary>
/// <remarks>
/// Testet das CollectionChanged-Event der SelectedItems-Collection bei Remove-Operation.
/// Shared Setup: 1 Model im Store, 1 Item zu SelectedItems hinzugefügt, dann wieder entfernt.
/// Alle Tests prüfen verschiedene Aspekte des CollectionChanged-Events.
/// </remarks>
public sealed class CollectionViewModel_SelectedItemsRemove_RaisesCollectionChanged : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;
    private NotifyCollectionChangedAction? _capturedAction;

    public CollectionViewModel_SelectedItemsRemove_RaisesCollectionChanged(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();

        // Shared Setup: Model zum Store hinzufügen
        _fixture.Sut.ModelStore.Add(new TestDto { Name = "Test" });

        // Item zu SelectedItems hinzufügen
        var item = _fixture.Sut.Items.Single();
        _fixture.Sut.SelectedItems.Add(item);

        // CollectionChanged-Event abonnieren (NACH dem Add, um nur Remove zu erfassen)
        _fixture.Sut.SelectedItems.CollectionChanged += (_, e) =>
        {
            _capturedAction = e.Action;
        };

        // Item aus SelectedItems entfernen
        _fixture.Sut.SelectedItems.Remove(item);
    }

    [Fact]
    public void CollectionChanged_WasRaised()
    {
        // Assert: Event wurde gefeuert
        _capturedAction.Should().NotBeNull();
    }

    [Fact]
    public void CollectionChanged_ActionIsRemove()
    {
        // Assert: Action ist Remove
        _capturedAction.Should().Be(NotifyCollectionChangedAction.Remove);
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
