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
    private readonly TestDto[] _addedDtos;

    public BulkAddTests(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();

        // ACT: Mehrere TestDtos zum lokalen ModelStore hinzufügen
        _addedDtos = new[]
        {
            new TestDto { Name = "First" },
            new TestDto { Name = "Second" },
            new TestDto { Name = "Third" }
        };
        _fixture.Sut.ModelStore.AddRange(_addedDtos);
    }

    [Fact]
    public void CreatesMultipleViewModels()
    {
        _fixture.Sut.Items.Should().HaveCount(3);
    }

    [Fact]
    public void CountIsThree()
    {
        _fixture.Sut.Count.Should().Be(3);
    }

    [Fact]
    public void ViewModelsHaveCorrectNames()
    {
        _fixture.Sut.Items.Should().Contain(vm => vm.Name == "First");
        _fixture.Sut.Items.Should().Contain(vm => vm.Name == "Second");
        _fixture.Sut.Items.Should().Contain(vm => vm.Name == "Third");
    }

    [Fact]
    public void ViewModelsAreInCorrectOrder()
    {
        _fixture.Sut.Items[0].Name.Should().Be("First");
        _fixture.Sut.Items[1].Name.Should().Be("Second");
        _fixture.Sut.Items[2].Name.Should().Be("Third");
    }

    [Fact]
    public void AllViewModelsReferenceCorrectModels()
    {
        for (int i = 0; i < _addedDtos.Length; i++)
        {
            _fixture.Sut.Items[i].Model.Should().BeSameAs(_addedDtos[i]);
        }
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
