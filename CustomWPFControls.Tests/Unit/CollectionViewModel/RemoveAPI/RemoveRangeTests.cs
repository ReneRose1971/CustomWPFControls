using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.RemoveAPI;

/// <summary>
/// Tests für die RemoveRange(viewModels) Public API.
/// </summary>
/// <remarks>
/// Setup: 5 Items hinzufügen
/// ACT: sut.RemoveRange([2 viewModels])
/// </remarks>
public sealed class RemoveRangeTests : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly CollectionViewModel<TestDto, TestViewModel> _sut;
    private readonly TestViewModel[] _removedViewModels;

    public RemoveRangeTests(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();

        // Setup: 5 Items hinzufügen
        _fixture.TestDtoStore.AddRange(new[]
        {
            new TestDto { Name = "Item1" },
            new TestDto { Name = "Item2" },
            new TestDto { Name = "Item3" },
            new TestDto { Name = "Item4" },
            new TestDto { Name = "Item5" }
        });

        _sut = new CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        // ACT: 2 ViewModels entfernen
        _removedViewModels = new[] { _sut.Items[1], _sut.Items[3] };
        _sut.RemoveRange(_removedViewModels);
    }

    [Fact]
    public void RemovesMultipleViewModels()
    {
        _sut.Items.Should().NotContain(_removedViewModels);
    }

    [Fact]
    public void RemovesModelsFromStore()
    {
        foreach (var vm in _removedViewModels)
        {
            _fixture.TestDtoStore.Items.Should().NotContain(vm.Model);
        }
    }

    [Fact]
    public void CountIsThree()
    {
        _sut.Count.Should().Be(3);
    }
}
