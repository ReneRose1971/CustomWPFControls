using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.DataStoreAdd;

/// <summary>
/// Tests für das Hinzufügen mehrerer Items per AddRange.
/// </summary>
/// <remarks>
/// ACT: fixture.TestDtoStore.AddRange([3 DTOs]) im Constructor
/// Mehrere Assertions testen verschiedene Aspekte dieser EINEN Operation.
/// </remarks>
public sealed class BulkAddTests : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly ViewModels.CollectionViewModel<TestDto, TestViewModel> _sut;
    private readonly TestDto[] _addedDtos;

    public BulkAddTests(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();

        // SUT erstellen mit leerem Store
        _sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        // ACT: Mehrere TestDtos hinzufügen
        _addedDtos = new[]
        {
            new TestDto { Name = "First" },
            new TestDto { Name = "Second" },
            new TestDto { Name = "Third" }
        };
        _fixture.TestDtoStore.AddRange(_addedDtos);
    }

    [Fact]
    public void CreatesMultipleViewModels()
    {
        _sut.Items.Should().HaveCount(3);
    }

    [Fact]
    public void CountIsThree()
    {
        _sut.Count.Should().Be(3);
    }

    [Fact]
    public void ViewModelsHaveCorrectNames()
    {
        _sut.Items.Should().Contain(vm => vm.Name == "First");
        _sut.Items.Should().Contain(vm => vm.Name == "Second");
        _sut.Items.Should().Contain(vm => vm.Name == "Third");
    }

    [Fact]
    public void ViewModelsAreInCorrectOrder()
    {
        _sut.Items[0].Name.Should().Be("First");
        _sut.Items[1].Name.Should().Be("Second");
        _sut.Items[2].Name.Should().Be("Third");
    }

    [Fact]
    public void AllViewModelsReferenceCorrectModels()
    {
        for (int i = 0; i < _addedDtos.Length; i++)
        {
            _sut.Items[i].Model.Should().BeSameAs(_addedDtos[i]);
        }
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }
}
