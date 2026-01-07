using System;
using CustomWPFControls.Commands;
using Xunit;

namespace CustomWPFControls.Tests.Unit.Commands.ObservableCommandTests;

/// <summary>
/// Szenario: ObservableCommand ohne observedObject.
/// Test: RaiseCanExecuteChanged() kann manuell aufgerufen werden.
/// </summary>
public sealed class RaiseCanExecuteChanged_ManualCall_RaisesEvent
{
    [Fact]
    public void RaiseCanExecuteChanged_RaisesEvent()
    {
        // Arrange
        var command = new ObservableCommand(_ => { });
        
        bool eventRaised = false;
        command.CanExecuteChanged += (sender, e) => eventRaised = true;

        // Act
        command.RaiseCanExecuteChanged();

        // Assert
        Assert.True(eventRaised);
    }
}
