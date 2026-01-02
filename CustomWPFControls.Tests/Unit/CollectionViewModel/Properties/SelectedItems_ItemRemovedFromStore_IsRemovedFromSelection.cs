using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
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
public sealed class SelectedItems_ItemRemovedViaRemove_IsRemovedFromSelection : IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly CollectionViewModel<TestDto, TestViewModel> _sut;

    public SelectedItems_ItemRemovedViaRemove_IsRemovedFromSelection()
    {
        _fixture = new CollectionViewModelFixture();
        
        // Setup: 3 Items hinzufügen
        _fixture.TestDtoStore.AddRange(new[]
        {
            new TestDto { Name = "First" },
            new TestDto { Name = "Second" },
            new TestDto { Name = "Third" }
        });

        _sut = new CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);
    }

    [Fact]
    public void ShouldRemoveItemFromSelectedItems()
    {
        // Arrange - 2 Items selektieren
        var firstItem = _sut.Items[0];
        var secondItem = _sut.Items[1];
        
        _sut.SelectedItems.Add(firstItem);
        _sut.SelectedItems.Add(secondItem);

        // Act - Erstes Item via Remove() entfernen
        _sut.Remove(firstItem);

        // Assert - Nur noch das zweite Item sollte selektiert sein
        _sut.SelectedItems.Should().ContainSingle();
        _sut.SelectedItems[0].Name.Should().Be("Second");
    }

    [Fact]
    public void ShouldNotAffectOtherSelectedItems()
    {
        // Arrange - Alle 3 Items selektieren
        var firstItem = _sut.Items[0];
        var secondItem = _sut.Items[1];
        var thirdItem = _sut.Items[2];
        
        _sut.SelectedItems.Add(firstItem);
        _sut.SelectedItems.Add(secondItem);
        _sut.SelectedItems.Add(thirdItem);

        // Act - Mittleres Item via Remove() entfernen
        _sut.Remove(secondItem);

        // Assert - Nur die verbleibenden 2 sollten selektiert sein
        _sut.SelectedItems.Should().HaveCount(2);
        _sut.SelectedItems.Should().Contain(vm => vm.Name == "First");
        _sut.SelectedItems.Should().Contain(vm => vm.Name == "Third");
    }

    public void Dispose()
    {
        _sut?.Dispose();
        _fixture?.Dispose();
    }
}
