using System.Collections.Generic;
using CustomWPFControls.Factories;
using DataStores.Abstractions;
using DataStores.Runtime;
using Microsoft.Extensions.DependencyInjection;

namespace CustomWPFControls.Tests.Testing
{
    /// <summary>
    /// Test-Helper-Klasse mit Factory-Methods für Test-Objekte.
    /// </summary>
    public static class WPFCOntrolsTestHelpers
    {
        /// <summary>
        /// Erstellt einen InMemoryDataStore mit TestModel.
        /// </summary>
        public static IDataStore<TestModel> CreateDataStore(IEqualityComparer<TestModel>? comparer = null)
        {
            return new InMemoryDataStore<TestModel>(comparer ?? EqualityComparer<TestModel>.Default);
        }

        /// <summary>
        /// Erstellt einen Mock IDataStores mit TestModel-Store.
        /// </summary>
        public static IDataStores CreateDataStores(IDataStore<TestModel>? modelStore = null)
        {
            var dataStore = modelStore ?? CreateDataStore();
            var dataStores = new DataStoresTestFacade(dataStore);
            return dataStores;
        }

        /// <summary>
        /// Erstellt einen Mock IEqualityComparerService.
        /// </summary>
        public static IEqualityComparerService CreateComparerService()
        {
            return new EqualityComparerServiceTestFacade();
        }

        /// <summary>
        /// Erstellt einen ServiceProvider mit registrierten Test-Dependencies.
        /// </summary>
        public static ServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();

            // EqualityComparer
            services.AddSingleton<IEqualityComparer<TestModel>>(
                EqualityComparer<TestModel>.Default);

            // IEqualityComparerService
            services.AddSingleton<IEqualityComparerService>(CreateComparerService());

            // DataStore
            var dataStore = CreateDataStore(EqualityComparer<TestModel>.Default);
            services.AddSingleton<IDataStore<TestModel>>(dataStore);

            // IDataStores
            services.AddSingleton<IDataStores>(CreateDataStores(dataStore));

            // ViewModelFactory
            services.AddViewModelFactory<TestModel, TestViewModel>();

            return services.BuildServiceProvider();
        }

        /// <summary>
        /// Erstellt ein TestModel mit Default-Werten.
        /// </summary>
        public static TestModel CreateTestModel(int id = 1, string name = "Test")
        {
            return new TestModel
            {
                Id = id,
                Name = name,
                Description = $"Description for {name}"
            };
        }

        /// <summary>
        /// Erstellt mehrere TestModels.
        /// </summary>
        public static List<TestModel> CreateTestModels(int count)
        {
            var models = new List<TestModel>();
            for (int i = 1; i <= count; i++)
            {
                models.Add(CreateTestModel(i, $"Model{i}"));
            }
            return models;
        }

        #region Test Facades

        private class DataStoresTestFacade : IDataStores
        {
            private readonly IDataStore<TestModel> _testModelStore;

            public DataStoresTestFacade(IDataStore<TestModel> testModelStore)
            {
                _testModelStore = testModelStore;
            }

            public IDataStore<T> GetGlobal<T>() where T : class
            {
                if (typeof(T) == typeof(TestModel))
                {
                    return (IDataStore<T>)_testModelStore;
                }
                throw new System.InvalidOperationException($"No store registered for type {typeof(T).Name}");
            }

            public IDataStore<T> CreateLocal<T>(IEqualityComparer<T>? comparer = null) where T : class
            {
                throw new System.NotImplementedException("CreateLocal not implemented in test facade");
            }

            public IDataStore<T> CreateLocalSnapshotFromGlobal<T>(System.Func<T, bool>? filter = null, IEqualityComparer<T>? comparer = null) where T : class
            {
                throw new System.NotImplementedException("CreateLocalSnapshotFromGlobal not implemented in test facade");
            }

            public void Dispose()
            {
            }
        }

        private class EqualityComparerServiceTestFacade : IEqualityComparerService
        {
            public IEqualityComparer<T> GetComparer<T>() where T : class
            {
                return EqualityComparer<T>.Default;
            }
        }

        #endregion
    }
}
