using System;
using System.Collections.Generic;
using System.Linq;
using CustomWPFControls.Factories;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using DataStores.Abstractions;
using Xunit;

namespace CustomWPFControls.Tests.Integration
{
    /// <summary>
    /// Integration-Tests für CollectionViewModel mit DataStore-Integration.
    /// </summary>
    public class CollectionViewModelIntegrationTests
    {
        #region Setup Helpers

        private (CollectionViewModel<TestModel, TestViewModel> viewModel, IDataStore<TestModel> dataStore) CreateCollectionViewModel(
            IDataStore<TestModel>? dataStore = null)
        {
            dataStore ??= WPFCOntrolsTestHelpers.CreateDataStore();
            
            var dataStores = WPFCOntrolsTestHelpers.CreateDataStores(dataStore);
            var comparerService = WPFCOntrolsTestHelpers.CreateComparerService();
            var serviceProvider = WPFCOntrolsTestHelpers.CreateServiceProvider();
            var factory = serviceProvider.GetService(typeof(IViewModelFactory<TestModel, TestViewModel>)) 
                as IViewModelFactory<TestModel, TestViewModel>;

            var viewModel = new CollectionViewModel<TestModel, TestViewModel>(
                dataStores, factory!, comparerService);

            return (viewModel, dataStore);
        }

        #endregion

        #region Constructor & Initialization Tests

        [Fact]
        public void Constructor_WithValidDependencies_CreatesInstance()
        {
            // Arrange & Act
            var (viewModel, _) = CreateCollectionViewModel();

            // Assert
            Assert.NotNull(viewModel);
            Assert.NotNull(viewModel.Items);
            Assert.Equal(0, viewModel.Count);
        }

        [Fact]
        public void Constructor_WithNullDataStores_ThrowsArgumentNullException()
        {
            // Arrange
            var serviceProvider = WPFCOntrolsTestHelpers.CreateServiceProvider();
            var factory = serviceProvider.GetService(typeof(IViewModelFactory<TestModel, TestViewModel>)) 
                as IViewModelFactory<TestModel, TestViewModel>;
            var comparerService = WPFCOntrolsTestHelpers.CreateComparerService();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new CollectionViewModel<TestModel, TestViewModel>(null!, factory!, comparerService));
        }

