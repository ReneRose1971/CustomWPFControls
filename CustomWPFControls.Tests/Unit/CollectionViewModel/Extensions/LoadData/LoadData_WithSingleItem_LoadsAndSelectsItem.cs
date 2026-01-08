using System;
using CustomWPFControls.Extensions;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Extensions.LoadData;

/// <summary>
/// Tests für LoadData-Extension mit einzelnem Item.
/// </summary>
/// <remarks>
/// Szenario: LoadData wird mit genau einem Item aufgerufen.
/// ACT: LoadData mit einem TestDto im Constructor.
/// Validiert Edge-Case: Einzelnes Item wird korrekt geladen und selektiert.
/// </remarks>
public sealed class LoadData_WithSingleItem_LoadsAndSelectsItem : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;
    private readonly TestDto _singleItem;

    public LoadData_WithSingleItem_LoadsAndSelectsItem(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();

        _singleItem = new TestDto { Name = "OnlyItem" };

        // ACT: LoadData mit einzelnem Item
        _fixture.Sut.LoadData(new[] { _singleItem });
    }

    [Fact]
    public void LoadsSingleItem()
    {
        _fixture.Sut.Items.Should().HaveCount(1);
    }

    [Fact]
    public void CountIsOne()
    {
        _fixture.Sut.Count.Should().Be(1);
    }

    [Fact]
    public void ItemHasCorrectData()
    {
        _fixture.Sut.Items[0].Name.Should().Be("OnlyItem");
    }

    [Fact]
    public void SelectsTheOnlyItem()
    {
        _fixture.Sut.SelectedItem.Should().NotBeNull();
        _fixture.Sut.SelectedItem!.Name.Should().Be("OnlyItem");
    }

    [Fact]
    public void SelectedItemReferencesTheViewModel()
    {
        _fixture.Sut.SelectedItem.Should().BeSameAs(_fixture.Sut.Items[0]);
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
