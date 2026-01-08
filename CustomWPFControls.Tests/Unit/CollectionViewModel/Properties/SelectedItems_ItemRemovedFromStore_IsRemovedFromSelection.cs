using System;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Properties;

/// <summary>
/// Tests für SelectedItems wenn ein selektiertes Item via Remove() entfernt wird.
/// </summary>
/// <remarks>
/// Kritisch für WPF-Binding: Wenn ein selektiertes Item gelöscht wird,
/// muss es aus SelectedItems entfernt werden.
/// </remarks>
public sealed class SelectedItems_ItemRemovedFromStore_IsRemovedFromSelection : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;

    public SelectedItems_ItemRemovedFromStore_IsRemovedFromSelection(TestHelperCustomWPFControlsTestFixture fixture)
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
    public void ShouldRemoveItemFromSelectedItems()
    {
        // Arrange - 2 Items selektieren
        var firstItem = _fixture.Sut.Items[0];
        var secondItem = _fixture.Sut.Items[1];
        
        _fixture.Sut.SelectedItems.Add(firstItem);
        _fixture.Sut.SelectedItems.Add(secondItem);

        // Act - Erstes Item via Remove() entfernen
        _fixture.Sut.Remove(firstItem);

        // Assert - Nur noch das zweite Item sollte selektiert sein
        _fixture.Sut.SelectedItems.Should().ContainSingle();
        _fixture.Sut.SelectedItems[0].Name.Should().Be("Second");
    }

    [Fact]
    public void ShouldNotAffectOtherSelectedItems()
    {
        // Arrange - Alle 3 Items selektieren
        var firstItem = _fixture.Sut.Items[0];
        var secondItem = _fixture.Sut.Items[1];
        var thirdItem = _fixture.Sut.Items[2];
        
        _fixture.Sut.SelectedItems.Add(firstItem);
        _fixture.Sut.SelectedItems.Add(secondItem);
        _fixture.Sut.SelectedItems.Add(thirdItem);

        // Act - Mittleres Item via Remove() entfernen
        _fixture.Sut.Remove(secondItem);

        // Assert - Nur die verbleibenden 2 sollten selektiert sein
        _fixture.Sut.SelectedItems.Should().HaveCount(2);
        _fixture.Sut.SelectedItems.Should().Contain(vm => vm.Name == "First");
        _fixture.Sut.SelectedItems.Should().Contain(vm => vm.Name == "Third");
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
