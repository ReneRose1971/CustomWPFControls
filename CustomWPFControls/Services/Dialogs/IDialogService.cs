using System;
using System.Threading.Tasks;
using System.Windows;

namespace CustomWPFControls.Services.Dialogs
{
    /// <summary>
    /// Service für die Darstellung von Dialogen, MessageBoxen und Windows.
    /// Ermöglicht testbare, ViewModel-basierte Dialoge im MVVM-Pattern.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Modal Dialogs:</b> Verwenden Sie <see cref="ShowDialog{TViewModel}"/> für blockierende Dialoge
    /// mit Ergebnis-Rückgabe (true/false/null).
    /// </para>
    /// <para>
    /// <b>Modeless Windows:</b> Verwenden Sie <see cref="ShowWindow{TViewModel}"/> für nicht-blockierende Fenster.
    /// </para>
    /// <para>
    /// <b>MessageBoxen:</b> Verwenden Sie <see cref="ShowMessage"/>, <see cref="ShowWarning"/>, 
    /// <see cref="ShowError"/> oder <see cref="ShowConfirmation"/> für Standard-Nachrichten.
    /// </para>
    /// <para>
    /// <b>Testbarkeit:</b> In Unit-Tests kann <c>MockDialogService</c> verwendet werden, 
    /// um Dialoge zu simulieren ohne UI anzuzeigen.
    /// </para>
    /// </remarks>
    public interface IDialogService
    {
        // ????????????????????????????????????????????????????????????
        // Modal Dialogs
        // ????????????????????????????????????????????????????????????

        /// <summary>
        /// Zeigt einen modalen Dialog für das angegebene ViewModel.
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel-Typ (muss in <see cref="IDialogRegistry"/> registriert sein)</typeparam>
        /// <param name="viewModel">ViewModel-Instanz (wird als DataContext gesetzt)</param>
        /// <param name="owner">Besitzer-Window (optional, null = zentriert auf Bildschirm)</param>
        /// <returns>
        /// <c>true</c> wenn Dialog mit OK/Accept geschlossen wurde,
        /// <c>false</c> wenn Dialog mit Cancel geschlossen wurde,
        /// <c>null</c> wenn Dialog ohne Aktion geschlossen wurde (z.B. via X-Button)
        /// </returns>
        /// <exception cref="ArgumentNullException">Wenn <paramref name="viewModel"/> null ist</exception>
        /// <exception cref="InvalidOperationException">Wenn keine View für den ViewModel-Typ registriert ist</exception>
        bool? ShowDialog<TViewModel>(TViewModel viewModel, Window? owner = null)
            where TViewModel : class;

        /// <summary>
        /// Zeigt einen modalen Dialog mit Titel.
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel-Typ</typeparam>
        /// <param name="viewModel">ViewModel-Instanz</param>
        /// <param name="title">Fenstertitel</param>
        /// <param name="owner">Besitzer-Window (optional)</param>
        /// <returns>Dialog-Ergebnis (true/false/null)</returns>
        bool? ShowDialog<TViewModel>(TViewModel viewModel, string title, Window? owner = null)
            where TViewModel : class;

        /// <summary>
        /// Zeigt einen modalen Dialog mit erweiterten Optionen.
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel-Typ</typeparam>
        /// <param name="viewModel">ViewModel-Instanz</param>
        /// <param name="title">Fenstertitel (optional)</param>
        /// <param name="width">Fensterbreite in Pixel (optional)</param>
        /// <param name="height">Fensterhöhe in Pixel (optional)</param>
        /// <param name="owner">Besitzer-Window (optional)</param>
        /// <returns>Dialog-Ergebnis (true/false/null)</returns>
        bool? ShowDialog<TViewModel>(
            TViewModel viewModel,
            string? title = null,
            double? width = null,
            double? height = null,
            Window? owner = null)
            where TViewModel : class;

