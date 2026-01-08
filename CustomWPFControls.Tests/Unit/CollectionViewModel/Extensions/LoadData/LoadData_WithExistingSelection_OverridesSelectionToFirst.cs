using System;
using CustomWPFControls.Extensions;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Extensions.LoadData;

/// <summary>
/// Tests für LoadData-Extension: Überschreibt bestehende Selektion.
/// </summary>
/// <remarks>
/// Szenario: LoadData wird mit selectFirst = true aufgerufen, während bereits ein Item selektiert ist.
/// ACT: Item manuell selektieren, dann LoadData mit neuen Daten im Constructor.
/// Validiert deterministisches Verhalten: Erstes Item wird IMMER selektiert bei selectFirst = true.
/// </remarks>
public sealed class LoadData_WithExistingSelection_OverridesSelectionToFirst : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;
    private readonly TestDto[] _newData;

    public LoadData_WithExistingSelection_OverridesSelectionToFirst(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();

        // ARRANGE: Bestehende Daten mit Selektion auf zweitem Item
        _fixture.AddTestData("Old1", "Old2", "Old3");
        _fixture.Sut.SelectedItem = _fixture.Sut.Items[1]; // "Old2" selektiert

        _newData = new[]
        {
            new TestDto { Name = "New1" },
            new TestDto { Name = "New2" },
            new TestDto { Name = "New3" }
        };

        // ACT: LoadData mit Standard-Selektion (selectFirst = true)
        _fixture.Sut.LoadData(_newData);
    }

    [Fact]
    public void SelectsFirstItemOfNewData()
    {
        _fixture.Sut.SelectedItem.Should().NotBeNull();
        _fixture.Sut.SelectedItem!.Name.Should().Be("New1");
    }

    [Fact]
    public void SelectedItemIsFirstViewModel()
    {
        _fixture.Sut.SelectedItem.Should().BeSameAs(_fixture.Sut.Items[0]);
    }

    [Fact]
    public void OldSelectionIsGone()
    {
        _fixture.Sut.Items.Should().NotContain(vm => vm.Name == "Old2");
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
