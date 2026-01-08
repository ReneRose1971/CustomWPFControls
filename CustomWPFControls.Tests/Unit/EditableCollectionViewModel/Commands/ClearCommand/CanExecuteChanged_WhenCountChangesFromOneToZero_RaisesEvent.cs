using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.EditableCollectionViewModel.Commands.ClearCommand;

/// <summary>
/// Szenario: ClearCommand reagiert auf Count-Änderungen.
/// Setup: ViewModel mit einem Item (Count = 1).
/// Test: CanExecuteChanged wird gefeuert wenn das Item entfernt wird (Count = 0).
/// </summary>
public sealed class CanExecuteChanged_WhenCountChangesFromOneToZero_RaisesEvent : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;
    private readonly EditableCollectionViewModel<TestDto, TestViewModel> _sut;
    private readonly TestDto _testDto;

    public CanExecuteChanged_WhenCountChangesFromOneToZero_RaisesEvent(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        
        // Eigene ViewModel-Instanz für vollständige Test-Isolation
        _sut = new EditableCollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        // Ausgangszustand: Ein Item (Count = 1)
        _testDto = new TestDto { Name = "TestItem" };
        _sut.ModelStore.Add(_testDto);
    }

    [Fact]
    public void CanExecuteChanged_RaisesEvent_WhenItemIsRemoved()
    {
        // Arrange
        bool eventRaised = false;
        _sut.ClearCommand.CanExecuteChanged += (sender, e) => eventRaised = true;

        // Act
        _sut.ModelStore.Remove(_testDto);

        // Assert
        Assert.True(eventRaised);
    }

    public void Dispose()
    {
        _sut?.Dispose();
    }
}
