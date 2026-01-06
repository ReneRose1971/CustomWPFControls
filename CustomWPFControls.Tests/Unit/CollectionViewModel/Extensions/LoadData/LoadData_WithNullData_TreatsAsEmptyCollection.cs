using System;
using CustomWPFControls.Extensions;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Extensions.LoadData;

/// <summary>
/// Tests für LoadData-Extension mit null als Datenwert.
/// </summary>
/// <remarks>
/// Szenario: LoadData wird mit null aufgerufen.
/// ACT: LoadData(null) im Constructor.
/// Validiert defensives Verhalten: Null wird als leere Collection behandelt.
/// </remarks>
public sealed class LoadData_WithNullData_TreatsAsEmptyCollection : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;

    public LoadData_WithNullData_TreatsAsEmptyCollection(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();

        // ARRANGE: Bestehende Daten im Store
        _fixture.AddTestData("Existing1", "Existing2");

        // ACT: LoadData mit null
        _fixture.Sut.LoadData(null);
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
