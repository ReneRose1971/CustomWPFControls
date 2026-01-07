using System;
using CustomWPFControls.Commands;
using Xunit;

namespace CustomWPFControls.Tests.Unit.Commands.ObservableCommandTests;

/// <summary>
/// Test: ObservableCommand.Execute() übergibt den Parameter an die Action.
/// </summary>
public sealed class Execute_PassesParameterToAction
{
    [Fact]
    public void Execute_PassesParameter()
    {
        // Arrange
        object? receivedParameter = null;
        var command = new ObservableCommand(p => receivedParameter = p);
        var expectedParameter = new object();

        // Act
        command.Execute(expectedParameter);

        // Assert
        Assert.Same(expectedParameter, receivedParameter);
    }
}
