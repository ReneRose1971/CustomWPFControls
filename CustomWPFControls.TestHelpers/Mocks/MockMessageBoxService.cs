using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CustomWPFControls.Services.MessageBoxes;

namespace CustomWPFControls.TestHelpers.Mocks
{
    /// <summary>
    /// Mock-Implementierung des IMessageBoxService für Unit-Tests.
    /// Tracked alle MessageBox-Aufrufe und ermöglicht konfigurierbare Antworten.
    /// </summary>
    public sealed class MockMessageBoxService : IMessageBoxService
    {
        private readonly List<MessageBoxCall> _calls = new();

        // ????????????????????????????????????????????????????????????
        // Konfiguration für Tests (Next-Result-Pattern)
        // ????????????????????????????????????????????????????????????

        /// <summary>
        /// Konfiguriert das Ergebnis für den nächsten ShowMessageBox-Aufruf.
        /// </summary>
        public MessageBoxResult NextResult { get; set; } = MessageBoxResult.OK;

        /// <summary>
        /// Konfiguriert das Ergebnis für den nächsten ShowConfirmation / AskYesNo-Aufruf.
        /// </summary>
        public bool NextYesNoResult { get; set; } = true;

        /// <summary>
        /// Konfiguriert das Ergebnis für den nächsten AskYesNoCancel-Aufruf.
        /// </summary>
        public bool? NextYesNoCancelResult { get; set; } = true;

        /// <summary>
        /// Konfiguriert das Ergebnis für den nächsten AskOkCancel-Aufruf.
        /// </summary>
        public bool NextOkCancelResult { get; set; } = true;

        // ????????????????????????????????????????????????????????????
        // Tracking aller Aufrufe
        // ????????????????????????????????????????????????????????????

        /// <summary>
        /// Liste aller MessageBox-Aufrufe.
        /// </summary>
        public IReadOnlyList<MessageBoxCall> Calls => _calls.AsReadOnly();

        // ????????????????????????????????????????????????????????????
        // IMessageBoxService Implementation
        // ????????????????????????????????????????????????????????????

        public void ShowMessage(string message, string title = "Information")
        {
            _calls.Add(new MessageBoxCall
            {
                Type = MessageBoxType.Information,
                Message = message,
                Title = title,
                Buttons = MessageBoxButton.OK,
                Icon = MessageBoxImage.Information
            });
        }

        public void ShowWarning(string message, string title = "Warnung")
        {
            _calls.Add(new MessageBoxCall
            {
                Type = MessageBoxType.Warning,
                Message = message,
                Title = title,
                Buttons = MessageBoxButton.OK,
                Icon = MessageBoxImage.Warning
            });
        }

        public void ShowError(string message, string title = "Fehler")
        {
            _calls.Add(new MessageBoxCall
            {
                Type = MessageBoxType.Error,
                Message = message,
                Title = title,
                Buttons = MessageBoxButton.OK,
                Icon = MessageBoxImage.Error
            });
        }

        public bool ShowConfirmation(string message, string title = "Bestätigung")
        {
            _calls.Add(new MessageBoxCall
            {
                Type = MessageBoxType.Question,
                Message = message,
                Title = title,
                Buttons = MessageBoxButton.YesNo,
                Icon = MessageBoxImage.Question
            });

            return NextYesNoResult;
        }

        public MessageBoxResult ShowMessageBox(
            string message,
            string title,
            MessageBoxButton buttons = MessageBoxButton.OK,
            MessageBoxImage icon = MessageBoxImage.None)
        {
            _calls.Add(new MessageBoxCall
            {
                Type = GetMessageBoxType(icon),
                Message = message,
                Title = title,
                Buttons = buttons,
                Icon = icon
            });

            return NextResult;
        }

        public bool AskYesNo(string question, string title = "Frage")
        {
            _calls.Add(new MessageBoxCall
            {
                Type = MessageBoxType.Question,
                Message = question,
                Title = title,
                Buttons = MessageBoxButton.YesNo,
                Icon = MessageBoxImage.Question
            });

            return NextYesNoResult;
        }

        public bool? AskYesNoCancel(string question, string title = "Frage")
        {
            _calls.Add(new MessageBoxCall
            {
                Type = MessageBoxType.Question,
                Message = question,
                Title = title,
                Buttons = MessageBoxButton.YesNoCancel,
                Icon = MessageBoxImage.Question
            });

            return NextYesNoCancelResult;
        }

