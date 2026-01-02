using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using CustomWPFControls.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CustomWPFControls.Services.Dialogs
{
    /// <summary>
    /// Production-Implementierung des IDialogService.
    /// Zeigt echte WPF-Dialoge, MessageBoxen und Windows an.
    /// Nutzt <see cref="WindowLayoutService"/> für automatische Position- und Größen-Persistierung.
    /// </summary>
    public sealed class DialogService : IDialogService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly WindowLayoutService? _layoutService;
        private readonly Dictionary<Type, Window> _singletonWindows = new();

        /// <summary>
        /// Erstellt eine neue DialogService-Instanz.
        /// </summary>
        /// <param name="serviceProvider">Service-Provider für DI</param>
        /// <param name="layoutService">WindowLayoutService für Position-Persistierung (optional)</param>
        /// <exception cref="ArgumentNullException">Wenn <paramref name="serviceProvider"/> null ist</exception>
        public DialogService(
            IServiceProvider serviceProvider,
            WindowLayoutService? layoutService = null)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _layoutService = layoutService;
        }

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
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            // View-Interface-Typ konstruieren
            var viewInterfaceType = typeof(IDialogView<>).MakeGenericType(typeof(TViewModel));

            // View aus DI-Container holen
            var window = _serviceProvider.GetRequiredService(viewInterfaceType) as Window;

            if (window == null)
            {
                throw new InvalidOperationException(
                    $"Keine View für ViewModel '{typeof(TViewModel).Name}' gefunden. " +
                    $"Stellen Sie sicher, dass eine View IDialogView<{typeof(TViewModel).Name}> implementiert.");
            }

            // DataContext setzen
            window.DataContext = viewModel;

            ConfigureDialog(window, title, width, height, owner);

            // WindowLayoutService-Integration
            AttachLayoutService(window, typeof(TViewModel));

            return window.ShowDialog();
        }

        private void ConfigureDialog(
            Window window,
            string? title,
            double? width,
            double? height,
            Window? owner)
        {
            if (!string.IsNullOrEmpty(title))
                window.Title = title;

            // Größe wird vom LayoutService wiederhergestellt, falls verfügbar
            // Manuelle Größe überschreibt nur, wenn explizit angegeben
            if (width.HasValue && width.Value > 0)
                window.Width = width.Value;

            if (height.HasValue && height.Value > 0)
                window.Height = height.Value;

            if (owner != null)
            {
                window.Owner = owner;
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                // LayoutService stellt Position wieder her, falls vorhanden
                // Andernfalls zentrieren
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            window.ShowInTaskbar = false;
        }

        // ????????????????????????????????????????????????????????????
        // Modeless Windows
        // ????????????????????????????????????????????????????????????

        public Window ShowWindow<TViewModel>(TViewModel viewModel, string? title = null)
            where TViewModel : class
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            // View-Interface-Typ konstruieren
            var viewInterfaceType = typeof(IDialogView<>).MakeGenericType(typeof(TViewModel));

            // View aus DI-Container holen
            var window = _serviceProvider.GetRequiredService(viewInterfaceType) as Window;

            if (window == null)
            {
                throw new InvalidOperationException(
                    $"Keine View für ViewModel '{typeof(TViewModel).Name}' gefunden.");
            }

            // DataContext setzen
            window.DataContext = viewModel;

            if (!string.IsNullOrEmpty(title))
                window.Title = title;

            // WindowLayoutService-Integration
            AttachLayoutService(window, typeof(TViewModel));

            window.Show();

            return window;
        }

        public Window ShowSingletonWindow<TViewModel>(TViewModel viewModel, string? title = null)
            where TViewModel : class
        {
            var viewModelType = typeof(TViewModel);

            if (_singletonWindows.TryGetValue(viewModelType, out var existingWindow))
            {
                if (existingWindow.IsLoaded)
                {
                    existingWindow.Activate();
                    existingWindow.Focus();
                    return existingWindow;
                }
                else
                {
                    _singletonWindows.Remove(viewModelType);
                }
            }

            var window = ShowWindow(viewModel, title);
            window.Closed += (_, _) => _singletonWindows.Remove(viewModelType);

            _singletonWindows[viewModelType] = window;

            return window;
        }

        public void CloseWindow(Window window)
        {
            if (window == null)
                throw new ArgumentNullException(nameof(window));

            window.Close();
        }

        // ????????????????????????????????????????????????????????????
        // WindowLayoutService Integration
        // ????????????????????????????????????????????????????????????

        /// <summary>
        /// Verknüpft ein Window mit dem WindowLayoutService für automatische Persistierung.
        /// </summary>
        /// <param name="window">Das zu verknüpfende Window</param>
        /// <param name="viewModelType">Typ des ViewModels (für Key-Generierung)</param>
        private void AttachLayoutService(Window window, Type viewModelType)
        {
            if (_layoutService == null)
                return;

            // Key aus ViewModel-Typ generieren (z.B. "Dialog_CustomerEditViewModel")
            var layoutKey = $"Dialog_{viewModelType.Name}";

            try
            {
                _layoutService.Attach(window, layoutKey);
            }
            catch
            {
                // Fehler beim Attach ignorieren (z.B. wenn Key bereits verwendet wird)
                // Dialog wird trotzdem angezeigt, nur ohne Persistierung
            }
        }

        // ????????????????????????????????????????????????????????????
        // MessageBoxen
        // ????????????????????????????????????????????????????????????

        public void ShowMessage(string message, string title = "Information")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowWarning(string message, string title = "Warnung")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public void ShowError(string message, string title = "Fehler")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public bool ShowConfirmation(string message, string title = "Bestätigung")
        {
            var result = MessageBox.Show(
                message,
                title,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            return result == MessageBoxResult.Yes;
        }

        public MessageBoxResult ShowMessageBox(
            string message,
            string title,
            MessageBoxButton buttons,
            MessageBoxImage icon)
        {
            return MessageBox.Show(message, title, buttons, icon);
        }

        // ????????????????????????????????????????????????????????????
        // Erweiterte Features
        // ????????????????????????????????????????????????????????????

        public TResult? ShowDialog<TViewModel, TResult>(TViewModel viewModel, Window? owner = null)
            where TViewModel : class, IDialogViewModel<TResult>
        {
            var dialogResult = ShowDialog(viewModel, owner);

            return viewModel.IsConfirmed ? viewModel.Result : default;
        }

        public Task<bool?> ShowDialogAsync<TViewModel>(TViewModel viewModel, Window? owner = null)
            where TViewModel : class
        {
            return Task.Run(() => ShowDialog(viewModel, owner));
        }

        // ????????????????????????????????????????????????????????????
        // Convention-based Dialogs (ohne ViewModel-Instanz)
        // ????????????????????????????????????????????????????????????

        public bool? ShowDialog<TViewModel>(string? title = null, Window? owner = null)
            where TViewModel : class, IDialogViewModelMarker
        {
            // 1. ViewModel aus DI-Container holen
            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();

            // 2. View-Interface-Typ konstruieren
            var viewInterfaceType = typeof(IDialogView<>).MakeGenericType(typeof(TViewModel));

            // 3. View aus DI-Container holen
            var window = _serviceProvider.GetRequiredService(viewInterfaceType) as Window;

            if (window == null)
            {
                throw new InvalidOperationException(
                    $"Keine View für ViewModel '{typeof(TViewModel).Name}' gefunden. " +
                    $"Stellen Sie sicher, dass eine View IDialogView<{typeof(TViewModel).Name}> implementiert " +
                    $"und via Assembly-Scanning registriert wurde.");
            }

            // 4. DataContext setzen
            window.DataContext = viewModel;

            // 5. Titel setzen (falls angegeben)
            if (!string.IsNullOrEmpty(title))
                window.Title = title;

            // 6. Owner setzen
            if (owner != null)
            {
                window.Owner = owner;
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            window.ShowInTaskbar = false;

            // 7. WindowLayoutService-Integration
            AttachLayoutService(window, typeof(TViewModel));

            // 8. Dialog anzeigen
            return window.ShowDialog();
        }
    }
}
