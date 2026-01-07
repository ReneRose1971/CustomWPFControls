using System;
using CustomWPFControls.Commands;
using Xunit;

namespace CustomWPFControls.Tests.Unit.Commands.ObservableCommandTests;

/// <summary>
/// Test: ObservableCommand.Execute() ruft die bereitgestellte Action auf.
/// </summary>
public sealed class Execute_CallsProvidedAction
{
    [Fact]
    public void Execute_CallsAction()
    {
        // Arrange
        bool actionCalled = false;
        var command = new ObservableCommand(_ => actionCalled = true);

        // Act
        command.Execute(null);

        // Assert
        Assert.True(actionCalled);
    }
}
