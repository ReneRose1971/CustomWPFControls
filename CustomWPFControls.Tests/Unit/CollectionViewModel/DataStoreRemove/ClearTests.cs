using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.DataStoreRemove;

/// <summary>
/// Tests für das Leeren des DataStores via Clear().
/// </summary>
public sealed class ClearTests : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly ViewModels.CollectionViewModel<TestDto, TestViewModel> _sut;

    public ClearTests(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);
        
        // Setup: 3 Items hinzufügen
        _fixture.TestDtoStore.AddRange(new[]
        {
            new TestDto { Name = "First" },
            new TestDto { Name = "Second" },
            new TestDto { Name = "Third" }
        });

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

    public void Dispose()
    {
        _fixture.ClearTestData();
        _sut?.Dispose();
    }
}
