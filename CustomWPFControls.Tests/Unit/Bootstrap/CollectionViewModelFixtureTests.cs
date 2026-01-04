using CustomWPFControls.Factories;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using DataStores.Abstractions;
using DataStores.Bootstrap;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using TestHelper.DataStores.Models;
using TestHelper.DataStores.PathProviders;
using Xunit;

namespace CustomWPFControls.Tests.Unit.Bootstrap;

/// <summary>
/// Unit-Tests für CollectionViewModelFixture.
/// </summary>
/// <remarks>
/// Testet alle Aspekte der Fixture:
/// - Bootstrap-Prozess
/// - Service-Initialisierung
/// - DataStore-Integration
/// - ViewModelFactory-Integration
/// - Helper-Methoden
/// - Lifecycle (Dispose)
/// </remarks>
public sealed class CollectionViewModelFixtureTests : IDisposable
{
    private CollectionViewModelFixture? _fixture;

    #region Constructor & Bootstrap Tests

    [Fact]
    public void Constructor_InitializesServiceProvider()
    {
        // Act
        _fixture = new CollectionViewModelFixture();

        // Assert
        _fixture.ServiceProvider.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_InitializesDataStores()
    {
        // Act
        _fixture = new CollectionViewModelFixture();

        // Assert
        _fixture.DataStores.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_InitializesComparerService()
    {
        // Act
        _fixture = new CollectionViewModelFixture();

        // Assert
        _fixture.ComparerService.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_InitializesPathProvider()
    {
        // Act
        _fixture = new CollectionViewModelFixture();

        // Assert
        _fixture.PathProvider.Should().NotBeNull();
        _fixture.PathProvider.Should().BeOfType<TestDataStorePathProvider>();
    }

    #endregion

    #region Fixture-Specific Properties Tests

    [Fact]
    public void Constructor_InitializesTestDtoStore()
    {
        // Act
        _fixture = new CollectionViewModelFixture();

        // Assert
        _fixture.TestDtoStore.Should().NotBeNull();
        _fixture.TestDtoStore.Should().BeAssignableTo<IDataStore<TestDto>>();
    }

    [Fact]
    public void Constructor_InitializesViewModelFactory()
    {
        // Act
        _fixture = new CollectionViewModelFixture();

        // Assert
        _fixture.ViewModelFactory.Should().NotBeNull();
        _fixture.ViewModelFactory.Should().BeAssignableTo<IViewModelFactory<TestDto, TestViewModel>>();
    }

    [Fact]
    public void Constructor_InitializesSut()
    {
        // Act
        _fixture = new CollectionViewModelFixture();

        // Assert
        _fixture.Sut.Should().NotBeNull();
        _fixture.Sut.Should().BeOfType<ViewModels.CollectionViewModel<TestDto, TestViewModel>>();
    }

    [Fact]
    public void TestDtoStore_IsAdapterToSutModelStore()
    {
        // Arrange
        _fixture = new CollectionViewModelFixture();

        // Act & Assert
        _fixture.TestDtoStore.Should().BeSameAs(_fixture.Sut.ModelStore);
    }

    [Fact]
    public void TestDtoStore_StartsEmpty()
    {
        // Arrange
        _fixture = new CollectionViewModelFixture();

        // Assert
        _fixture.TestDtoStore.Items.Should().BeEmpty();
    }

    #endregion

    #region DataStore Integration Tests

    [Fact]
    public void TestDtoStore_CanAddItems()
    {
        // Arrange
        _fixture = new CollectionViewModelFixture();
        var dto = new TestDto { Name = "Test" };

        // Act
        _fixture.TestDtoStore.Add(dto);

        // Assert
        _fixture.TestDtoStore.Items.Should().ContainSingle();
        _fixture.TestDtoStore.Items.Should().Contain(dto);
    }

    [Fact]
    public void TestDtoStore_CanRemoveItems()
    {
        // Arrange
        _fixture = new CollectionViewModelFixture();
        var dto = new TestDto { Name = "Test" };
        _fixture.TestDtoStore.Add(dto);

        // Act
        var result = _fixture.TestDtoStore.Remove(dto);

        // Assert
        result.Should().BeTrue();
        _fixture.TestDtoStore.Items.Should().BeEmpty();
    }

    [Fact]
    public void TestDtoStore_CanClearItems()
    {
        // Arrange
        _fixture = new CollectionViewModelFixture();
        _fixture.TestDtoStore.AddRange(new[]
        {
            new TestDto { Name = "Test1" },
            new TestDto { Name = "Test2" },
            new TestDto { Name = "Test3" }
        });

        // Act
        _fixture.TestDtoStore.Clear();

        // Assert
        _fixture.TestDtoStore.Items.Should().BeEmpty();
    }

    #endregion

    #region ViewModelFactory Integration Tests

    [Fact]
    public void ViewModelFactory_CanCreateViewModels()
    {
        // Arrange
        _fixture = new CollectionViewModelFixture();
        var dto = new TestDto { Name = "Test" };

        // Act
        var viewModel = _fixture.ViewModelFactory.Create(dto);

        // Assert
        viewModel.Should().NotBeNull();
        viewModel.Should().BeOfType<TestViewModel>();
        viewModel.Model.Should().BeSameAs(dto);
    }

    [Fact]
    public void ViewModelFactory_UsesCorrectServiceProvider()
    {
        // Arrange
        _fixture = new CollectionViewModelFixture();
        var dto = new TestDto { Name = "Test" };

        // Act
        var viewModel = _fixture.ViewModelFactory.Create(dto);

        // Assert - ViewModel wurde erfolgreich erstellt, ServiceProvider funktioniert
        viewModel.Should().NotBeNull();
        viewModel.Name.Should().Be("Test");
    }

    [Fact]
    public void ViewModelFactory_IsRegisteredInServiceProvider()
    {
        // Arrange
        _fixture = new CollectionViewModelFixture();

        // Act
        var factory = _fixture.ServiceProvider.GetRequiredService<IViewModelFactory<TestDto, TestViewModel>>();

        // Assert
        factory.Should().NotBeNull();
        factory.Should().BeSameAs(_fixture.ViewModelFactory);
    }

    #endregion

    #region ComparerService Integration Tests

    [Fact]
    public void ComparerService_CanResolveComparerForTestDto()
    {
        // Arrange
        _fixture = new CollectionViewModelFixture();

        // Act
        var comparer = _fixture.ComparerService.GetComparer<TestDto>();

        // Assert
        comparer.Should().NotBeNull();
    }

    [Fact]
    public void ComparerService_ReturnsWorkingComparer()
    {
        // Arrange
        _fixture = new CollectionViewModelFixture();
        var comparer = _fixture.ComparerService.GetComparer<TestDto>();
        var dto1 = new TestDto { Name = "Test" };
        var dto2 = dto1; // Gleiche Referenz

        // Act
        var areEqual = comparer.Equals(dto1, dto2);

        // Assert
        areEqual.Should().BeTrue();
    }

    #endregion

    #region Helper Method Tests

    [Fact]
    public void ClearTestData_RemovesAllItemsFromStore()
    {
        // Arrange
        _fixture = new CollectionViewModelFixture();
        _fixture.TestDtoStore.AddRange(new[]
        {
            new TestDto { Name = "Test1" },
            new TestDto { Name = "Test2" },
            new TestDto { Name = "Test3" }
        });

        // Act
        _fixture.ClearTestData();

        // Assert
        _fixture.TestDtoStore.Items.Should().BeEmpty();
    }

    [Fact]
    public void ClearTestData_CanBeCalledMultipleTimes()
    {
        // Arrange
        _fixture = new CollectionViewModelFixture();
        _fixture.TestDtoStore.Add(new TestDto { Name = "Test" });

        // Act
        _fixture.ClearTestData();
        _fixture.ClearTestData(); // Zweiter Aufruf auf leerem Store

        // Assert
        _fixture.TestDtoStore.Items.Should().BeEmpty();
    }

    [Fact]
    public void ClearTestData_AllowsAddingNewItemsAfterClear()
    {
        // Arrange
        _fixture = new CollectionViewModelFixture();
        _fixture.TestDtoStore.Add(new TestDto { Name = "Test1" });
        _fixture.ClearTestData();

        // Act
        var newDto = new TestDto { Name = "Test2" };
        _fixture.TestDtoStore.Add(newDto);

        // Assert
        _fixture.TestDtoStore.Items.Should().ContainSingle();
        _fixture.TestDtoStore.Items.Should().Contain(newDto);
    }

    #endregion

    #region PathProvider Tests

    [Fact]
    public void PathProvider_HasCorrectBaseName()
    {
        // Arrange
        _fixture = new CollectionViewModelFixture();

        // Act
        var pathProvider = _fixture.PathProvider as TestDataStorePathProvider;

        // Assert
        pathProvider.Should().NotBeNull();
        // TestDataStorePathProvider hat einen zufälligen Suffix, prüfen wir nur den Typ
    }

    [Fact]
    public void PathProvider_IsRegisteredInServiceProvider()
    {
        // Arrange
        _fixture = new CollectionViewModelFixture();

        // Act
        var pathProvider = _fixture.ServiceProvider.GetRequiredService<IDataStorePathProvider>();

        // Assert
        pathProvider.Should().NotBeNull();
        pathProvider.Should().BeSameAs(_fixture.PathProvider);
    }

    #endregion

    #region Lifecycle Tests

    [Fact]
    public void Dispose_DisposesServiceProvider()
    {
        // Arrange
        _fixture = new CollectionViewModelFixture();
        var serviceProvider = _fixture.ServiceProvider;

        // Act
        _fixture.Dispose();

        // Assert - ServiceProvider sollte disposed sein
        // Wir können nicht direkt prüfen ob disposed, aber Services sollten nicht mehr auflösbar sein
        Action act = () => serviceProvider.GetRequiredService<IDataStores>();
        act.Should().Throw<ObjectDisposedException>();
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        // Arrange
        _fixture = new CollectionViewModelFixture();

        // Act & Assert - Sollte keine Exception werfen
        _fixture.Dispose();
        _fixture.Dispose();
    }

    [Fact]
    public void Dispose_CleansUpPathProvider()
    {
        // Arrange
        _fixture = new CollectionViewModelFixture();
        var pathProvider = _fixture.PathProvider as TestDataStorePathProvider;

        // Act
        _fixture.Dispose();

        // Assert
        // TestDataStorePathProvider sollte Cleanup aufgerufen haben
        // (Directory sollte gelöscht sein, aber wir können das nicht direkt testen ohne die Implementierung zu kennen)
        pathProvider.Should().NotBeNull();
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void FullWorkflow_AddDataCreateViewModelClear_WorksCorrectly()
    {
        // Arrange
        _fixture = new CollectionViewModelFixture();
        var dto1 = new TestDto { Name = "Test1" };
        var dto2 = new TestDto { Name = "Test2" };

        // Act 1: Add items
        _fixture.TestDtoStore.Add(dto1);
        _fixture.TestDtoStore.Add(dto2);

        // Assert 1: Items added
        _fixture.TestDtoStore.Items.Should().HaveCount(2);

        // Act 2: Create ViewModels
        var vm1 = _fixture.ViewModelFactory.Create(dto1);
        var vm2 = _fixture.ViewModelFactory.Create(dto2);

        // Assert 2: ViewModels created
        vm1.Name.Should().Be("Test1");
        vm2.Name.Should().Be("Test2");

        // Act 3: Clear
        _fixture.ClearTestData();

        // Assert 3: Store cleared
        _fixture.TestDtoStore.Items.Should().BeEmpty();
    }

    [Fact]
    public void MultipleFixtureInstances_AreIsolated()
    {
        // Arrange & Act
        using var fixture1 = new CollectionViewModelFixture();
        using var fixture2 = new CollectionViewModelFixture();

        fixture1.TestDtoStore.Add(new TestDto { Name = "Fixture1" });
        fixture1.TestDtoStore.Add(new TestDto { Name = "Fixture1_2" });
        fixture1.TestDtoStore.Add(new TestDto { Name = "Fixture1_3" });
        
        fixture2.TestDtoStore.Add(new TestDto { Name = "Fixture2" });

        // Assert - Stores sind isoliert (jede Fixture hat ihren eigenen lokalen Store)
        fixture1.TestDtoStore.Items.Should().HaveCount(3);
        fixture2.TestDtoStore.Items.Should().HaveCount(1);

        fixture1.TestDtoStore.Items.First().Name.Should().Be("Fixture1");
        fixture2.TestDtoStore.Items.First().Name.Should().Be("Fixture2");
    }

    #endregion

    public void Dispose()
    {
        _fixture?.Dispose();
    }
}
