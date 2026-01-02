namespace CustomWPFControls.Services.Dialogs
{
    /// <summary>
    /// Interface für ViewModels mit benutzerdefiniertem Dialog-Ergebnis.
    /// </summary>
    /// <typeparam name="TResult">Typ des Ergebnisses (z.B. ausgewähltes Objekt, Konfiguration)</typeparam>
    /// <remarks>
    /// <para>
    /// Verwenden Sie dieses Interface, wenn Ihr Dialog ein spezifisches Ergebnis zurückgeben soll,
    /// das über das Standard-DialogResult (true/false/null) hinausgeht.
    /// </para>
    /// <para>
    /// <b>Beispiel:</b> Ein DateiauswählDialog könnte <c>IDialogViewModel&lt;FileInfo&gt;</c> implementieren
    /// und die ausgewählte Datei als <see cref="Result"/> zurückgeben.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// public class FilePickerViewModel : IDialogViewModel&lt;FileInfo&gt;
    /// {
    ///     public FileInfo? Result { get; private set; }
    ///     public bool IsConfirmed { get; private set; }
    ///     
    ///     public void SelectFile(FileInfo file)
    ///     {
    ///         Result = file;
    ///         IsConfirmed = true;
    ///         // Dialog schließen
    ///     }
    /// }
    /// 
    /// // Verwendung:
    /// var vm = new FilePickerViewModel();
    /// var file = dialogService.ShowDialog&lt;FilePickerViewModel, FileInfo&gt;(vm);
    /// if (file != null)
    /// {
    ///     // Datei wurde ausgewählt
    /// }
    /// </code>
    /// </example>
    public interface IDialogViewModel<out TResult>
    {
        /// <summary>
        /// Das Ergebnis des Dialogs.
        /// </summary>
        /// <remarks>
        /// Dieses Property wird nur verwendet, wenn <see cref="IsConfirmed"/> <c>true</c> ist.
        /// Andernfalls gibt <see cref="IDialogService.ShowDialog{TViewModel, TResult}"/> <c>default(TResult)</c> zurück.
        /// </remarks>
        TResult? Result { get; }

        /// <summary>
        /// Gibt an, ob der Dialog erfolgreich bestätigt wurde.
        /// </summary>
        /// <remarks>
        /// <c>true</c> wenn der Dialog mit OK/Accept/Auswählen geschlossen wurde,
        /// <c>false</c> wenn der Dialog mit Cancel/Abbrechen geschlossen wurde.
        /// </remarks>
        bool IsConfirmed { get; }
    }
}