        // ????????????????????????????????????????????????????????????
        // Modeless Windows
        // ????????????????????????????????????????????????????????????

        /// <summary>
        /// Zeigt ein modelloses (nicht-blockierendes) Window.
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel-Typ</typeparam>
        /// <param name="viewModel">ViewModel-Instanz</param>
        /// <param name="title">Fenstertitel (optional)</param>
        /// <returns>Window-Instanz für Lifecycle-Management (z.B. zum späteren Schließen)</returns>
        /// <exception cref="ArgumentNullException">Wenn <paramref name="viewModel"/> null ist</exception>
        Window ShowWindow<TViewModel>(TViewModel viewModel, string? title = null)
            where TViewModel : class;

        /// <summary>
        /// Zeigt ein Singleton-Window (nur eine Instanz pro ViewModel-Typ erlaubt).
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel-Typ</typeparam>
        /// <param name="viewModel">ViewModel-Instanz</param>
        /// <param name="title">Fenstertitel (optional)</param>
        /// <returns>
        /// Neue Window-Instanz wenn noch keine existiert,
        /// andernfalls die bestehende Instanz (wird aktiviert)
        /// </returns>
        /// <remarks>
        /// Wenn bereits ein Window für diesen ViewModel-Typ geöffnet ist,
        /// wird dieses aktiviert (in den Vordergrund geholt) statt ein neues zu erstellen.
        /// </remarks>
        Window ShowSingletonWindow<TViewModel>(TViewModel viewModel, string? title = null)
            where TViewModel : class;

        /// <summary>
        /// Schließt ein Window programmatisch.
        /// </summary>
        /// <param name="window">Das zu schließende Window</param>
        /// <exception cref="ArgumentNullException">Wenn <paramref name="window"/> null ist</exception>
        void CloseWindow(Window window);

        // ????????????????????????????????????????????????????????????
        // MessageBoxen
        // ????????????????????????????????????????????????????????????

        /// <summary>
        /// Zeigt eine Informations-Nachricht.
        /// </summary>
        /// <param name="message">Nachrichtentext</param>
        /// <param name="title">Fenstertitel (Standard: "Information")</param>
        void ShowMessage(string message, string title = "Information");

        /// <summary>
        /// Zeigt eine Warnung.
        /// </summary>
        /// <param name="message">Warnungstext</param>
        /// <param name="title">Fenstertitel (Standard: "Warnung")</param>
        void ShowWarning(string message, string title = "Warnung");

        /// <summary>
        /// Zeigt eine Fehlermeldung.
        /// </summary>
        /// <param name="message">Fehlertext</param>
        /// <param name="title">Fenstertitel (Standard: "Fehler")</param>
        void ShowError(string message, string title = "Fehler");

        /// <summary>
        /// Zeigt eine Bestätigungsabfrage (Ja/Nein).
        /// </summary>
        /// <param name="message">Abfragetext</param>
        /// <param name="title">Fenstertitel (Standard: "Bestätigung")</param>
        /// <returns><c>true</c> wenn Ja geklickt wurde, <c>false</c> wenn Nein geklickt wurde</returns>
        bool ShowConfirmation(string message, string title = "Bestätigung");

        /// <summary>
        /// Zeigt eine MessageBox mit benutzerdefinierten Buttons und Icon.
        /// </summary>
        /// <param name="message">Nachrichtentext</param>
        /// <param name="title">Fenstertitel</param>
        /// <param name="buttons">Button-Konfiguration (z.B. OK, YesNo, YesNoCancel)</param>
        /// <param name="icon">Icon (z.B. Information, Warning, Error, Question)</param>
        /// <returns>Ergebnis der MessageBox (z.B. Yes, No, Cancel, OK)</returns>
        MessageBoxResult ShowMessageBox(
            string message,
            string title,
            MessageBoxButton buttons,
            MessageBoxImage icon);

        // ????????????????????????????????????????????????????????????
        // Erweiterte Features
        // ????????????????????????????????????????????????????????????

