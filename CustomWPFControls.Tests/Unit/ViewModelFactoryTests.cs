using System;
using CustomWPFControls.Factories;
using CustomWPFControls.Tests.Testing;
using Microsoft.Extensions.DependencyInjection;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit
{
    /// <summary>
    /// Unit-Tests für ViewModelFactory&lt;TModel, TViewModel&gt;.
    /// </summary>
    public class ViewModelFactoryTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidServiceProvider_CreatesInstance()
        {
            // Arrange
            var serviceProvider = WPFCOntrolsTestHelpers.CreateServiceProvider();

            // Act
            var factory = new ViewModelFactory<TestDto, TestViewModel>(serviceProvider);

            // Assert
            Assert.NotNull(factory);
        }

        [Fact]
        public void Constructor_WithNullServiceProvider_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new ViewModelFactory<TestDto, TestViewModel>(null!));
        }

        #endregion

        #region Create Tests

        [Fact]
        public void Create_WithValidModel_ReturnsViewModel()
        {
            // Arrange
            var serviceProvider = WPFCOntrolsTestHelpers.CreateServiceProvider();
            var factory = new ViewModelFactory<TestDto, TestViewModel>(serviceProvider);
            var model = new TestDto { Name = "Test" };

            // Act
            var viewModel = factory.Create(model);

            // Assert
            Assert.NotNull(viewModel);
            Assert.Same(model, viewModel.Model);
        }

        [Fact]
        public void Create_WithNullModel_ThrowsArgumentNullException()
        {
            // Arrange
            var serviceProvider = WPFCOntrolsTestHelpers.CreateServiceProvider();
            var factory = new ViewModelFactory<TestDto, TestViewModel>(serviceProvider);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => factory.Create(null!));
        }

        [Fact]
        public void Create_MultipleCalls_ReturnsDifferentInstances()
        {
            // Arrange
            var serviceProvider = WPFCOntrolsTestHelpers.CreateServiceProvider();
            var factory = new ViewModelFactory<TestDto, TestViewModel>(serviceProvider);
            var model = new TestDto { Name = "Test" };

            // Act
            var vm1 = factory.Create(model);
            var vm2 = factory.Create(model);

            // Assert
            Assert.NotSame(vm1, vm2); // Unterschiedliche ViewModel-Instanzen
            Assert.Same(vm1.Model, vm2.Model); // Aber gleiches Model
        }

        [Fact]
        public void Create_WithDifferentModels_ReturnsDifferentViewModels()
        {
            // Arrange
            var serviceProvider = WPFCOntrolsTestHelpers.CreateServiceProvider();
            var factory = new ViewModelFactory<TestDto, TestViewModel>(serviceProvider);
            var model1 = new TestDto { Name = "Model1" };
            var model2 = new TestDto { Name = "Model2" };

            // Act
            var vm1 = factory.Create(model1);
            var vm2 = factory.Create(model2);

            // Assert
            Assert.NotSame(vm1, vm2);
            Assert.NotSame(vm1.Model, vm2.Model);
            Assert.Equal("Model1", vm1.Name);
            Assert.Equal("Model2", vm2.Name);
        }

        #endregion

        #region DI Integration Tests

        [Fact]
        public void Create_ResolvesDependenciesFromServiceProvider()
        {
            // Arrange
            var services = new ServiceCollection();
            
            // Registriere ViewModel mit zusätzlicher Dependency (als Beispiel)
            // TestViewModel hat keine zusätzlichen Dependencies, aber das Prinzip wird getestet
            services.AddViewModelFactory<TestDto, TestViewModel>();
            var serviceProvider = services.BuildServiceProvider();

            var factory = serviceProvider.GetRequiredService<IViewModelFactory<TestDto, TestViewModel>>();
            var model = new TestDto { Name = "Test" };

            // Act
            var viewModel = factory.Create(model);

            // Assert
            Assert.NotNull(viewModel);
            Assert.Same(model, viewModel.Model);
        }

        #endregion
    }
}
