using System;
using System.ComponentModel;
using CustomWPFControls.Commands;
using Xunit;

namespace CustomWPFControls.Tests.Unit.Commands.ObservableCommandTests;

/// <summary>
/// Szenario: ObservableCommand überwacht ein spezifisches Property.
/// Setup: Command überwacht "TestProperty", aber ein anderes Property ändert sich.
/// </summary>
public sealed class CanExecuteChanged_WhenOtherPropertyChanges_DoesNotRaiseEvent
{
    private class TestObservableObject : INotifyPropertyChanged
    {
        private string? _testProperty;
        private string? _otherProperty;

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

        public string? OtherProperty
        {
            get => _otherProperty;
            set
            {
                if (_otherProperty != value)
                {
                    _otherProperty = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OtherProperty)));
                }
            }
        }
    }

    [Fact]
    public void CanExecuteChanged_DoesNotRaiseEvent_WhenOtherPropertyChanges()
    {
        // Arrange
        var observable = new TestObservableObject();
        var command = new ObservableCommand(_ => { }, null, observable, nameof(TestObservableObject.TestProperty));
        
        bool eventRaised = false;
        command.CanExecuteChanged += (sender, e) => eventRaised = true;

        // Act
        observable.OtherProperty = "NewValue";

        // Assert
        Assert.False(eventRaised);
    }
}
