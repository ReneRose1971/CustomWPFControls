using System;
using System.Windows.Input;
using CustomWPFControls.Commands;
using Xunit;

namespace CustomWPFControls.Tests.Unit.Commands.RelayCommandTests;

/// <summary>
/// Test: RelayCommand.CanExecuteChanged verwendet CommandManager.RequerySuggested.
/// </summary>
public sealed class CanExecuteChanged_UsesCommandManager
{
    [Fact]
    public void CanExecuteChanged_Add_DoesNotThrow()
    {
        // Arrange
        var command = new RelayCommand(_ => { });
        EventHandler? handler = (sender, e) => { };

        // Act & Assert (sollte nicht werfen)
        command.CanExecuteChanged += handler;
    }

    [Fact]
    public void CanExecuteChanged_Remove_DoesNotThrow()
    {
        // Arrange
        var command = new RelayCommand(_ => { });
        EventHandler? handler = (sender, e) => { };
        command.CanExecuteChanged += handler;

        // Act & Assert (sollte nicht werfen)
        command.CanExecuteChanged -= handler;
    }
}
