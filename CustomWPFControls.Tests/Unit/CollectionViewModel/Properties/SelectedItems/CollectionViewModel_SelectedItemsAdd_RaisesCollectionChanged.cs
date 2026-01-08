using System;
using System.Collections.Specialized;
using System.Linq;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Properties.SelectedItems;

/// <summary>
/// Szenario: Ein Item wurde zu SelectedItems hinzugefügt.
/// </summary>
/// <remarks>
/// Testet das CollectionChanged-Event der SelectedItems-Collection bei Add-Operation.
/// Shared Setup: 1 Model im Store, 1 Item zu SelectedItems hinzugefügt.
/// Alle Tests prüfen verschiedene Aspekte des CollectionChanged-Events.
/// </remarks>
public sealed class CollectionViewModel_SelectedItemsAdd_RaisesCollectionChanged : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;
    private NotifyCollectionChangedAction? _capturedAction;

    public CollectionViewModel_SelectedItemsAdd_RaisesCollectionChanged(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();

        // Shared Setup: Model zum Store hinzufügen
        _fixture.Sut.ModelStore.Add(new TestDto { Name = "Test" });

        // CollectionChanged-Event abonnieren
        _fixture.Sut.SelectedItems.CollectionChanged += (_, e) =>
        {
            _capturedAction = e.Action;
        };

        // Item zu SelectedItems hinzufügen
        _fixture.Sut.SelectedItems.Add(_fixture.Sut.Items.Single());
    }

    [Fact]
    public void CollectionChanged_WasRaised()
    {
        // Assert: Event wurde gefeuert
        _capturedAction.Should().NotBeNull();
    }

    [Fact]
    public void CollectionChanged_ActionIsAdd()
    {
        // Assert: Action ist Add
        _capturedAction.Should().Be(NotifyCollectionChangedAction.Add);
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
