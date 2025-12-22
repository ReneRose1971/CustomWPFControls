using System;
using System.IO;
using System.Text.Json;
using DataStores.Abstractions;
using DataStores.Persistence;
using CustomWPFControls.Services;

namespace CustomWPFControls.Bootstrap;

/// <summary>
/// Registrar für den WindowLayoutData DataStore.
/// Registriert einen globalen JSON-persistenten DataStore für WindowLayoutData.
/// </summary>
/// <remarks>
/// Dieser Registrar wird automatisch durch <see cref="DataStores.Bootstrap.DataStoreBootstrap"/> 
/// gefunden und ausgeführt, wenn er im DI-Container registriert ist.
/// </remarks>
public sealed class WindowLayoutDataStoreRegistrar : IDataStoreRegistrar
{
    private readonly string? _customJsonPath;
    private readonly JsonSerializerOptions? _jsonOptions;

    /// <summary>
    /// Erstellt einen neuen WindowLayoutDataStoreRegistrar mit Standard-Einstellungen.
    /// </summary>
    /// <remarks>
    /// Der DataStore wird in "MyDocuments\CustomWPFControls\windowlayouts.json" persistiert.
    /// </remarks>
    public WindowLayoutDataStoreRegistrar()
    {
    }

    /// <summary>
    /// Erstellt einen neuen WindowLayoutDataStoreRegistrar mit benutzerdefiniertem JSON-Pfad.
    /// </summary>
    /// <param name="customJsonPath">Der vollständige Pfad zur JSON-Datei.</param>
    /// <param name="jsonOptions">Optionale JSON-Serialisierungsoptionen.</param>
    public WindowLayoutDataStoreRegistrar(string customJsonPath, JsonSerializerOptions? jsonOptions = null)
    {
        _customJsonPath = customJsonPath ?? throw new ArgumentNullException(nameof(customJsonPath));
        _jsonOptions = jsonOptions;
    }

    /// <summary>
    /// Registriert den globalen DataStore für WindowLayoutData.
    /// </summary>
    /// <param name="registry">Die globale DataStore-Registry.</param>
    /// <param name="serviceProvider">Der Service Provider für Dependency Resolution.</param>
    public void Register(IGlobalStoreRegistry registry, IServiceProvider serviceProvider)
    {
        var jsonPath = _customJsonPath ?? GetDefaultJsonPath();
        var options = _jsonOptions ?? new JsonSerializerOptions { WriteIndented = true };

        registry.RegisterGlobalWithJsonFile<WindowLayoutData>(
            jsonPath,
            jsonOptions: options,
            autoLoad: true,
            autoSave: true);
    }

    /// <summary>
    /// Liefert den Standard-Pfad für die JSON-Datei.
    /// </summary>
    /// <returns>Der vollständige Pfad zur JSON-Datei im MyDocuments-Ordner.</returns>
    private static string GetDefaultJsonPath()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "CustomWPFControls",
            "windowlayouts.json");
    }
}
