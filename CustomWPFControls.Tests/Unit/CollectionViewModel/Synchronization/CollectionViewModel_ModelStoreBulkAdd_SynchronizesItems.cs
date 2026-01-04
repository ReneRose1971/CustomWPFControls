using System;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Synchronization;

/// <summary>
/// Szenario: Mehrere Models wurden per AddRange zum ModelStore hinzugefügt.
/// </summary>
/// <remarks>
/// Testet die automatische Synchronisation via TransformTo bei Bulk-Add-Operation.
/// Shared Setup: 3 Models im Store.
/// Alle Tests prüfen verschiedene Aspekte der automatischen ViewModel-Erstellung für alle Models.
/// </remarks>
public sealed class CollectionViewModel_ModelStoreBulkAdd_SynchronizesItems : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;

    public CollectionViewModel_ModelStoreBulkAdd_SynchronizesItems(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();

        // Shared Setup: Mehrere Models per BulkAdd hinzufügen
        var models = new[]
        {
            new TestDto { Name = "Model1" },
            new TestDto { Name = "Model2" },
            new TestDto { Name = "Model3" }
        };
        _fixture.Sut.ModelStore.AddRange(models);
    }

    [Fact]
    public void Count_IsThree()
    {
        // Assert: Count entspricht Anzahl hinzugefügter Models
        _fixture.Sut.Count.Should().Be(3);
    }

    [Fact]
    public void Items_ContainsThreeViewModels()
    {
        // Assert: Für jedes Model wurde ein ViewModel erstellt
        _fixture.Sut.Items.Should().HaveCount(3);
    }

    [Fact]
    public void Items_ContainsViewModelForModel1()
    {
        // Assert: ViewModel für "Model1" existiert
        _fixture.Sut.Items.Should().Contain(vm => vm.Name == "Model1");
    }

    [Fact]
    public void Items_ContainsViewModelForModel2()
    {
        // Assert: ViewModel für "Model2" existiert
        _fixture.Sut.Items.Should().Contain(vm => vm.Name == "Model2");
    }

    [Fact]
    public void Items_ContainsViewModelForModel3()
    {
        // Assert: ViewModel für "Model3" existiert
        _fixture.Sut.Items.Should().Contain(vm => vm.Name == "Model3");
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
