using System;
using System.ComponentModel;
using System.Windows.Input;

namespace CustomWPFControls.Commands
{
    /// <summary>
    /// ICommand-Implementierung die auf PropertyChanged-Events eines INotifyPropertyChanged-Objekts reagiert.
    /// Feuert CanExecuteChanged automatisch wenn sich überwachte Properties ändern.
    /// </summary>
    public class ObservableCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;
        private readonly INotifyPropertyChanged? _observedObject;
        private readonly string[]? _observedProperties;

        /// <summary>
        /// Erstellt ein ObservableCommand das auf PropertyChanged-Events reagiert.
        /// </summary>
        /// <param name="execute">Die auszuführende Aktion.</param>
        /// <param name="canExecute">Optionale Funktion zur Prüfung, ob das Command ausgeführt werden kann.</param>
        /// <param name="observedObject">Objekt dessen PropertyChanged-Events überwacht werden sollen.</param>
        /// <param name="observedProperties">Array von Property-Namen die überwacht werden sollen. Null oder leer = alle Properties.</param>
        /// <exception cref="ArgumentNullException">Wird ausgelöst, wenn execute null ist.</exception>
        public ObservableCommand(
            Action<object?> execute, 
            Func<object?, bool>? canExecute = null,
            INotifyPropertyChanged? observedObject = null,
            params string[]? observedProperties)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
            _observedObject = observedObject;
            _observedProperties = observedProperties;

            if (_observedObject != null)
            {
                _observedObject.PropertyChanged += OnObservedPropertyChanged;
            }
        }

        private void OnObservedPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Wenn keine spezifischen Properties angegeben wurden, oder
            // wenn das geänderte Property in der Liste ist, CanExecuteChanged feuern
            if (_observedProperties == null || 
                _observedProperties.Length == 0 || 
                string.IsNullOrEmpty(e.PropertyName) ||
                Array.IndexOf(_observedProperties, e.PropertyName) >= 0)
            {
                RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Bestimmt, ob das Command im aktuellen Zustand ausgeführt werden kann.
        /// </summary>
        /// <param name="parameter">Parameter für das Command.</param>
        /// <returns>true, wenn das Command ausgeführt werden kann; andernfalls false.</returns>
        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        /// <summary>
        /// Führt das Command aus.
        /// </summary>
        /// <param name="parameter">Parameter für das Command.</param>
        public void Execute(object? parameter)
        {
            _execute(parameter);
        }

        /// <summary>
        /// Tritt auf, wenn sich Änderungen ergeben, die sich darauf auswirken, ob das Command ausgeführt werden kann.
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// Löst das CanExecuteChanged-Event aus.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
