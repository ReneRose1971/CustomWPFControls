using System;
using CustomWPFControls.Extensions;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Extensions.LoadData;

/// <summary>
/// Tests für LoadData-Extension mit validen Daten und Standard-Selektion.
/// </summary>
/// <remarks>
/// Szenario: LoadData wird mit mehreren Items aufgerufen (selectFirst = default/true).
/// ACT: LoadData mit 3 TestDtos im Constructor.
/// </remarks>
public sealed class LoadData_WithValidData_LoadsDataAndSelectsFirst : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly TestDto[] _testData;

    public LoadData_WithValidData_LoadsDataAndSelectsFirst(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();

        _testData = new[]
        {
            new TestDto { Name = "First" },
            new TestDto { Name = "Second" },
            new TestDto { Name = "Third" }
        };

        // ACT: LoadData mit Standard-Selektion
        _fixture.Sut.LoadData(_testData);
    }

    [Fact]
    public void LoadsAllItemsToStore()
    {
        _fixture.Sut.Items.Should().HaveCount(3);
    }

    [Fact]
    public void CountIsCorrect()
    {
        _fixture.Sut.Count.Should().Be(3);
    }

    [Fact]
    public void ViewModelsHaveCorrectData()
    {
        _fixture.Sut.Items[0].Name.Should().Be("First");
        _fixture.Sut.Items[1].Name.Should().Be("Second");
        _fixture.Sut.Items[2].Name.Should().Be("Third");
    }

    [Fact]
    public void SelectsFirstItem()
    {
        _fixture.Sut.SelectedItem.Should().NotBeNull();
        _fixture.Sut.SelectedItem!.Name.Should().Be("First");
    }

    [Fact]
    public void SelectedItemReferencesFirstViewModel()
    {
        _fixture.Sut.SelectedItem.Should().BeSameAs(_fixture.Sut.Items[0]);
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
