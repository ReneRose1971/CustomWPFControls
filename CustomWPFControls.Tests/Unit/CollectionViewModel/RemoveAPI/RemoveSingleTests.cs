using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.RemoveAPI;

/// <summary>
/// Tests für die Remove(viewModel) Public API.
/// </summary>
/// <remarks>
/// Setup: 3 Items hinzufügen
/// ACT: sut.Remove(viewModel)
/// </remarks>
public sealed class RemoveSingleTests : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly ViewModels.CollectionViewModel<TestDto, TestViewModel> _sut;
    private readonly TestViewModel _removedViewModel;
    private readonly bool _removeResult;

    public RemoveSingleTests(CollectionViewModelFixture fixture)
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

        _sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        // ACT: Mittleres ViewModel entfernen
        _removedViewModel = _sut.Items[1];
        _removeResult = _sut.Remove(_removedViewModel);
    }

    [Fact]
    public void RemovesViewModel()
    {
        _sut.Items.Should().NotContain(_removedViewModel);
    }

    [Fact]
    public void RemovesModelFromStore()
    {
        _fixture.TestDtoStore.Items.Should().NotContain(_removedViewModel.Model);
    }

    [Fact]
    public void CountDecrements()
    {
        _sut.Count.Should().Be(2);
    }

    [Fact]
    public void ReturnsTrueOnSuccess()
    {
        _removeResult.Should().BeTrue();
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }
}
