using System;
using CustomWPFControls.Tests.Testing;
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
public sealed class RemoveSingleTests : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;
    private readonly TestDto _removedDto;
    private readonly TestDto[] _remainingDtos;

    public RemoveSingleTests(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();

        // Setup: 3 Items zum lokalen ModelStore hinzufügen
        var dto1 = new TestDto { Name = "First" };
        var dto2 = new TestDto { Name = "Second" };
        var dto3 = new TestDto { Name = "Third" };
        _fixture.Sut.ModelStore.AddRange(new[] { dto1, dto2, dto3 });

        // ACT: Mittleres Item entfernen
        _removedDto = dto2;
        _remainingDtos = new[] { dto1, dto3 };
        _fixture.Sut.ModelStore.Remove(dto2);
    }

    [Fact]
    public void RemovesViewModel()
    {
        _fixture.Sut.Items.Should().NotContain(vm => vm.Model == _removedDto);
    }

    [Fact]
    public void CountIsTwo()
    {
        _fixture.Sut.Count.Should().Be(2);
    }

    [Fact]
    public void RemainingViewModelsCorrect()
    {
        _fixture.Sut.Items.Should().Contain(vm => vm.Name == "First");
        _fixture.Sut.Items.Should().Contain(vm => vm.Name == "Third");
    }

    [Fact]
    public void RemovedModelNotInItems()
    {
        _fixture.Sut.Items.Should().NotContain(vm => vm.Name == "Second");
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
