using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CustomWPFControls.Services.Dialogs;

namespace CustomWPFControls.Testing.Dialogs
{
    /// <summary>
    /// Mock-Implementation des IDialogService für Unit-Tests.
    /// Zeichnet alle Dialog-Aufrufe auf ohne UI anzuzeigen.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Verwenden Sie diese Klasse in Unit-Tests, um Dialoge zu simulieren ohne WPF-UI zu laden.
    /// </para>
    /// <para>
    /// <b>Vorkonfigurierte Antworten:</b> Setzen Sie <see cref="NextDialogResult"/> und 
    /// <see cref="NextConfirmationResult"/> um Benutzer-Aktionen zu simulieren.
    /// </para>
    /// <para>
    /// <b>Assertions:</b> Verwenden Sie <see cref="Calls"/> oder die Verify-Methoden
    /// um zu prüfen, ob Dialoge korrekt aufgerufen wurden.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// [Fact]
    /// public void AddCustomer_ShowsEditDialog()
    /// {
    ///     // Arrange
    ///     var mockDialog = new MockDialogService();
    ///     mockDialog.NextDialogResult = true;  // Simuliert OK-Klick
    ///     
    ///     var viewModel = new CustomerListViewModel(dataStore, factory, comparer, mockDialog);
    ///     
    ///     // Act
    ///     viewModel.AddCustomerCommand.Execute(null);
    ///     
    ///     // Assert
    ///     mockDialog.VerifyDialogShown&lt;CustomerEditViewModel&gt;();
    ///     Assert.Single(mockDialog.Calls);
    /// }
    /// </code>
    /// </example>
    public sealed class MockDialogService : IDialogService
    {
        private readonly List<DialogCall> _calls = new();

        /// <summary>
        /// Vorkonfiguriertes Ergebnis für den nächsten ShowDialog-Aufruf.
        /// </summary>
        /// <remarks>
        /// Standard: <c>true</c> (simuliert OK/Accept-Klick)
        /// </remarks>
        public bool? NextDialogResult { get; set; } = true;

        /// <summary>
        /// Vorkonfiguriertes Ergebnis für den nächsten ShowConfirmation-Aufruf.
        /// </summary>
        /// <remarks>
        /// Standard: <c>true</c> (simuliert Ja-Klick)
        /// </remarks>
        public bool NextConfirmationResult { get; set; } = true;

        /// <summary>
        /// Vorkonfiguriertes Ergebnis für den nächsten ShowMessageBox-Aufruf.
        /// </summary>
        public MessageBoxResult NextMessageBoxResult { get; set; } = MessageBoxResult.OK;

        /// <summary>
        /// Aufgezeichnete Dialog-Aufrufe (read-only).
        /// </summary>
        public IReadOnlyList<DialogCall> Calls => _calls.AsReadOnly();

        // ????????????????????????????????????????????????????????????
        // Modal Dialogs
        // ????????????????????????????????????????????????????????????

        public bool? ShowDialog<TViewModel>(TViewModel viewModel, Window? owner = null)
            where TViewModel : class
        {
            return ShowDialog(viewModel, null, null, null, owner);
        }

        public bool? ShowDialog<TViewModel>(TViewModel viewModel, string title, Window? owner = null)
            where TViewModel : class
        {
            return ShowDialog(viewModel, title, null, null, owner);
        }

        public bool? ShowDialog<TViewModel>(
            TViewModel viewModel,
            string? title = null,
            double? width = null,
            double? height = null,
            Window? owner = null)
            where TViewModel : class
        {
            _calls.Add(new DialogCall
            {
                Type = DialogType.Modal,
                ViewModel = viewModel,
                ViewModelType = typeof(TViewModel),
                Title = title,
                Width = width,
                Height = height,
                Owner = owner
            });

            return NextDialogResult;
        }

        // ????????????????????????????????????????????????????????????
        // Modeless Windows
        // ????????????????????????????????????????????????????????????

        public Window ShowWindow<TViewModel>(TViewModel viewModel, string? title = null)
            where TViewModel : class
        {
            _calls.Add(new DialogCall
            {
                Type = DialogType.Modeless,
                ViewModel = viewModel,
                ViewModelType = typeof(TViewModel),
                Title = title
            });

            // Dummy Window für Tests (wird nicht wirklich erstellt, nur referenziert)
            return null!;
        }

        public Window ShowSingletonWindow<TViewModel>(TViewModel viewModel, string? title = null)
            where TViewModel : class
        {
            _calls.Add(new DialogCall
            {
                Type = DialogType.SingletonWindow,
                ViewModel = viewModel,
                ViewModelType = typeof(TViewModel),
                Title = title
            });

            // Dummy Window für Tests
            return null!;
        }

        public void CloseWindow(Window window)
        {
            _calls.Add(new DialogCall
            {
                Type = DialogType.CloseWindow,
                Window = window
            });
        }

        // ????????????????????????????????????????????????????????????
        // MessageBoxen
        // ????????????????????????????????????????????????????????????

        public void ShowMessage(string message, string title = "Information")
        {
            _calls.Add(new DialogCall
            {
                Type = DialogType.Message,
                Message = message,
                Title = title,
                Icon = MessageBoxImage.Information
            });
        }

        public void ShowWarning(string message, string title = "Warnung")
        {
            _calls.Add(new DialogCall
            {
                Type = DialogType.Warning,
                Message = message,
                Title = title,
                Icon = MessageBoxImage.Warning
            });
        }

        public void ShowError(string message, string title = "Fehler")
        {
            _calls.Add(new DialogCall
            {
                Type = DialogType.Error,
                Message = message,
                Title = title,
                Icon = MessageBoxImage.Error
            });
        }

