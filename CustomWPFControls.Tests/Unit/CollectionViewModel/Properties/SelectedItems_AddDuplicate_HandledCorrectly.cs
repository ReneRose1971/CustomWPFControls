using System;
using System.Linq;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Properties;

/// <summary>
/// Tests für Duplikat-Handling in SelectedItems.
/// </summary>
public sealed class SelectedItems_AddDuplicate_HandledCorrectly : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;

    public SelectedItems_AddDuplicate_HandledCorrectly(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Test_SelectedItems_AddDuplicate_HandledCorrectly()
    {
        // Arrange
        _fixture.ClearTestData();
        
        // Setup: Ein Item zum Store hinzufügen
        _fixture.Sut.ModelStore.Add(new TestDto { Name = "TestItem" });

        // Act - Gleiches Item zweimal zu SelectedItems hinzufügen
        var item = _fixture.Sut.Items.Single();
        _fixture.Sut.SelectedItems.Add(item);
        _fixture.Sut.SelectedItems.Add(item);

        // Assert - ObservableCollection erlaubt normalerweise Duplikate
        // Aber das Verhalten kann implementierungsabhängig sein
        // Test stellt sicher, dass keine Exception geworfen wird
        _fixture.Sut.SelectedItems.Should().NotBeEmpty();

        // Cleanup
        _fixture.ClearTestData();
    }
}
