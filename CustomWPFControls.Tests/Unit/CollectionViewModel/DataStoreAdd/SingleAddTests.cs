using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.DataStoreAdd;

/// <summary>
/// Tests für das Hinzufügen eines einzelnen Items zum DataStore.
/// </summary>
/// <remarks>
/// ACT: fixture.TestDtoStore.Add(single TestDto) im Constructor
/// Mehrere Assertions testen verschiedene Aspekte dieser EINEN Operation.
/// </remarks>
public sealed class SingleAddTests : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly CollectionViewModel<TestDto, TestViewModel> _sut;
    private readonly TestDto _addedDto;

    public SingleAddTests(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();

        // SUT erstellen mit leerem Store
        _sut = new CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        // ACT: Ein TestDto hinzufügen
        _addedDto = new TestDto { Name = "TestItem" };
        _fixture.TestDtoStore.Add(_addedDto);
    }

    [Fact]
    public void CreatesViewModel()
    {
        _sut.Items.Should().ContainSingle();
    }

    [Fact]
    public void ViewModelHasCorrectName()
    {
        _sut.Items.Single().Name.Should().Be("TestItem");
    }

    [Fact]
    public void ViewModelReferencesCorrectModel()
    {
        _sut.Items.Single().Model.Should().BeSameAs(_addedDto);
    }

    [Fact]
    public void ItemsCountIsOne()
    {
        _sut.Items.Count.Should().Be(1);
    }

    [Fact]
    public void CountPropertyIsOne()
    {
        _sut.Count.Should().Be(1);
    }
}
