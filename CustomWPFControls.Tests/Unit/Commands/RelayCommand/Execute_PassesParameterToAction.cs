using System;
using CustomWPFControls.Commands;
using Xunit;

namespace CustomWPFControls.Tests.Unit.Commands.RelayCommandTests;

/// <summary>
/// Test: RelayCommand.Execute() übergibt den Parameter an die Action.
/// </summary>
public sealed class Execute_PassesParameterToAction
{
    [Fact]
    public void Execute_PassesParameter()
    {
        // Arrange
        object? receivedParameter = null;
        var command = new RelayCommand(param => receivedParameter = param);
        var testParameter = "TestValue";

        // Act
        command.Execute(testParameter);

        // Assert
        Assert.Same(testParameter, receivedParameter);
    }
}
