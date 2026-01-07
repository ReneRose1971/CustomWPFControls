using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.EditableCollectionViewModel.Commands.EditCommand;

/// <summary>
/// Szenario: EditCommand reagiert auf SelectedItem-Änderungen.
/// Setup: ViewModel mit einem Item, EditModel-Delegate gesetzt, SelectedItem ist null.
/// Test: CanExecuteChanged wird gefeuert wenn SelectedItem gesetzt wird.
/// </summary>
public sealed class CanExecuteChanged_WhenSelectedItemChangesFromNullToItem_RaisesEvent : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly EditableCollectionViewModel<TestDto, TestViewModel> _sut;
    private readonly TestDto _testDto;

    public CanExecuteChanged_WhenSelectedItemChangesFromNullToItem_RaisesEvent(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        
        // Eigene ViewModel-Instanz für vollständige Test-Isolation
        _sut = new EditableCollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        // Setup: EditModel-Delegate setzen
        _sut.EditModel = _ => { };

        // Setup: Ein Item hinzufügen
        _testDto = new TestDto { Name = "TestItem" };
        _sut.ModelStore.Add(_testDto);
        
        // Ausgangszustand: SelectedItem ist null
        _sut.SelectedItem = null;
    }

    [Fact]
    public void CanExecuteChanged_RaisesEvent_WhenSelectedItemIsSet()
    {
        // Arrange
        bool eventRaised = false;
        _sut.EditCommand.CanExecuteChanged += (sender, e) => eventRaised = true;

        // Act
        _sut.SelectedItem = _sut.Items[0];

        // Assert
        Assert.True(eventRaised);
    }

    public void Dispose()
    {
        _sut?.Dispose();
    }
}
