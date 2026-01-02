using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.DataStoreRemove;

/// <summary>
/// Tests für das Entfernen eines einzelnen Items aus dem DataStore.
/// </summary>
/// <remarks>
/// Setup: 3 Items hinzufügen
/// ACT: Mittleres Item entfernen
/// </remarks>
public sealed class RemoveSingleTests : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly CollectionViewModel<TestDto, TestViewModel> _sut;
    private readonly TestDto _removedDto;
    private readonly TestDto[] _remainingDtos;

    public RemoveSingleTests(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();

        // Setup: 3 Items hinzufügen
        var dto1 = new TestDto { Name = "First" };
        var dto2 = new TestDto { Name = "Second" };
        var dto3 = new TestDto { Name = "Third" };
        _fixture.TestDtoStore.AddRange(new[] { dto1, dto2, dto3 });

        _sut = new CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        // ACT: Mittleres Item entfernen
        _removedDto = dto2;
        _remainingDtos = new[] { dto1, dto3 };
        _fixture.TestDtoStore.Remove(dto2);
    }

    [Fact]
    public void RemovesViewModel()
    {
        _sut.Items.Should().NotContain(vm => vm.Model == _removedDto);
    }

    [Fact]
    public void CountIsTwo()
    {
        _sut.Count.Should().Be(2);
    }

    [Fact]
    public void RemainingViewModelsCorrect()
    {
        _sut.Items.Should().Contain(vm => vm.Name == "First");
        _sut.Items.Should().Contain(vm => vm.Name == "Third");
    }

    [Fact]
    public void RemovedModelNotInItems()
    {
        _sut.Items.Should().NotContain(vm => vm.Name == "Second");
    }
}
