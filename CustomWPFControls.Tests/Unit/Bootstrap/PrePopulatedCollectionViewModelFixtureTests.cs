using CustomWPFControls.Tests.Testing;
using CustomWPFControls.Tests.Testing.Bootstrap;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.Bootstrap;

/// <summary>
/// Tests für PrePopulatedCollectionViewModelFixture.
/// </summary>
/// <remarks>
/// Testet:
/// - Bootstrap mit vorausgefüllten Daten
/// - TestData Property
/// - DataStore-Integration
/// - Vererbung von CollectionViewModelFixture
/// </remarks>
public sealed class PrePopulatedCollectionViewModelFixtureTests : IDisposable
{
    private PrePopulatedCollectionViewModelFixture? _fixture;

    #region Bootstrap & Initialization Tests

    [Fact]
    public void Constructor_InitializesAllBaseProperties()
    {
        // Act
        _fixture = new PrePopulatedCollectionViewModelFixture();

        // Assert - Basis-Properties von CollectionViewModelFixture
        _fixture.ServiceProvider.Should().NotBeNull();
        _fixture.DataStores.Should().NotBeNull();
        _fixture.ComparerService.Should().NotBeNull();
        _fixture.PathProvider.Should().NotBeNull();
        _fixture.TestDtoStore.Should().NotBeNull();
        _fixture.ViewModelFactory.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_InitializesTestDataProperty()
    {
        // Act
        _fixture = new PrePopulatedCollectionViewModelFixture();

        // Assert
        _fixture.TestData.Should().NotBeNull();
        _fixture.TestData.Should().NotBeEmpty();
    }

    [Fact]
    public void Constructor_CreatesExactlyThreeTestItems()
    {
        // Act
        _fixture = new PrePopulatedCollectionViewModelFixture();

        // Assert
        _fixture.TestData.Should().HaveCount(3);
    }

    #endregion

    #region TestData Property Tests

    [Fact]
    public void TestData_IsReadOnlyList()
    {
        // Act
        _fixture = new PrePopulatedCollectionViewModelFixture();

        // Assert
        _fixture.TestData.Should().BeAssignableTo<IReadOnlyList<TestDto>>();
    }

    [Fact]
    public void TestData_ContainsItemsWithCorrectNames()
    {
        // Act
        _fixture = new PrePopulatedCollectionViewModelFixture();

        // Assert
        _fixture.TestData.Should().Contain(dto => dto.Name == "FirstItem");
        _fixture.TestData.Should().Contain(dto => dto.Name == "SecondItem");
        _fixture.TestData.Should().Contain(dto => dto.Name == "ThirdItem");
    }

    [Fact]
    public void TestData_ItemsAreInCorrectOrder()
    {
        // Act
        _fixture = new PrePopulatedCollectionViewModelFixture();

        // Assert
        _fixture.TestData[0].Name.Should().Be("FirstItem");
        _fixture.TestData[1].Name.Should().Be("SecondItem");
        _fixture.TestData[2].Name.Should().Be("ThirdItem");
    }

    [Fact]
    public void TestData_AllItemsAreTestDtoInstances()
    {
        // Act
        _fixture = new PrePopulatedCollectionViewModelFixture();

        // Assert
        _fixture.TestData.Should().AllBeOfType<TestDto>();
    }

    [Fact]
    public void TestData_AllItemsHaveNonEmptyNames()
    {
        // Act
        _fixture = new PrePopulatedCollectionViewModelFixture();

        // Assert
        _fixture.TestData.Should().OnlyContain(dto => !string.IsNullOrEmpty(dto.Name));
    }

    [Fact]
    public void TestData_AllItemsHaveGeneratedIds()
    {
        // Act
        _fixture = new PrePopulatedCollectionViewModelFixture();

        // Assert - TestDto generiert automatisch Guid-IDs
        _fixture.TestData.Should().OnlyContain(dto => dto.Id != Guid.Empty);
    }

    #endregion

    #region DataStore Integration Tests

    [Fact]
    public void TestDtoStore_ContainsAllTestDataItems()
    {
        // Act
        _fixture = new PrePopulatedCollectionViewModelFixture();

        // Assert
        _fixture.TestDtoStore.Items.Should().HaveCount(3);
    }

    [Fact]
    public void TestDtoStore_ContainsSameInstancesAsTestData()
    {
        // Act
        _fixture = new PrePopulatedCollectionViewModelFixture();

        // Assert - Store sollte die gleichen Referenzen enthalten
        foreach (var testDto in _fixture.TestData)
        {
            _fixture.TestDtoStore.Items.Should().Contain(testDto);
        }
    }

    [Fact]
    public void TestDtoStore_ItemsHaveCorrectNames()
    {
        // Act
        _fixture = new PrePopulatedCollectionViewModelFixture();

        // Assert
        _fixture.TestDtoStore.Items.Should().Contain(dto => dto.Name == "FirstItem");
        _fixture.TestDtoStore.Items.Should().Contain(dto => dto.Name == "SecondItem");
        _fixture.TestDtoStore.Items.Should().Contain(dto => dto.Name == "ThirdItem");
    }

    #endregion

    #region ClearTestData Tests

    [Fact]
    public void ClearTestData_RemovesAllItemsFromStore()
    {
        // Arrange
        _fixture = new PrePopulatedCollectionViewModelFixture();

        // Act
        _fixture.ClearTestData();

        // Assert
        _fixture.TestDtoStore.Items.Should().BeEmpty();
    }

    [Fact]
    public void ClearTestData_DoesNotModifyTestDataProperty()
    {
        // Arrange
        _fixture = new PrePopulatedCollectionViewModelFixture();
        var originalTestData = _fixture.TestData;

        // Act
        _fixture.ClearTestData();

        // Assert - TestData Property sollte unverändert bleiben
        _fixture.TestData.Should().BeSameAs(originalTestData);
        _fixture.TestData.Should().HaveCount(3);
    }

    [Fact]
    public void ClearTestData_AllowsReAddingItems()
    {
        // Arrange
        _fixture = new PrePopulatedCollectionViewModelFixture();
        var firstItem = _fixture.TestData[0];
        _fixture.ClearTestData();

        // Act - Item wieder hinzufügen
        _fixture.TestDtoStore.Add(firstItem);

        // Assert
        _fixture.TestDtoStore.Items.Should().ContainSingle();
        _fixture.TestDtoStore.Items.Should().Contain(firstItem);
    }

    #endregion

    #region Inheritance Tests

    [Fact]
    public void Fixture_InheritsFromCollectionViewModelFixture()
    {
        // Act
        _fixture = new PrePopulatedCollectionViewModelFixture();

        // Assert
        _fixture.Should().BeAssignableTo<CollectionViewModelFixture>();
    }

    [Fact]
    public void Fixture_InheritsFromDataStoresFixtureBase()
    {
        // Act
        _fixture = new PrePopulatedCollectionViewModelFixture();

        // Assert
        _fixture.Should().BeAssignableTo<DataStoresFixtureBase>();
    }

    #endregion

    #region Multiple Instances Tests

    [Fact]
    public void MultipleFixtureInstances_HaveIsolatedData()
    {
        // Arrange & Act
        using var fixture1 = new PrePopulatedCollectionViewModelFixture();
        using var fixture2 = new PrePopulatedCollectionViewModelFixture();

        // Assert - Beide haben ihre eigenen Daten
        fixture1.TestData.Should().HaveCount(3);
        fixture2.TestData.Should().HaveCount(3);

        // Assert - Aber unterschiedliche Instanzen
        fixture1.TestData.Should().NotBeSameAs(fixture2.TestData);
        fixture1.TestData[0].Should().NotBeSameAs(fixture2.TestData[0]);
    }

    [Fact]
    public void MultipleFixtureInstances_HaveIsolatedStores()
    {
        // Arrange & Act
        using var fixture1 = new PrePopulatedCollectionViewModelFixture();
        using var fixture2 = new PrePopulatedCollectionViewModelFixture();

        fixture1.ClearTestData();

        // Assert - Nur fixture1 wurde gecleared
        fixture1.TestDtoStore.Items.Should().BeEmpty();
        fixture2.TestDtoStore.Items.Should().HaveCount(3);
    }

    #endregion

    #region Lifecycle Tests

    [Fact]
    public void Dispose_CleansUpResourcesProperly()
    {
        // Arrange
        _fixture = new PrePopulatedCollectionViewModelFixture();
        var serviceProvider = _fixture.ServiceProvider;

        // Act
        _fixture.Dispose();

        // Assert - ServiceProvider sollte disposed sein
        var act = () => serviceProvider.GetService(typeof(DataStores.Abstractions.IDataStores));
        act.Should().Throw<ObjectDisposedException>();
    }

    #endregion

    public void Dispose()
    {
        _fixture?.Dispose();
    }
}
