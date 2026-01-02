using System.ComponentModel;
using PropertyChanged;
using DataStores.Abstractions;

namespace CustomWPFControls.Services;

/// <summary>
/// Speichert Position und Größe eines WPF-Fensters zur JSON-Persistierung.
/// Verwendet Fody.PropertyChanged für automatische INotifyPropertyChanged-Implementierung.
/// </summary>
[AddINotifyPropertyChangedInterface]
public sealed class WindowLayoutData : EntityBase
{
    /// <summary>
    /// Eindeutiger Schlüssel zur Identifikation des Fensters.
    /// </summary>
    public string WindowKey { get; set; } = string.Empty;

    /// <summary>
    /// Linke Position des Fensters.
    /// </summary>
    public double Left { get; set; }

    /// <summary>
    /// Obere Position des Fensters.
    /// </summary>
    public double Top { get; set; }

    /// <summary>
    /// Breite des Fensters.
    /// </summary>
    public double Width { get; set; }

    /// <summary>
    /// Höhe des Fensters.
    /// </summary>
    public double Height { get; set; }

    /// <summary>
    /// Fensterstatus (0=Normal, 1=Minimized, 2=Maximized).
    /// </summary>
    public int WindowState { get; set; }

    public override string ToString()
    {
        return $"WindowLayout #{Id}: {WindowKey} ({Left},{Top},{Width}x{Height})";
    }

    public override bool Equals(object? obj)
    {
        if (obj is not WindowLayoutData other) return false;
        if (Id > 0 && other.Id > 0) return Id == other.Id;
        return WindowKey == other.WindowKey;
    }

    public override int GetHashCode()
    {
        return Id > 0 ? Id : HashCode.Combine(WindowKey);
    }
}
