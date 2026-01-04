using System;
using System.Linq;
using CustomWPFControls.Factories;
using CustomWPFControls.Services;
using CustomWPFControls.ViewModels;
using DataStores.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using TestHelper.DataStores.Models;

namespace CustomWPFControls.Tests.Testing;

/// <summary>
/// Test-Fixture für CollectionViewModel-Tests mit DataStores-Bootstrap und SUT-Management.
/// </summary>
/// <remarks>
/// <para>
/// Diese Fixture erbt von <see cref="DataStoresFixtureBase"/> und stellt bereit:
/// </para>
/// <list type="bullet">
/// <item><description>ViewModelFactory für TestDto/TestViewModel (via AddViewModelPackage)</description></item>
/// <item><description>ICustomWPFServices Facade (DataStores, ComparerService, DialogService, MessageBoxService)</description></item>
/// <item><description>Zentrale SUT-Instanz (CollectionViewModel) mit Helper-Methoden</description></item>
/// </list>
/// <para>
/// <b>SUT-Management:</b> Die Fixture stellt eine zentrale SUT-Instanz bereit, die via DI aufgelöst wird.
/// Tests können diese verwenden oder eigene Instanzen erstellen (z.B. für Mock-Injection).
/// </para>
/// <para>
/// <b>Wichtig:</b> Jede CollectionViewModel-Instanz hat ihren eigenen lokalen ModelStore.
/// Verwenden Sie <see cref="Sut"/>.ModelStore für Daten-Operationen.
/// </para>
/// </remarks>
public class CollectionViewModelFixture : DataStoresFixtureBase
{
    /// <summary>
    /// ViewModelFactory für TestDto → TestViewModel.
    /// </summary>
    public IViewModelFactory<TestDto, TestViewModel> ViewModelFactory { get; protected set; } = null!;

    /// <summary>
    /// CustomWPFServices Facade mit allen Core-Services.
    /// </summary>
    /// <remarks>
    /// Kapselt: DataStores, ComparerService, DialogService, MessageBoxService
    /// </remarks>
    public ICustomWPFServices Services { get; protected set; } = null!;

    /// <summary>
    /// System Under Test - Zentrale CollectionViewModel-Instanz für Tests.
    /// </summary>
    /// <remarks>
    /// Diese Instanz wird via DI aufgelöst und kann von Tests wiederverwendet werden.
    /// Tests, die eine frische Instanz brauchen, können <see cref="ResetSut"/> aufrufen
    /// oder eigene Instanzen manuell erstellen.
    /// </remarks>
    public CollectionViewModel<TestDto, TestViewModel> Sut { get; private set; } = null!;

    /// <summary>
    /// Der TestDto Data Store der SUT (Adapter für Backward-Compatibility).
    /// </summary>
    /// <remarks>
    /// <para>
    /// Diese Property ist ein Adapter, der auf <see cref="Sut"/>.ModelStore verweist.
    /// Existiert für Backward-Compatibility mit älteren Tests, die _fixture.TestDtoStore verwenden.
    /// </para>
    /// <para>
    /// <b>Empfehlung:</b> Neue Tests sollten direkt <see cref="Sut"/>.ModelStore verwenden,
    /// um klarer zu machen, dass sie mit dem lokalen Store der SUT-Instanz arbeiten.
    /// </para>
    /// </remarks>
    public IDataStore<TestDto> TestDtoStore => Sut.ModelStore;

    /// <summary>
    /// Erstellt die Fixture und führt den kompletten Bootstrap-Prozess aus.
    /// </summary>
    public CollectionViewModelFixture()
    {
        // Basisklasse führt Bootstrap-Prozess aus und ruft dann
        // InitializeServices() und InitializeData() auf
    }

    /// <summary>
    /// Initialisiert die Services nach dem Bootstrap.
    /// </summary>
    /// <remarks>
    /// Löst ViewModelFactory, Services und SUT aus dem ServiceProvider auf.
    /// Alle Services wurden automatisch via Assembly-Scanning registriert.
    /// </remarks>
    protected override void InitializeServices()
    {
        // Core-Services auflösen
        ViewModelFactory = ServiceProvider.GetRequiredService<IViewModelFactory<TestDto, TestViewModel>>();
        Services = ServiceProvider.GetRequiredService<ICustomWPFServices>();
        
        // SUT via DI auflösen (wurde via AddViewModelPackage registriert)
        Sut = ServiceProvider.GetRequiredService<CollectionViewModel<TestDto, TestViewModel>>();
    }

