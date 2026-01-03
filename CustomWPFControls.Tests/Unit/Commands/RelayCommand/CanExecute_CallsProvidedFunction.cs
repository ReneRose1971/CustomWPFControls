using System;
using CustomWPFControls.Commands;
using Xunit;

namespace CustomWPFControls.Tests.Unit.Commands.RelayCommandTests;

/// <summary>
/// Test: RelayCommand.CanExecute() ruft die bereitgestellte Funktion auf.
/// </summary>
public sealed class CanExecute_CallsProvidedFunction
{
    [Fact]
    public void CanExecute_CallsFunction()
    {
        // Arrange
        bool functionCalled = false;
        var command = new RelayCommand(
            _ => { },
            _ => { functionCalled = true; return true; });

        // Act
        command.CanExecute(null);

        // Assert
        Assert.True(functionCalled);
    }
}
