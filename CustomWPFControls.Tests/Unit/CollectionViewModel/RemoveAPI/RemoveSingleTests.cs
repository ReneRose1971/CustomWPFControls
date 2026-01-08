using System;
using CustomWPFControls.Tests.Testing;
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
public sealed class RemoveSingleTests : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;
    private readonly TestViewModel _removedViewModel;
    private readonly bool _removeResult;

    public RemoveSingleTests(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();

        // Setup: 3 Items hinzufügen
        _fixture.Sut.ModelStore.AddRange(new[]
        {
            new TestDto { Name = "First" },
            new TestDto { Name = "Second" },
            new TestDto { Name = "Third" }
        });

        // ACT: Mittleres ViewModel entfernen
        _removedViewModel = _fixture.Sut.Items[1];
        _removeResult = _fixture.Sut.Remove(_removedViewModel);
    }

    [Fact]
    public void RemovesViewModel()
    {
        _fixture.Sut.Items.Should().NotContain(_removedViewModel);
    }

    [Fact]
    public void RemovesModelFromStore()
    {
        _fixture.Sut.ModelStore.Items.Should().NotContain(_removedViewModel.Model);
    }

    [Fact]
    public void CountDecrements()
    {
        _fixture.Sut.Count.Should().Be(2);
    }

    [Fact]
    public void ReturnsTrueOnSuccess()
    {
        _removeResult.Should().BeTrue();
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
