using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.EditableCollectionViewModel.Commands.DeleteCommand;

/// <summary>
/// Szenario: DeleteCommand reagiert auf SelectedItem-Änderungen.
/// Setup: ViewModel mit einem Item, SelectedItem ist gesetzt.
/// Test: CanExecuteChanged wird gefeuert wenn SelectedItem auf null gesetzt wird.
/// </summary>
public sealed class CanExecuteChanged_WhenSelectedItemChangesFromItemToNull_RaisesEvent : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;
    private readonly EditableCollectionViewModel<TestDto, TestViewModel> _sut;
    private readonly TestDto _testDto;

    public CanExecuteChanged_WhenSelectedItemChangesFromItemToNull_RaisesEvent(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        
        // Eigene ViewModel-Instanz für vollständige Test-Isolation
        _sut = new EditableCollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        // Setup: Ein Item hinzufügen und selektieren
        _testDto = new TestDto { Name = "TestItem" };
        _sut.ModelStore.Add(_testDto);
        _sut.SelectedItem = _sut.Items[0];
    }

    [Fact]
    public void CanExecuteChanged_RaisesEvent_WhenSelectedItemSetToNull()
    {
        // Arrange
        bool eventRaised = false;
        _sut.DeleteCommand.CanExecuteChanged += (sender, e) => eventRaised = true;

        // Act
        _sut.SelectedItem = null;

        // Assert
        Assert.True(eventRaised);
    }

    public void Dispose()
    {
        _sut?.Dispose();
    }
}