    /// <summary>
    /// Initialisiert die Testdaten nach der Service-Initialisierung.
    /// </summary>
    /// <remarks>
    /// Für CollectionViewModel-Tests bleibt der Store initial leer.
    /// Tests fügen ihre eigenen Daten via Helper-Methoden hinzu.
    /// </remarks>
    protected override void InitializeData()
    {
        // Store bleibt leer - Tests fügen ihre eigenen Daten hinzu
    }

    #region Test Helper Methods

    /// <summary>
    /// Fügt mehrere Testdaten zum ModelStore der SUT hinzu.
    /// </summary>
    /// <param name="names">Namen der zu erstellenden TestDtos.</param>
    /// <returns>Array der hinzugefügten TestDtos.</returns>
    /// <example>
    /// <code>
    /// var dtos = _fixture.AddTestData("First", "Second", "Third");
    /// </code>
    /// </example>
    public TestDto[] AddTestData(params string[] names)
    {
        var dtos = names.Select(n => new TestDto { Name = n }).ToArray();
        Sut.ModelStore.AddRange(dtos);
        return dtos;
    }

    /// <summary>
    /// Fügt ein einzelnes Testdatum zum ModelStore hinzu.
    /// </summary>
    /// <param name="name">Name des TestDto (default: "TestItem").</param>
    /// <returns>Das hinzugefügte TestDto.</returns>
    /// <example>
    /// <code>
    /// var dto = _fixture.AddSingleItem("MyItem");
    /// </code>
    /// </example>
    public TestDto AddSingleItem(string name = "TestItem")
    {
        var dto = new TestDto { Name = name };
        Sut.ModelStore.Add(dto);
        return dto;
    }

    /// <summary>
    /// Räumt den ModelStore der SUT auf (entfernt alle Items).
    /// </summary>
    /// <remarks>
    /// Sollte nach jedem Test aufgerufen werden, um saubere Test-Isolation zu gewährleisten.
    /// Leert auch SelectedItems und setzt SelectedItem zurück.
    /// </remarks>
    public void ClearTestData()
    {
        Sut.ModelStore.Clear();
        Sut.SelectedItems.Clear();
        Sut.SelectedItem = null;
    }

    /// <summary>
    /// Erstellt eine neue SUT-Instanz (für Tests, die eine frische Instanz brauchen).
    /// </summary>
    /// <remarks>
    /// Disposed die alte SUT-Instanz und erstellt eine neue via DI.
    /// Verwendung: Für Tests, die mit komplett frischem State starten müssen.
    /// </remarks>
    public void ResetSut()
    {
        Sut?.Dispose();
        Sut = ServiceProvider.GetRequiredService<CollectionViewModel<TestDto, TestViewModel>>();
    }

    /// <summary>
    /// Erstellt eine neue CollectionViewModel-Instanz (unabhängig von SUT).
    /// </summary>
    /// <returns>Neue CollectionViewModel-Instanz.</returns>
    /// <remarks>
    /// Für Tests, die mehrere CollectionViewModel-Instanzen parallel benötigen.
    /// Die Instanz MUSS vom Test selbst disposed werden!
    /// </remarks>
    public CollectionViewModel<TestDto, TestViewModel> CreateCollectionViewModel()
    {
        return ServiceProvider.GetRequiredService<CollectionViewModel<TestDto, TestViewModel>>();
    }

    /// <summary>
    /// Erstellt eine neue EditableCollectionViewModel-Instanz.
    /// </summary>
    /// <returns>Neue EditableCollectionViewModel-Instanz.</returns>
    /// <remarks>
    /// Die Instanz MUSS vom Test selbst disposed werden!
    /// </remarks>
    public EditableCollectionViewModel<TestDto, TestViewModel> CreateEditableCollectionViewModel()
    {
        return ServiceProvider.GetRequiredService<EditableCollectionViewModel<TestDto, TestViewModel>>();
    }

    #endregion

    /// <summary>
    /// Disposed die Fixture und alle verwalteten Ressourcen.
    /// </summary>
    public override void Dispose()
    {
        Sut?.Dispose();
        base.Dispose();
    }
}
