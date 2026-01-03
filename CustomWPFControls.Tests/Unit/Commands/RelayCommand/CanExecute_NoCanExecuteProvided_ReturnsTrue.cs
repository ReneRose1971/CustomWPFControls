using System;
using CustomWPFControls.Commands;
using Xunit;

namespace CustomWPFControls.Tests.Unit.Commands.RelayCommandTests;

/// <summary>
/// Test: RelayCommand.CanExecute() gibt true zurück wenn kein CanExecute bereitgestellt wurde.
/// </summary>
public sealed class CanExecute_NoCanExecuteProvided_ReturnsTrue
{
    [Fact]
    public void CanExecute_ReturnsTrue()
    {
        // Arrange
        var command = new RelayCommand(_ => { });

        // Act & Assert
        Assert.True(command.CanExecute(null));
    }
}
