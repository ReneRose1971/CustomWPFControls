using System;
using CustomWPFControls.Commands;
using Xunit;

namespace CustomWPFControls.Tests.Unit.Commands.ObservableCommandTests;

/// <summary>
/// Test: ObservableCommand.CanExecute() gibt das Ergebnis der CanExecute-Funktion zurück.
/// </summary>
public sealed class CanExecute_ReturnsFunctionResult
{
    [Fact]
    public void CanExecute_ReturnsTrue_WhenFunctionReturnsTrue()
    {
        // Arrange
        var command = new ObservableCommand(_ => { }, _ => true);

        // Act & Assert
        Assert.True(command.CanExecute(null));
    }

    [Fact]
    public void CanExecute_ReturnsFalse_WhenFunctionReturnsFalse()
    {
        // Arrange
        var command = new ObservableCommand(_ => { }, _ => false);

        // Act & Assert
        Assert.False(command.CanExecute(null));
    }
}
