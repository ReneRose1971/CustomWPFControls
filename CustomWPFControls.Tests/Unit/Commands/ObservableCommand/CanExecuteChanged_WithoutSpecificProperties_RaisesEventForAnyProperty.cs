using System;
using System.ComponentModel;
using CustomWPFControls.Commands;
using Xunit;

namespace CustomWPFControls.Tests.Unit.Commands.ObservableCommandTests;

/// <summary>
/// Szenario: ObservableCommand überwacht ALLE Properties (keine spezifische Property-Liste).
/// Setup: Command wird ohne spezifische observedProperties erstellt.
/// </summary>
public sealed class CanExecuteChanged_WithoutSpecificProperties_RaisesEventForAnyProperty
{
    private class TestObservableObject : INotifyPropertyChanged
    {
        private string? _property1;
        private string? _property2;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string? Property1
        {
            get => _property1;
            set
            {
                if (_property1 != value)
                {
                    _property1 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Property1)));
                }
            }
        }

        public string? Property2
        {
            get => _property2;
            set
            {
                if (_property2 != value)
                {
                    _property2 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Property2)));
                }
            }
        }
    }

    [Fact]
    public void CanExecuteChanged_RaisesEvent_ForProperty1()
    {
        // Arrange
        var observable = new TestObservableObject();
        var command = new ObservableCommand(_ => { }, null, observable);
        
        bool eventRaised = false;
        command.CanExecuteChanged += (sender, e) => eventRaised = true;

        // Act
        observable.Property1 = "NewValue";

        // Assert
        Assert.True(eventRaised);
    }

    [Fact]
    public void CanExecuteChanged_RaisesEvent_ForProperty2()
    {
        // Arrange
        var observable = new TestObservableObject();
        var command = new ObservableCommand(_ => { }, null, observable);
        
        bool eventRaised = false;
        command.CanExecuteChanged += (sender, e) => eventRaised = true;

        // Act
        observable.Property2 = "NewValue";

        // Assert
        Assert.True(eventRaised);
    }
}
