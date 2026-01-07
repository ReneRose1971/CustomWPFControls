using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.EditableCollectionViewModel.Commands.ClearCommand;

/// <summary>
/// Szenario: ClearCommand reagiert auf Count-Änderungen.
/// Setup: ViewModel ist leer (Count = 0).
/// Test: CanExecuteChanged wird gefeuert wenn ein Item hinzugefügt wird (Count > 0).
/// </summary>
public sealed class CanExecuteChanged_WhenCountChangesFromZeroToOne_RaisesEvent : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly EditableCollectionViewModel<TestDto, TestViewModel> _sut;

    public CanExecuteChanged_WhenCountChangesFromZeroToOne_RaisesEvent(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        
        // Eigene ViewModel-Instanz für vollständige Test-Isolation
        _sut = new EditableCollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        // Ausgangszustand: Keine Items (Count = 0)
    }

    [Fact]
    public void CanExecuteChanged_RaisesEvent_WhenItemIsAdded()
    {
        // Arrange
        bool eventRaised = false;
        _sut.ClearCommand.CanExecuteChanged += (sender, e) => eventRaised = true;

        // Act
        _sut.ModelStore.Add(new TestDto { Name = "TestItem" });

        // Assert
        Assert.True(eventRaised);
    }

    public void Dispose()
    {
        _sut?.Dispose();
    }
}
