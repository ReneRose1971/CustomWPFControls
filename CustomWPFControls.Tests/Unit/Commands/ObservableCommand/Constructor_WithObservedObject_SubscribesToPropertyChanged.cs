using System;
using System.ComponentModel;
using CustomWPFControls.Commands;
using Xunit;

namespace CustomWPFControls.Tests.Unit.Commands.ObservableCommandTests;

/// <summary>
/// Test: ObservableCommand-Konstruktor abonniert PropertyChanged-Event wenn observedObject übergeben wird.
/// </summary>
public sealed class Constructor_WithObservedObject_SubscribesToPropertyChanged
{
    private class TestObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int PropertyChangedSubscriberCount => PropertyChanged?.GetInvocationList().Length ?? 0;
    }

    [Fact]
    public void Constructor_SubscribesToPropertyChanged()
    {
        // Arrange
        var observable = new TestObservableObject();
        var initialCount = observable.PropertyChangedSubscriberCount;

        // Act
        var command = new ObservableCommand(_ => { }, null, observable, "TestProperty");

        // Assert
        Assert.Equal(initialCount + 1, observable.PropertyChangedSubscriberCount);
    }
}