        public bool ShowConfirmation(string message, string title = "Bestätigung")
        {
            _calls.Add(new DialogCall
            {
                Type = DialogType.Confirmation,
                Message = message,
                Title = title,
                Icon = MessageBoxImage.Question
            });

            return NextConfirmationResult;
        }

        public MessageBoxResult ShowMessageBox(
            string message,
            string title,
            MessageBoxButton buttons,
            MessageBoxImage icon)
        {
            _calls.Add(new DialogCall
            {
                Type = DialogType.MessageBox,
                Message = message,
                Title = title,
                Buttons = buttons,
                Icon = icon
            });

            return NextMessageBoxResult;
        }

        // ????????????????????????????????????????????????????????????
        // Erweiterte Features
        // ????????????????????????????????????????????????????????????

        public TResult? ShowDialog<TViewModel, TResult>(TViewModel viewModel, Window? owner = null)
            where TViewModel : class, IDialogViewModel<TResult>
        {
            ShowDialog(viewModel, owner);

            return viewModel.IsConfirmed ? viewModel.Result : default;
        }

        public Task<bool?> ShowDialogAsync<TViewModel>(TViewModel viewModel, Window? owner = null)
            where TViewModel : class
        {
            return Task.FromResult(ShowDialog(viewModel, owner));
        }

        // ????????????????????????????????????????????????????????????
        // Convention-based Dialogs
        // ????????????????????????????????????????????????????????????

        public bool? ShowDialog<TViewModel>(string? title = null, Window? owner = null)
            where TViewModel : class, IDialogViewModelMarker
        {
            _calls.Add(new DialogCall
            {
                Type = DialogType.Modal,
                ViewModelType = typeof(TViewModel),
                Title = title,
                Owner = owner
            });

            return NextDialogResult;
        }
        // ????????????????????????????????????????????????????????????
        // Test-Hilfsmethoden
        // ????????????????????????????????????????????????????????????

        /// <summary>
        /// Setzt alle aufgezeichneten Aufrufe und Konfigurationen zurück.
        /// </summary>
        public void Reset()
        {
            _calls.Clear();
            NextDialogResult = true;
            NextConfirmationResult = true;
            NextMessageBoxResult = MessageBoxResult.OK;
        }

        /// <summary>
        /// Verifiziert, dass ein Dialog für den angegebenen ViewModel-Typ angezeigt wurde.
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel-Typ</typeparam>
        /// <exception cref="AssertionException">Wenn kein Dialog für den Typ gefunden wurde</exception>
        public void VerifyDialogShown<TViewModel>() where TViewModel : class
        {
            if (!_calls.Any(c => c.ViewModelType == typeof(TViewModel)))
            {
                throw new AssertionException(
                    $"Kein Dialog für ViewModel '{typeof(TViewModel).Name}' aufgerufen. " +
                    $"Aufgerufene ViewModels: {string.Join(", ", _calls.Select(c => c.ViewModelType?.Name ?? "null"))}");
            }
        }

        /// <summary>
        /// Verifiziert, dass eine Nachricht mit dem angegebenen Text angezeigt wurde.
        /// </summary>
        /// <param name="expectedMessage">Erwarteter Nachrichtentext</param>
        /// <exception cref="AssertionException">Wenn keine passende Nachricht gefunden wurde</exception>
        public void VerifyMessageShown(string expectedMessage)
        {
            if (!_calls.Any(c => c.Message == expectedMessage))
            {
                throw new AssertionException(
                    $"Keine Nachricht mit Text '{expectedMessage}' angezeigt. " +
                    $"Angezeigte Nachrichten: {string.Join(", ", _calls.Where(c => c.Message != null).Select(c => $"'{c.Message}'"))}");
            }
        }

        /// <summary>
        /// Verifiziert, dass mindestens ein Dialog aufgerufen wurde.
        /// </summary>
        /// <exception cref="AssertionException">Wenn keine Dialoge aufgerufen wurden</exception>
        public void VerifyAnyDialogShown()
        {
            if (_calls.Count == 0)
            {
                throw new AssertionException("Keine Dialoge aufgerufen.");
            }
        }

        /// <summary>
        /// Gibt den ersten Aufruf für den angegebenen ViewModel-Typ zurück.
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel-Typ</typeparam>
        /// <returns>DialogCall oder null wenn nicht gefunden</returns>
        public DialogCall? GetCallFor<TViewModel>() where TViewModel : class
        {
            return _calls.FirstOrDefault(c => c.ViewModelType == typeof(TViewModel));
        }
    }

    /// <summary>
    /// Repräsentiert einen aufgezeichneten Dialog-Aufruf.
    /// </summary>
    public class DialogCall
    {
        public DialogType Type { get; init; }
        public object? ViewModel { get; init; }
        public Type? ViewModelType { get; init; }
        public string? Message { get; init; }
        public string? Title { get; init; }
        public double? Width { get; init; }
        public double? Height { get; init; }
        public MessageBoxButton? Buttons { get; init; }
        public MessageBoxImage Icon { get; init; }
        public Window? Owner { get; init; }
        public Window? Window { get; init; }
    }

    /// <summary>
    /// Dialog-Typen für MockDialogService.
    /// </summary>
    public enum DialogType
    {
        Modal,
        Modeless,
        SingletonWindow,
        CloseWindow,
        Message,
        Warning,
        Error,
        Confirmation,
        MessageBox
    }

    /// <summary>
    /// Exception für fehlgeschlagene Assertions in MockDialogService.
    /// </summary>
    public class AssertionException : Exception
    {
        public AssertionException(string message) : base(message) { }
    }
}
