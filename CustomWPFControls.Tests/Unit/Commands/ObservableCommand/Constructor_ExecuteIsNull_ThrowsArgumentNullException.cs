using System;
using CustomWPFControls.Commands;
using Xunit;

namespace CustomWPFControls.Tests.Unit.Commands.ObservableCommandTests;

/// <summary>
/// Test: ObservableCommand-Konstruktor wirft ArgumentNullException wenn execute null ist.
/// </summary>
public sealed class Constructor_ExecuteIsNull_ThrowsArgumentNullException
{
    [Fact]
    public void Constructor_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ObservableCommand(null!));
    }
}
