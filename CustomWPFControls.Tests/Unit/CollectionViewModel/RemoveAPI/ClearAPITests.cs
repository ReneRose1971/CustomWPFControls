using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.RemoveAPI;

/// <summary>
/// Tests für die Clear() Public API.
/// </summary>
/// <remarks>
/// Setup: 3 Items hinzufügen
/// ACT: sut.Clear()
/// </remarks>
public sealed class ClearAPITests : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly CollectionViewModel<TestDto, TestViewModel> _sut;
    private readonly List<TestViewModel> _viewModelsBeforeClear;

    public ClearAPITests(CollectionViewModelFixture fixture)
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

        _viewModelsBeforeClear = _sut.Items.ToList();

        // ACT: Clear
        _sut.Clear();
    }

    [Fact]
    public void RemovesAllViewModels()
    {
        _sut.Items.Should().BeEmpty();
    }

    [Fact]
    public void ClearsStore()
    {
        _fixture.TestDtoStore.Items.Should().BeEmpty();
    }

    [Fact]
    public void CountIsZero()
    {
        _sut.Count.Should().Be(0);
    }
}
