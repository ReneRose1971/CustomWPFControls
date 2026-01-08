using System;
using System.Linq;
using CustomWPFControls.Extensions;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Extensions.LoadData;

/// <summary>
/// Tests für LoadData-Extension mit leerer Collection.
/// </summary>
/// <remarks>
/// Szenario: LoadData wird mit leerer Enumerable aufgerufen.
/// ACT: LoadData mit Enumerable.Empty im Constructor.
/// Validiert Library-Verhalten: Keine Exception, UI bleibt einfach leer.
/// </remarks>
public sealed class LoadData_WithEmptyData_ClearsCollectionWithoutError : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;

    public LoadData_WithEmptyData_ClearsCollectionWithoutError(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();

        // ARRANGE: Bestehende Daten im Store
        _fixture.AddTestData("Existing1", "Existing2");

        // ACT: LoadData mit leerer Collection
        _fixture.Sut.LoadData(Enumerable.Empty<TestDto>());
    }

    [Fact]
    public void ClearsAllItems()
    {
        _fixture.Sut.Items.Should().BeEmpty();
    }

    [Fact]
    public void CountIsZero()
    {
        _fixture.Sut.Count.Should().Be(0);
    }

    [Fact]
    public void SelectedItemIsNull()
    {
        _fixture.Sut.SelectedItem.Should().BeNull();
    }

    [Fact]
    public void DoesNotThrowException()
    {
        // Test-Assertion: Wenn wir hier ankommen, wurde keine Exception geworfen
        _fixture.Sut.Items.Should().NotBeNull();
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
