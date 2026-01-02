using System.Windows;

namespace CustomWPFControls.Services.MessageBoxes
{
    /// <summary>
    /// Service für testbare MessageBox-Anzeige.
    /// Ermöglicht Mocking in Unit-Tests.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Verwendung in Production:</b> Nutzt WPF MessageBox.Show()
    /// </para>
    /// <para>
    /// <b>Verwendung in Tests:</b> Nutzt MockMessageBoxService für Verification
    /// </para>
    /// </remarks>
    public interface IMessageBoxService
    {
        // ????????????????????????????????????????????????????????????
        // Einfache Shortcuts (häufigste Use-Cases)
        // ????????????????????????????????????????????????????????????

        /// <summary>
        /// Zeigt eine Informationsmeldung an.
        /// </summary>
        /// <param name="message">Anzuzeigende Nachricht</param>
        /// <param name="title">Fenstertitel (Standard: "Information")</param>
        void ShowMessage(string message, string title = "Information");

        /// <summary>
        /// Zeigt eine Warnmeldung an.
        /// </summary>
        /// <param name="message">Anzuzeigende Warnung</param>
        /// <param name="title">Fenstertitel (Standard: "Warnung")</param>
        void ShowWarning(string message, string title = "Warnung");

        /// <summary>
        /// Zeigt eine Fehlermeldung an.
        /// </summary>
        /// <param name="message">Anzuzeigende Fehlermeldung</param>
        /// <param name="title">Fenstertitel (Standard: "Fehler")</param>
        void ShowError(string message, string title = "Fehler");

        /// <summary>
        /// Zeigt eine Bestätigungsfrage (Ja/Nein) an.
        /// </summary>
        /// <param name="message">Anzuzeigende Frage</param>
        /// <param name="title">Fenstertitel (Standard: "Bestätigung")</param>
        /// <returns>true wenn Ja geklickt wurde, false wenn Nein</returns>
        bool ShowConfirmation(string message, string title = "Bestätigung");

        // ????????????????????????????????????????????????????????????
        // Erweiterte Methode (volle Kontrolle)
        // ????????????????????????????????????????????????????????????

        /// <summary>
        /// Zeigt eine MessageBox mit vollständiger Kontrolle über Buttons und Icon an.
        /// </summary>
        /// <param name="message">Anzuzeigende Nachricht</param>
        /// <param name="title">Fenstertitel</param>
        /// <param name="buttons">Anzuzeigende Buttons (OK, OKCancel, YesNo, YesNoCancel)</param>
        /// <param name="icon">Anzuzeigendes Icon (None, Information, Warning, Error, Question)</param>
        /// <returns>MessageBoxResult mit geklicktem Button</returns>
        MessageBoxResult ShowMessageBox(
            string message,
            string title,
            MessageBoxButton buttons = MessageBoxButton.OK,
            MessageBoxImage icon = MessageBoxImage.None);

        // ????????????????????????????????????????????????????????????
        // Ask-Patterns (explizite semantische Fragen)
        // ????????????????????????????????????????????????????????????

        /// <summary>
        /// Stellt eine Ja/Nein-Frage.
        /// </summary>
        /// <param name="question">Anzuzeigende Frage</param>
        /// <param name="title">Fenstertitel (Standard: "Frage")</param>
        /// <returns>true wenn Ja geklickt wurde, false wenn Nein</returns>
        bool AskYesNo(string question, string title = "Frage");

        /// <summary>
        /// Stellt eine Ja/Nein/Abbrechen-Frage.
        /// </summary>
        /// <param name="question">Anzuzeigende Frage</param>
        /// <param name="title">Fenstertitel (Standard: "Frage")</param>
        /// <returns>true wenn Ja, false wenn Nein, null wenn Abbrechen</returns>
        bool? AskYesNoCancel(string question, string title = "Frage");

        /// <summary>
        /// Stellt eine OK/Abbrechen-Frage.
        /// </summary>
        /// <param name="message">Anzuzeigende Nachricht</param>
        /// <param name="title">Fenstertitel (Standard: "Bestätigung")</param>
        /// <returns>true wenn OK geklickt wurde, false wenn Abbrechen</returns>
        bool AskOkCancel(string message, string title = "Bestätigung");
    }
}
