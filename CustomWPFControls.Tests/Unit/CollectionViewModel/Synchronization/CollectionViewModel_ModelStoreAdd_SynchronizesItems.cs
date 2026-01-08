using System;
using System.Linq;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Synchronization;

/// <summary>
/// Szenario: Ein Model wurde zum ModelStore hinzugefügt.
/// </summary>
/// <remarks>
/// Testet die automatische Synchronisation via TransformTo bei Add-Operation.
/// Shared Setup: 1 Model im Store.
/// Alle Tests prüfen verschiedene Aspekte der automatischen ViewModel-Erstellung.
/// </remarks>
public sealed class CollectionViewModel_ModelStoreAdd_SynchronizesItems : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;
    private readonly TestDto _model;

    public CollectionViewModel_ModelStoreAdd_SynchronizesItems(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();

        // Shared Setup: Model zum Store hinzufügen
        _model = new TestDto { Name = "Test1" };
        _fixture.Sut.ModelStore.Add(_model);
    }

    [Fact]
    public void Count_IsOne()
    {
        // Assert: Count wurde aktualisiert
        _fixture.Sut.Count.Should().Be(1);
    }

    [Fact]
    public void Items_ContainsSingleViewModel()
    {
        // Assert: ViewModel wurde automatisch erstellt
        _fixture.Sut.Items.Should().ContainSingle();
    }

    [Fact]
    public void ViewModel_HasCorrectId()
    {
        // Arrange
        var viewModel = _fixture.Sut.Items.First();

        // Assert: ViewModel-ID entspricht Model-ID
        viewModel.Id.Should().Be(_model.Id);
    }

    [Fact]
    public void ViewModel_HasCorrectName()
    {
        // Arrange
        var viewModel = _fixture.Sut.Items.First();

        // Assert: ViewModel-Name entspricht Model-Name
        viewModel.Name.Should().Be("Test1");
    }

    [Fact]
    public void ViewModel_ReferencesCorrectModel()
    {
        // Arrange
        var viewModel = _fixture.Sut.Items.First();

        // Assert: ViewModel referenziert das Original-Model
        viewModel.Model.Should().BeSameAs(_model);
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