        public bool AskOkCancel(string message, string title = "Bestätigung")
        {
            _calls.Add(new MessageBoxCall
            {
                Type = MessageBoxType.Question,
                Message = message,
                Title = title,
                Buttons = MessageBoxButton.OKCancel,
                Icon = MessageBoxImage.Question
            });

            return NextOkCancelResult;
        }

        // ????????????????????????????????????????????????????????????
        // Verification Helpers
        // ????????????????????????????????????????????????????????????

        /// <summary>
        /// Prüft, ob eine Nachricht mit dem erwarteten Text angezeigt wurde.
        /// </summary>
        public void VerifyMessageShown(string expectedMessage)
        {
            if (!Calls.Any(c => c.Message.Contains(expectedMessage)))
            {
                throw new InvalidOperationException(
                    $"Erwartete Nachricht '{expectedMessage}' wurde nicht angezeigt.");
            }
        }

        /// <summary>
        /// Prüft, ob eine Warnung mit dem erwarteten Text angezeigt wurde.
        /// </summary>
        public void VerifyWarningShown(string expectedMessage)
        {
            if (!Calls.Any(c => c.Type == MessageBoxType.Warning && c.Message.Contains(expectedMessage)))
            {
                throw new InvalidOperationException(
                    $"Erwartete Warnung '{expectedMessage}' wurde nicht angezeigt.");
            }
        }

        /// <summary>
        /// Prüft, ob ein Fehler mit dem erwarteten Text angezeigt wurde.
        /// </summary>
        public void VerifyErrorShown(string expectedMessage)
        {
            if (!Calls.Any(c => c.Type == MessageBoxType.Error && c.Message.Contains(expectedMessage)))
            {
                throw new InvalidOperationException(
                    $"Erwarteter Fehler '{expectedMessage}' wurde nicht angezeigt.");
            }
        }

        /// <summary>
        /// Prüft, ob eine Bestätigungsfrage mit dem erwarteten Text angezeigt wurde.
        /// </summary>
        public void VerifyConfirmationShown(string expectedQuestion)
        {
            if (!Calls.Any(c => c.Type == MessageBoxType.Question && c.Message.Contains(expectedQuestion)))
            {
                throw new InvalidOperationException(
                    $"Erwartete Bestätigung '{expectedQuestion}' wurde nicht angezeigt.");
            }
        }

        /// <summary>
        /// Prüft, ob genau N MessageBoxen angezeigt wurden.
        /// </summary>
        public void VerifyCallCount(int expectedCount)
        {
            if (Calls.Count != expectedCount)
            {
                throw new InvalidOperationException(
                    $"Erwartete {expectedCount} MessageBox-Aufrufe, aber {Calls.Count} wurden angezeigt.");
            }
        }

        /// <summary>
        /// Setzt den Mock zurück (löscht alle Aufrufe und setzt Defaults).
        /// </summary>
        public void Reset()
        {
            _calls.Clear();
            NextResult = MessageBoxResult.OK;
            NextYesNoResult = true;
            NextYesNoCancelResult = true;
            NextOkCancelResult = true;
        }

        // ????????????????????????????????????????????????????????????
        // Private Helpers
        // ????????????????????????????????????????????????????????????

        private static MessageBoxType GetMessageBoxType(MessageBoxImage icon)
        {
            return icon switch
            {
                MessageBoxImage.Information => MessageBoxType.Information,
                MessageBoxImage.Warning => MessageBoxType.Warning,
                MessageBoxImage.Error => MessageBoxType.Error,
                MessageBoxImage.Question => MessageBoxType.Question,
                _ => MessageBoxType.Information
            };
        }
    }

    // ????????????????????????????????????????????????????????????
    // Supporting Types
    // ????????????????????????????????????????????????????????????

    /// <summary>
    /// Repräsentiert einen MessageBox-Aufruf für Tracking.
    /// </summary>
    public sealed class MessageBoxCall
    {
        public MessageBoxType Type { get; init; }
        public string Message { get; init; } = "";
        public string Title { get; init; } = "";
        public MessageBoxButton Buttons { get; init; }
        public MessageBoxImage Icon { get; init; }
    }

    /// <summary>
    /// Typ der MessageBox (vereinfacht für Tests).
    /// </summary>
    public enum MessageBoxType
    {
        Information,
        Warning,
        Error,
        Question
    }
}
