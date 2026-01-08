using System;
using CustomWPFControls.Extensions;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Extensions.LoadData;

/// <summary>
/// Tests für LoadData-Extension: Ersetzt bestehende Daten komplett.
/// </summary>
/// <remarks>
/// Szenario: LoadData wird auf CollectionViewModel mit bestehenden Daten aufgerufen.
/// ACT: Erst 2 Items hinzufügen, dann LoadData mit 3 neuen Items im Constructor.
/// Validiert, dass LoadData Clear + AddRange ausführt (vollständiger Ersatz).
/// </remarks>
public sealed class LoadData_WithExistingData_ReplacesCompletelyWithNewData : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;
    private readonly TestDto[] _newData;

    public LoadData_WithExistingData_ReplacesCompletelyWithNewData(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();

        // ARRANGE: Bestehende Daten im Store
        _fixture.AddTestData("Old1", "Old2");

        _newData = new[]
        {
            new TestDto { Name = "New1" },
            new TestDto { Name = "New2" },
            new TestDto { Name = "New3" }
        };

        // ACT: LoadData mit neuen Daten
        _fixture.Sut.LoadData(_newData);
    }

    [Fact]
    public void ContainsOnlyNewData()
    {
        _fixture.Sut.Items.Should().HaveCount(3);
        _fixture.Sut.Items.Should().OnlyContain(vm => 
            vm.Name == "New1" || vm.Name == "New2" || vm.Name == "New3");
    }

    [Fact]
    public void OldDataIsCompletelyRemoved()
    {
        _fixture.Sut.Items.Should().NotContain(vm => vm.Name == "Old1");
        _fixture.Sut.Items.Should().NotContain(vm => vm.Name == "Old2");
    }

    [Fact]
    public void CountReflectsNewData()
    {
        _fixture.Sut.Count.Should().Be(3);
    }

    [Fact]
    public void SelectsFirstItemOfNewData()
    {
        _fixture.Sut.SelectedItem.Should().NotBeNull();
        _fixture.Sut.SelectedItem!.Name.Should().Be("New1");
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
