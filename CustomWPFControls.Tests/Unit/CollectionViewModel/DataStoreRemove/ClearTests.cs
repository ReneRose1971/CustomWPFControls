using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.DataStoreRemove;

/// <summary>
/// Tests für das Leeren des DataStores via Clear().
/// </summary>
/// <remarks>
/// Setup: 3 Items hinzufügen
/// ACT: Store.Clear()
/// </remarks>
public sealed class ClearTests : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly CollectionViewModel<TestDto, TestViewModel> _sut;

    public ClearTests(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();

        // Setup: 3 Items hinzufügen
        _fixture.TestDtoStore.AddRange(new[]
        {
            new TestDto { Name = "First" },
            new TestDto { Name = "Second" },
            new TestDto { Name = "Third" }
        });

        _sut = new CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        // ACT: Store leeren
        _fixture.TestDtoStore.Clear();
    }

    [Fact]
    public void RemovesAllViewModels()
    {
        _sut.Items.Should().BeEmpty();
    }

    [Fact]
    public void CountIsZero()
    {
        _sut.Count.Should().Be(0);
    }

    [Fact]
    public void ItemsIsEmpty()
    {
        _sut.Items.Count.Should().Be(0);
    }
}
