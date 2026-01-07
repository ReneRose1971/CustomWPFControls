using System;
using System.ComponentModel;
using CustomWPFControls.Commands;
using Xunit;

namespace CustomWPFControls.Tests.Unit.Commands.ObservableCommandTests;

/// <summary>
/// Szenario: ObservableCommand überwacht ein spezifisches Property.
/// Setup: Command überwacht "TestProperty" eines INotifyPropertyChanged-Objekts.
/// </summary>
public sealed class CanExecuteChanged_WhenObservedPropertyChanges_RaisesEvent
{
    private class TestObservableObject : INotifyPropertyChanged
    {
        private string? _testProperty;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string? TestProperty
        {
            get => _testProperty;
            set
            {
                if (_testProperty != value)
                {
                    _testProperty = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TestProperty)));
                }
            }
        }
    }

    [Fact]
    public void CanExecuteChanged_RaisesEvent_WhenObservedPropertyChanges()
    {
        // Arrange
        var observable = new TestObservableObject();
        var command = new ObservableCommand(_ => { }, null, observable, nameof(TestObservableObject.TestProperty));
        
        bool eventRaised = false;
        command.CanExecuteChanged += (sender, e) => eventRaised = true;

        // Act
        observable.TestProperty = "NewValue";

        // Assert
        Assert.True(eventRaised);
    }
}
