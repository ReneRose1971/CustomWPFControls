using System;
using CustomWPFControls.Commands;
using Xunit;

namespace CustomWPFControls.Tests.Unit.Commands.ObservableCommandTests;

/// <summary>
/// Test: ObservableCommand.CanExecute() gibt true zurück wenn kein CanExecute bereitgestellt wurde.
/// </summary>
public sealed class CanExecute_NoCanExecuteProvided_ReturnsTrue
{
    [Fact]
    public void CanExecute_ReturnsTrue()
    {
        // Arrange
        var command = new ObservableCommand(_ => { });

        // Act & Assert
        Assert.True(command.CanExecute(null));
    }
}