        /// <summary>
        /// Zeigt einen Dialog mit benutzerdefiniertem Ergebnis.
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel-Typ (muss <see cref="IDialogViewModel{TResult}"/> implementieren)</typeparam>
        /// <typeparam name="TResult">Typ des Ergebnisses</typeparam>
        /// <param name="viewModel">ViewModel-Instanz</param>
        /// <param name="owner">Besitzer-Window (optional)</param>
        /// <returns>
        /// <see cref="IDialogViewModel{TResult}.Result"/> wenn <see cref="IDialogViewModel{TResult}.IsConfirmed"/> true ist,
        /// andernfalls <c>default(TResult)</c>
        /// </returns>
        TResult? ShowDialog<TViewModel, TResult>(TViewModel viewModel, Window? owner = null)
            where TViewModel : class, IDialogViewModel<TResult>;

        /// <summary>
        /// Zeigt einen Dialog asynchron (für async/await-Pattern).
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel-Typ</typeparam>
        /// <param name="viewModel">ViewModel-Instanz</param>
        /// <param name="owner">Besitzer-Window (optional)</param>
        /// <returns>Task mit Dialog-Ergebnis</returns>
        /// <remarks>
        /// Diese Methode führt <see cref="ShowDialog{TViewModel}(TViewModel, Window)"/> 
        /// auf einem Hintergrund-Thread aus und gibt ein Task zurück.
        /// </remarks>
        Task<bool?> ShowDialogAsync<TViewModel>(TViewModel viewModel, Window? owner = null)
            where TViewModel : class;

        // ????????????????????????????????????????????????????????????
        // Convention-based Dialogs (ohne ViewModel-Instanz)
        // ????????????????????????????????????????????????????????????

        /// <summary>
        /// Zeigt einen Dialog für das angegebene ViewModel.
        /// Das ViewModel wird automatisch vom DI-Container erstellt.
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel-Typ (muss IDialogViewModelMarker implementieren)</typeparam>
        /// <param name="title">Fenstertitel (optional)</param>
        /// <param name="owner">Besitzer-Window (optional)</param>
        /// <returns>Dialog-Ergebnis (true/false/null)</returns>
        /// <exception cref="InvalidOperationException">
        /// Wenn keine View für den ViewModel-Typ registriert ist oder
        /// das ViewModel nicht im DI-Container registriert ist.
        /// </exception>
        /// <remarks>
        /// <para>
        /// <b>Automatische Erstellung:</b> Das ViewModel wird über den DI-Container erstellt.
        /// Alle Constructor-Dependencies werden automatisch aufgelöst.
        /// </para>
        /// <para>
        /// <b>Voraussetzungen:</b>
        /// </para>
        /// <list type="bullet">
        /// <item>ViewModel muss <see cref="IDialogViewModelMarker"/> implementieren</item>
        /// <item>ViewModel muss via Assembly-Scanning registriert sein</item>
        /// <item>View muss <see cref="IDialogView{TViewModel}"/> implementieren</item>
        /// <item>View muss via Assembly-Scanning registriert sein</item>
        /// </list>
        /// </remarks>
        /// <example>
        /// <code>
        /// // ViewModel mit Dependencies
        /// public class CustomerEditViewModel : IDialogViewModelMarker
        /// {
        ///     public CustomerEditViewModel(ICustomerService service) { }
        /// }
        /// 
        /// // View
        /// public class CustomerEditDialog : Window, IDialogView&lt;CustomerEditViewModel&gt; { }
        /// 
        /// // Verwendung:
        /// var result = dialogService.ShowDialog&lt;CustomerEditViewModel&gt;("Neuer Kunde");
        /// // ViewModel wird automatisch mit ICustomerService erstellt
        /// </code>
        /// </example>
        bool? ShowDialog<TViewModel>(string? title = null, Window? owner = null)
            where TViewModel : class, IDialogViewModelMarker;
    }
}
