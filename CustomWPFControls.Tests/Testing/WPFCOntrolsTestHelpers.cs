using System.Collections.Generic;
using CustomWPFControls.Bootstrap;
using CustomWPFControls.Factories;
using DataStores.Abstractions;
using DataStores.Runtime;
using Microsoft.Extensions.DependencyInjection;
using TestHelper.DataStores.Models;

namespace CustomWPFControls.Tests.Testing
{
    /// <summary>
    /// Test-Helper-Klasse mit Factory-Methods für Test-Objekte.
    /// </summary>
    public static class WPFCOntrolsTestHelpers
    {
        /// <summary>
        /// Erstellt einen InMemoryDataStore mit TestDto.
        /// </summary>
        public static IDataStore<TestDto> CreateDataStore(IEqualityComparer<TestDto>? comparer = null)
        {
            return new InMemoryDataStore<TestDto>(comparer ?? EqualityComparer<TestDto>.Default);
        }

        /// <summary>
        /// Erstellt einen Mock IDataStores mit TestDto-Store.
        /// </summary>
        public static IDataStores CreateDataStores(IDataStore<TestDto>? modelStore = null)
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
            services.AddSingleton<IEqualityComparer<TestDto>>(
                EqualityComparer<TestDto>.Default);

            // IEqualityComparerService
            services.AddSingleton<IEqualityComparerService>(CreateComparerService());

            // DataStore
            var dataStore = CreateDataStore(EqualityComparer<TestDto>.Default);
            services.AddSingleton<IDataStore<TestDto>>(dataStore);

            // IDataStores
            services.AddSingleton<IDataStores>(CreateDataStores(dataStore));

            // ViewModelFactory
            services.AddViewModelPackage<TestDto, TestViewModel>();

            return services.BuildServiceProvider();
        }

        /// <summary>
        /// Erstellt ein TestDto mit Default-Werten.
        /// </summary>
        public static TestDto CreateTestDto(string name = "Test")
        {
            return new TestDto
            {
                Name = name
            };
        }

        /// <summary>
        /// Erstellt mehrere TestDtos.
        /// </summary>
        public static List<TestDto> CreateTestDtos(int count)
        {
            var models = new List<TestDto>();
            for (int i = 1; i <= count; i++)
            {
                models.Add(CreateTestDto($"Item{i}"));
            }
            return models;
        }

        #region Test Facades

        private class DataStoresTestFacade : IDataStores
        {
            private readonly IDataStore<TestDto> _testDtoStore;

            public DataStoresTestFacade(IDataStore<TestDto> testDtoStore)
            {
                _testDtoStore = testDtoStore;
            }

            public IDataStore<T> GetGlobal<T>() where T : class
            {
                if (typeof(T) == typeof(TestDto))
                {
                    return (IDataStore<T>)_testDtoStore;
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
