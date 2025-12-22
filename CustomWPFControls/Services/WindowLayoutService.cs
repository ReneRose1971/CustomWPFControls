using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DataStores.Abstractions;

namespace CustomWPFControls.Services;

/// <summary>
/// Service zur Persistierung von WPF-Fenster-Positionen und -Größen.
/// Nutzt einen global registrierten DataStore über IDataStores.
/// </summary>
public sealed class WindowLayoutService : IDisposable
{
    private readonly IDataStore<WindowLayoutData> _store;
    private readonly Dictionary<string, Window> _attachedWindows = new();
    private bool _disposed;

    /// <summary>
    /// Erstellt einen WindowLayoutService.
    /// </summary>
    /// <param name="dataStores">IDataStores-Facade zum Abrufen des globalen DataStores.</param>
    /// <exception cref="ArgumentNullException">Wenn dataStores null ist.</exception>
    /// <exception cref="GlobalStoreNotRegisteredException">
    /// Wenn der DataStore für <see cref="WindowLayoutData"/> nicht registriert wurde.
    /// Stellen Sie sicher, dass <see cref="DataStores.Bootstrap.DataStoreBootstrap.RunAsync"/> aufgerufen wurde.
    /// </exception>
    public WindowLayoutService(IDataStores dataStores)
    {
        if (dataStores == null) throw new ArgumentNullException(nameof(dataStores));

        // DataStore über IDataStores abrufen (global registriert)
        _store = dataStores.GetGlobal<WindowLayoutData>();
    }

    /// <summary>
    /// Hängt ein Fenster an den Service an und stellt gespeicherte Position/Größe wieder her.
    /// </summary>
    /// <param name="window">Das zu verwaltende Fenster.</param>
    /// <param name="key">Eindeutiger Schlüssel zur Identifikation.</param>
    /// <exception cref="ArgumentNullException">Wenn window null ist.</exception>
    /// <exception cref="ArgumentException">Wenn key leer ist.</exception>
    /// <exception cref="InvalidOperationException">Wenn bereits ein Fenster mit dem Key angehängt ist.</exception>
    public void Attach(Window window, string key)
    {
        if (window == null) throw new ArgumentNullException(nameof(window));
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Key darf nicht leer sein.", nameof(key));

        if (_attachedWindows.ContainsKey(key))
            throw new InvalidOperationException($"Ein Fenster mit Key '{key}' ist bereits angehängt.");

        _attachedWindows[key] = window;

        // Vorhandene Daten laden
        var layoutData = _store.Items.FirstOrDefault(x => x.WindowKey == key);
        if (layoutData == null)
        {
            // Neue Daten erstellen mit aktueller Fensterposition
            // NaN-Werte durch 0 ersetzen für JSON-Serialisierung
            layoutData = new WindowLayoutData
            {
                WindowKey = key,
                Left = double.IsNaN(window.Left) ? 0 : window.Left,
                Top = double.IsNaN(window.Top) ? 0 : window.Top,
                Width = double.IsNaN(window.Width) ? 0 : window.Width,
                Height = double.IsNaN(window.Height) ? 0 : window.Height,
                WindowState = (int)window.WindowState
            };
            _store.Add(layoutData);
        }
        else
        {
            // Gespeicherte Position/Größe wiederherstellen
            if (layoutData.Width > 0 && layoutData.Height > 0)
            {
                window.Left = layoutData.Left;
                window.Top = layoutData.Top;
                window.Width = layoutData.Width;
                window.Height = layoutData.Height;
                window.WindowState = (WindowState)layoutData.WindowState;
            }
        }

        // Event-Handler registrieren für Live-Updates
        window.LocationChanged += (s, e) => UpdateLayout(key);
        window.SizeChanged += (s, e) => UpdateLayout(key);
        window.StateChanged += (s, e) => UpdateLayout(key);
        window.Closed += (s, e) => Detach(key);
    }

    /// <summary>
    /// Entfernt ein Fenster aus der Verwaltung.
    /// </summary>
    /// <param name="key">Schlüssel des zu entfernenden Fensters.</param>
    public void Detach(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return;
        _attachedWindows.Remove(key);
    }

    private void UpdateLayout(string key)
    {
        if (!_attachedWindows.TryGetValue(key, out var window)) return;

        var layoutData = _store.Items.FirstOrDefault(x => x.WindowKey == key);
        if (layoutData == null) return;

        // Änderungen werden durch PropertyChanged automatisch persistiert (via Fody)
        // NaN-Werte durch 0 ersetzen für JSON-Serialisierung
        layoutData.Left = double.IsNaN(window.Left) ? 0 : window.Left;
        layoutData.Top = double.IsNaN(window.Top) ? 0 : window.Top;
        layoutData.Width = double.IsNaN(window.Width) ? 0 : window.Width;
        layoutData.Height = double.IsNaN(window.Height) ? 0 : window.Height;
        layoutData.WindowState = (int)window.WindowState;
    }

    /// <summary>
    /// Gibt alle Ressourcen frei.
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _attachedWindows.Clear();
    }
}
