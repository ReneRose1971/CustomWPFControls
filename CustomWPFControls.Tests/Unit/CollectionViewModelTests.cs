using System;
using System.Linq;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit;

/// <summary>
/// Unit-Tests für CollectionViewModel mit echtem DataStores-Bootstrap.
/// </summary>
/// <remarks>
/// Diese Tests verwenden die CollectionViewModelFixture, die den minimalen
/// Bootstrap-Prozess für DataStores abbildet.
/// TestDto-Store wird automatisch registriert via CustomWPFControlsTestDataStoreRegistrar.
/// </remarks>
public sealed class CollectionViewModelTests : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly CollectionViewModel<TestDto, TestViewModel> _sut;

    public CollectionViewModelTests(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;

        // CollectionViewModel erstellen mit echten Dependencies aus Bootstrap
        _sut = new CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);
    }

    [Fact]
    public void Constructor_Should_CreateEmptyCollection()
    {
        // Assert
        Assert.NotNull(_sut.Items);
        Assert.Equal(0, _sut.Count);
        Assert.Empty(_sut.Items);
    }

    [Fact]
    public void Items_Should_SyncWithModelStore_OnAdd()
    {
        // Arrange: Model zum Store hinzufügen
        var model = new TestDto { Name = "Test1" };

        // Act: Model zum globalen Store hinzufügen
        _fixture.TestDtoStore.Add(model);

        // Assert: ViewModel wurde automatisch erstellt via TransformTo
        Assert.Equal(1, _sut.Count);
        Assert.Single(_sut.Items);

        var viewModel = _sut.Items.First();
        Assert.Equal(model.Id, viewModel.Id);
        Assert.Equal("Test1", viewModel.Name);
        Assert.Same(model, viewModel.Model);
    }

    [Fact]
    public void Items_Should_SyncWithModelStore_OnRemove()
    {
        // Arrange: Model hinzufügen
        var model = new TestDto { Name = "Test1" };
        _fixture.TestDtoStore.Add(model);

        // Assert: ViewModel wurde erstellt
        Assert.Equal(1, _sut.Count);

        // Act: Model aus Store entfernen
        _fixture.TestDtoStore.Remove(model);

        // Assert: ViewModel wurde automatisch entfernt
        Assert.Equal(0, _sut.Count);
        Assert.Empty(_sut.Items);
    }

    [Fact]
    public void Items_Should_SyncWithModelStore_OnBulkAdd()
    {
        // Arrange: Mehrere Models erstellen
        var models = new[]
        {
            new TestDto { Name = "Model1" },
            new TestDto { Name = "Model2" },
            new TestDto { Name = "Model3" }
        };

        // Act: Bulk-Add zum Store
        _fixture.TestDtoStore.AddRange(models);

        // Assert: Alle ViewModels wurden erstellt
        Assert.Equal(3, _sut.Count);
        Assert.Equal(3, _sut.Items.Count);

        // Namen prüfen (IDs sind Guids, keine feste Reihenfolge)
        Assert.Contains(_sut.Items, vm => vm.Name == "Model1");
        Assert.Contains(_sut.Items, vm => vm.Name == "Model2");
        Assert.Contains(_sut.Items, vm => vm.Name == "Model3");
    }

    [Fact]
    public void Items_Should_SyncWithModelStore_OnClear()
    {
        // Arrange: Models hinzufügen
        var models = new[]
        {
            new TestDto { Name = "Model1" },
            new TestDto { Name = "Model2" }
        };
        _fixture.TestDtoStore.AddRange(models);

        // Assert: ViewModels wurden erstellt
        Assert.Equal(2, _sut.Count);

        // Act: Store leeren
        _fixture.TestDtoStore.Clear();

        // Assert: Alle ViewModels wurden entfernt
        Assert.Equal(0, _sut.Count);
        Assert.Empty(_sut.Items);
    }

    [Fact]
    public void Remove_Should_RemoveViewModelAndModel()
    {
        // Arrange: Model hinzufügen
        var model = new TestDto { Name = "Test1" };
        _fixture.TestDtoStore.Add(model);
        var viewModel = _sut.Items.First();

        // Act: ViewModel über CollectionViewModel entfernen
        var result = _sut.Remove(viewModel);

        // Assert: ViewModel und Model wurden entfernt
        Assert.True(result);
        Assert.Equal(0, _sut.Count);
        Assert.Empty(_sut.Items);
        Assert.Empty(_fixture.TestDtoStore.Items);
    }

    [Fact]
    public void RemoveRange_Should_RemoveMultipleViewModelsAndModels()
    {
        // Arrange: Models hinzufügen
        var models = new[]
        {
            new TestDto { Name = "Model1" },
            new TestDto { Name = "Model2" },
            new TestDto { Name = "Model3" }
        };
        _fixture.TestDtoStore.AddRange(models);

        // ViewModels für Entfernung sammeln
        var viewModelsToRemove = _sut.Items.Take(2).ToList();

        // Act: Mehrere ViewModels entfernen
        var removedCount = _sut.RemoveRange(viewModelsToRemove);

        // Assert: Zwei ViewModels/Models wurden entfernt
        Assert.Equal(2, removedCount);
        Assert.Equal(1, _sut.Count);
        Assert.Single(_sut.Items);
    }

    [Fact]
    public void Clear_Should_RemoveAllViewModelsAndModels()
    {
        // Arrange: Models hinzufügen
        var models = new[]
        {
            new TestDto { Name = "Model1" },
            new TestDto { Name = "Model2" }
        };
        _fixture.TestDtoStore.AddRange(models);

        // Act: Alle entfernen
        _sut.Clear();

        // Assert: Alle ViewModels und Models wurden entfernt
        Assert.Equal(0, _sut.Count);
        Assert.Empty(_sut.Items);
        Assert.Empty(_fixture.TestDtoStore.Items);
    }

    [Fact]
    public void Contains_Should_ReturnTrueForExistingViewModel()
    {
        // Arrange: Model hinzufügen
        var model = new TestDto { Name = "Test1" };
        _fixture.TestDtoStore.Add(model);
        var viewModel = _sut.Items.First();

        // Act & Assert
        Assert.True(_sut.Contains(viewModel));
    }

    [Fact]
    public void Contains_Should_ReturnFalseForNonExistingViewModel()
    {
        // Arrange: Ein anderes ViewModel erstellen
        var otherModel = new TestDto { Name = "Other" };
        var otherViewModel = new TestViewModel(otherModel);

        // Act & Assert
        Assert.False(_sut.Contains(otherViewModel));
    }

    public void Dispose()
    {
        // Cleanup: Store nach jedem Test leeren für Test-Isolation
        _fixture.ClearTestData();

        // CollectionViewModel disposed
        _sut?.Dispose();
    }
}