        [Fact]
        public void Constructor_WithNullFactory_ThrowsArgumentNullException()
        {
            // Arrange
            var dataStore = WPFCOntrolsTestHelpers.CreateDataStore();
            var dataStores = WPFCOntrolsTestHelpers.CreateDataStores(dataStore);
            var comparerService = WPFCOntrolsTestHelpers.CreateComparerService();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new CollectionViewModel<TestModel, TestViewModel>(dataStores, null!, comparerService));
        }

        [Fact]
        public void Constructor_WithNullComparerService_ThrowsArgumentNullException()
        {
            // Arrange
            var dataStore = WPFCOntrolsTestHelpers.CreateDataStore();
            var dataStores = WPFCOntrolsTestHelpers.CreateDataStores(dataStore);
            var serviceProvider = WPFCOntrolsTestHelpers.CreateServiceProvider();
            var factory = serviceProvider.GetService(typeof(IViewModelFactory<TestModel, TestViewModel>)) 
                as IViewModelFactory<TestModel, TestViewModel>;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new CollectionViewModel<TestModel, TestViewModel>(dataStores, factory!, null!));
        }

        [Fact]
        public void Constructor_WithPrePopulatedDataStore_CreatesInitialViewModels()
        {
            // Arrange
            var dataStore = WPFCOntrolsTestHelpers.CreateDataStore();
            var models = WPFCOntrolsTestHelpers.CreateTestModels(3);
            foreach (var model in models)
            {
                dataStore.Add(model);
            }

            // Act
            var (viewModel, _) = CreateCollectionViewModel(dataStore);

            // Assert
            Assert.Equal(3, viewModel.Count);
            Assert.Equal(3, viewModel.Items.Count);
            Assert.All(viewModel.Items, vm => Assert.NotNull(vm.Model));
        }

        #endregion

        #region DataStore ? ViewModels Synchronization Tests

        [Fact]
        public void DataStoreAdd_CreatesNewViewModel()
        {
            // Arrange
            var (viewModel, dataStore) = CreateCollectionViewModel();
            var model = WPFCOntrolsTestHelpers.CreateTestModel(1, "Model1");

            // Act
            dataStore.Add(model);

            // Assert
            Assert.Equal(1, viewModel.Count);
            Assert.Single(viewModel.Items);
            Assert.Same(model, viewModel.Items[0].Model);
        }

        [Fact]
        public void DataStoreAddMultiple_CreatesMultipleViewModels()
        {
            // Arrange
            var (viewModel, dataStore) = CreateCollectionViewModel();
            var models = WPFCOntrolsTestHelpers.CreateTestModels(5);

            // Act
            foreach (var model in models)
            {
                dataStore.Add(model);
            }

            // Assert
            Assert.Equal(5, viewModel.Count);
            Assert.Equal(5, viewModel.Items.Count);
        }

        [Fact]
        public void DataStoreRemove_RemovesViewModel()
        {
            // Arrange
            var (viewModel, dataStore) = CreateCollectionViewModel();
            var model = WPFCOntrolsTestHelpers.CreateTestModel(1, "Model1");
            dataStore.Add(model);

            // Act
            dataStore.Remove(model);

            // Assert
            Assert.Equal(0, viewModel.Count);
            Assert.Empty(viewModel.Items);
        }

        [Fact]
        public void DataStoreRemoveMultiple_RemovesMultipleViewModels()
        {
            // Arrange
            var (viewModel, dataStore) = CreateCollectionViewModel();
            var models = WPFCOntrolsTestHelpers.CreateTestModels(5);
            foreach (var model in models)
            {
                dataStore.Add(model);
            }

            // Act
            dataStore.Remove(models[0]);
            dataStore.Remove(models[2]);
            dataStore.Remove(models[4]);

            // Assert
            Assert.Equal(2, viewModel.Count);
            Assert.Contains(viewModel.Items, vm => vm.Model == models[1]);
            Assert.Contains(viewModel.Items, vm => vm.Model == models[3]);
        }

        [Fact]
        public void DataStoreClear_RemovesAllViewModels()
        {
            // Arrange
            var (viewModel, dataStore) = CreateCollectionViewModel();
            var models = WPFCOntrolsTestHelpers.CreateTestModels(5);
            foreach (var model in models)
            {
                dataStore.Add(model);
            }

            // Act
            dataStore.Clear();

            // Assert
            Assert.Equal(0, viewModel.Count);
            Assert.Empty(viewModel.Items);
        }

        #endregion

        #region SelectedItem Tests

        [Fact]
        public void SelectedItem_CanBeSetAndGet()
        {
            // Arrange
            var (viewModel, dataStore) = CreateCollectionViewModel();
            var model = WPFCOntrolsTestHelpers.CreateTestModel(1, "Model1");
            dataStore.Add(model);
            var vm = viewModel.Items.First();

            // Act
            viewModel.SelectedItem = vm;

            // Assert
            Assert.Same(vm, viewModel.SelectedItem);
        }

        [Fact]
        public void SelectedItem_CanBeSetToNull()
        {
            // Arrange
            var (viewModel, dataStore) = CreateCollectionViewModel();
            var model = WPFCOntrolsTestHelpers.CreateTestModel(1, "Model1");
            dataStore.Add(model);
            viewModel.SelectedItem = viewModel.Items.First();

            // Act
            viewModel.SelectedItem = null;

            // Assert
            Assert.Null(viewModel.SelectedItem);
        }

        [Fact]
        public void SelectedItem_RaisesPropertyChanged()
        {
            // Arrange
            var (viewModel, dataStore) = CreateCollectionViewModel();
            var model = WPFCOntrolsTestHelpers.CreateTestModel(1, "Model1");
            dataStore.Add(model);
            var vm = viewModel.Items.First();

            var propertyChangedRaised = false;
            viewModel.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(viewModel.SelectedItem))
                    propertyChangedRaised = true;
            };

            // Act
            viewModel.SelectedItem = vm;

            // Assert
            Assert.True(propertyChangedRaised);
        }

        [Fact]
        public void SelectedItem_AutoNullOnRemove()
        {
            // Arrange
            var (viewModel, dataStore) = CreateCollectionViewModel();
            var model = WPFCOntrolsTestHelpers.CreateTestModel(1, "Model1");
            dataStore.Add(model);
            viewModel.SelectedItem = viewModel.Items.First();

            // Act
            dataStore.Remove(model);

            // Assert
            Assert.Null(viewModel.SelectedItem);
        }

        #endregion

        #region Count & PropertyChanged Tests

        [Fact]
        public void Count_UpdatesWhenItemsAdded()
        {
            // Arrange
            var (viewModel, dataStore) = CreateCollectionViewModel();

            // Act & Assert
            Assert.Equal(0, viewModel.Count);
            
            dataStore.Add(WPFCOntrolsTestHelpers.CreateTestModel(1));
            Assert.Equal(1, viewModel.Count);
            
            dataStore.Add(WPFCOntrolsTestHelpers.CreateTestModel(2));
            Assert.Equal(2, viewModel.Count);
        }

        [Fact]
        public void Count_RaisesPropertyChangedWhenItemsAdded()
        {
            // Arrange
            var (viewModel, dataStore) = CreateCollectionViewModel();
            var propertyChangedRaised = false;
            
            viewModel.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(viewModel.Count))
                    propertyChangedRaised = true;
            };

            // Act
            dataStore.Add(WPFCOntrolsTestHelpers.CreateTestModel(1));

            // Assert
            Assert.True(propertyChangedRaised);
        }

        #endregion

        #region Dispose Tests

        [Fact]
        public void Dispose_UnsubscribesFromDataStore()
        {
            // Arrange
            var (viewModel, dataStore) = CreateCollectionViewModel();
            var model = WPFCOntrolsTestHelpers.CreateTestModel(1);

            // Act
            viewModel.Dispose();
            dataStore.Add(model); // Should not update viewModel

            // Assert
            Assert.Equal(0, viewModel.Count); // Still 0 because unsubscribed
        }

        [Fact]
        public void Dispose_ClearsViewModels()
        {
            // Arrange
            var dataStore = WPFCOntrolsTestHelpers.CreateDataStore();
            var models = WPFCOntrolsTestHelpers.CreateTestModels(3);
            foreach (var model in models)
            {
                dataStore.Add(model);
            }
            var (viewModel, _) = CreateCollectionViewModel(dataStore);

            // Act
            viewModel.Dispose();

            // Assert
            Assert.Equal(0, viewModel.Count);
            Assert.Empty(viewModel.Items);
        }

        #endregion
    }
}
