using System;
using System.Linq;
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
public sealed class SingleAddTests : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly TestDto _addedDto;

    public SingleAddTests(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();

        // ACT: Einzelnes TestDto zum lokalen ModelStore hinzufügen
        _addedDto = new TestDto { Name = "TestItem" };
        _fixture.Sut.ModelStore.Add(_addedDto);
    }

    [Fact]
    public void CreatesViewModel()
    {
        _fixture.Sut.Items.Should().ContainSingle();
    }

    [Fact]
    public void ViewModelHasCorrectName()
    {
        _fixture.Sut.Items.Single().Name.Should().Be("TestItem");
    }

    [Fact]
    public void ViewModelReferencesCorrectModel()
    {
        _fixture.Sut.Items.Single().Model.Should().BeSameAs(_addedDto);
    }

    [Fact]
    public void ItemsCountIsOne()
    {
        _fixture.Sut.Items.Count.Should().Be(1);
    }

    [Fact]
    public void CountPropertyIsOne()
    {
        _fixture.Sut.Count.Should().Be(1);
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
