using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.EditableCollectionViewModel.Commands.ClearCommand;

/// <summary>
/// Szenario: ClearCommand CanExecute ändert sich basierend auf Count.
/// Setup: Leeres ViewModel.
/// Test: CanExecute gibt false zurück wenn Count = 0, true wenn Count > 0.
/// </summary>
public sealed class CanExecute_ChangesWithCount : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly EditableCollectionViewModel<TestDto, TestViewModel> _sut;

    public CanExecute_ChangesWithCount(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        
        // Eigene ViewModel-Instanz für vollständige Test-Isolation
        _sut = new EditableCollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);
    }

    [Fact]
    public void CanExecute_ReturnsFalse_WhenCountIsZero()
    {
        // Arrange - Count ist 0 (initial state)

        // Act & Assert
        Assert.False(_sut.ClearCommand.CanExecute(null));
    }

    [Fact]
    public void CanExecute_ReturnsTrue_WhenCountIsGreaterThanZero()
    {
        // Arrange
        _sut.ModelStore.Add(new TestDto { Name = "TestItem" });

        // Act & Assert
        Assert.True(_sut.ClearCommand.CanExecute(null));
    }

    [Fact]
    public void CanExecute_ReturnsFalse_AfterRemovingAllItems()
    {
        // Arrange
        var dto = new TestDto { Name = "TestItem" };
        _sut.ModelStore.Add(dto);

        // Act
        _sut.ModelStore.Remove(dto);

        // Assert
        Assert.False(_sut.ClearCommand.CanExecute(null));
    }

    public void Dispose()
    {
        _sut?.Dispose();
    }
}
