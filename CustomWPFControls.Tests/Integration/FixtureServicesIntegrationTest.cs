using System;
using CustomWPFControls.Factories;
using CustomWPFControls.Services;
using CustomWPFControls.Tests.Testing;
using DataStores.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Integration;

/// <summary>
/// Integrationstest zur Validierung der TestHelperCustomWPFControlsTestFixture-Services.
/// Prüft ob alle Services korrekt aufgelöst werden und der ServiceProvider verfügbar ist.
/// </summary>
public sealed class FixtureServicesIntegrationTest : IClassFixture<TestHelperCustomWPFControlsTestFixture>
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;

    public FixtureServicesIntegrationTest(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
    }

    [Fact]
    public void ServiceProvider_IsNotNull()
    {
        _fixture.ServiceProvider.Should().NotBeNull("ServiceProvider muss verfügbar sein");
    }

    [Fact]
    public void ServiceProvider_IsNotDisposed()
    {
        // Versuche einen Service aufzulösen - sollte nicht werfen
        var act = () => _fixture.ServiceProvider.GetRequiredService<IDataStores>();
        act.Should().NotThrow<ObjectDisposedException>("ServiceProvider sollte nicht disposed sein");
    }

    [Fact]
    public void DataStores_IsNotNull()
    {
        _fixture.DataStores.Should().NotBeNull("DataStores Facade muss verfügbar sein");
    }

    [Fact]
    public void TestDtoStore_IsNotNull()
    {
        _fixture.TestDtoStore.Should().NotBeNull("TestDtoStore muss verfügbar sein");
    }

    [Fact]
    public void ViewModelFactory_IsNotNull()
    {
        _fixture.ViewModelFactory.Should().NotBeNull("ViewModelFactory muss verfügbar sein");
    }

    [Fact]
    public void Services_IsNotNull()
    {
        _fixture.Services.Should().NotBeNull("ICustomWPFServices muss verfügbar sein");
    }

    [Fact]
    public void Services_DataStores_IsNotNull()
    {
        _fixture.Services.DataStores.Should().NotBeNull("Services.DataStores muss verfügbar sein");
    }

    [Fact]
    public void Services_ComparerService_IsNotNull()
    {
        _fixture.Services.ComparerService.Should().NotBeNull("Services.ComparerService muss verfügbar sein");
    }

    [Fact]
    public void Services_DialogService_IsNotNull()
    {
        _fixture.Services.DialogService.Should().NotBeNull("Services.DialogService muss verfügbar sein");
    }

    [Fact]
    public void Services_MessageBoxService_IsNotNull()
    {
        _fixture.Services.MessageBoxService.Should().NotBeNull("Services.MessageBoxService muss verfügbar sein");
    }

    [Fact]
    public void ComparerService_GetComparer_ForTestDto_DoesNotThrow()
    {
        var act = () => _fixture.Services.ComparerService.GetComparer<TestDto>();
        act.Should().NotThrow("GetComparer<TestDto> sollte nicht werfen");
    }

    [Fact]
    public void ComparerService_GetComparer_ForTestDto_ReturnsComparer()
    {
        var comparer = _fixture.Services.ComparerService.GetComparer<TestDto>();
        comparer.Should().NotBeNull("GetComparer sollte einen Comparer zurückgeben");
    }

    [Fact]
    public void ComparerService_GetComparer_ForTestViewModel_DoesNotThrow()
    {
        var act = () => _fixture.Services.ComparerService.GetComparer<TestViewModel>();
        act.Should().NotThrow("GetComparer<TestViewModel> sollte nicht werfen");
    }

    [Fact]
    public void ComparerService_GetComparer_ForTestViewModel_ReturnsComparer()
    {
        var comparer = _fixture.Services.ComparerService.GetComparer<TestViewModel>();
        comparer.Should().NotBeNull("GetComparer sollte einen Comparer zurückgeben");
    }

    [Fact]
    public void ViewModelFactory_Create_DoesNotThrow()
    {
        var dto = new TestDto { Name = "Test" };
        var act = () => _fixture.ViewModelFactory.Create(dto);
        act.Should().NotThrow("ViewModelFactory.Create sollte nicht werfen");
    }

    [Fact]
    public void ViewModelFactory_Create_ReturnsViewModel()
    {
        var dto = new TestDto { Name = "Test" };
        var viewModel = _fixture.ViewModelFactory.Create(dto);
        viewModel.Should().NotBeNull("ViewModelFactory sollte ein ViewModel erstellen");
        viewModel.Model.Should().BeSameAs(dto);
    }

    [Fact]
    public void CollectionViewModel_Constructor_DoesNotThrow()
    {
        var act = () => new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);
        
        act.Should().NotThrow("CollectionViewModel-Konstruktor sollte nicht werfen");
    }

    [Fact]
    public void CollectionViewModel_Constructor_CreatesInstance()
    {
        var viewModel = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        viewModel.Should().NotBeNull("CollectionViewModel sollte erstellt werden");
        viewModel.Dispose();
    }

    [Fact]
    public void MultipleCollectionViewModels_CanBeCreated()
    {
        var vm1 = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        var vm2 = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        vm1.Should().NotBeNull();
        vm2.Should().NotBeNull();

        vm1.Dispose();
        vm2.Dispose();
    }
}
