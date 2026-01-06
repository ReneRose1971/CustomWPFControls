using System;
using CustomWPFControls.Extensions;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Extensions.LoadData;

/// <summary>
/// Tests für LoadData-Extension mit deaktivierter Auto-Selektion.
/// </summary>
/// <remarks>
/// Szenario: LoadData wird mit selectFirst = false aufgerufen.
/// ACT: LoadData mit 3 TestDtos und selectFirst = false im Constructor.
/// </remarks>
public sealed class LoadData_WithSelectFirstFalse_DoesNotSelectItem : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly TestDto[] _testData;

    public LoadData_WithSelectFirstFalse_DoesNotSelectItem(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();

        _testData = new[]
        {
            new TestDto { Name = "First" },
            new TestDto { Name = "Second" },
            new TestDto { Name = "Third" }
        };

        // ACT: LoadData mit deaktivierter Selektion
        _fixture.Sut.LoadData(_testData, selectFirst: false);
    }

    [Fact]
    public void LoadsAllItemsToStore()
    {
        _fixture.Sut.Items.Should().HaveCount(3);
    }

    [Fact]
    public void SelectedItemRemainsNull()
    {
        _fixture.Sut.SelectedItem.Should().BeNull();
    }

    [Fact]
    public void ViewModelsAreAvailable()
    {
        _fixture.Sut.Items[0].Name.Should().Be("First");
        _fixture.Sut.Items[1].Name.Should().Be("Second");
        _fixture.Sut.Items[2].Name.Should().Be("Third");
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
