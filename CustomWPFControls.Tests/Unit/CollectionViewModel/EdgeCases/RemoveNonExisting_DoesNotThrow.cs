using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.EdgeCases;

/// <summary>
/// Tests für Remove-Operation mit nicht vorhandenem ViewModel.
/// </summary>
public sealed class RemoveNonExisting_DoesNotThrow : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;

    public RemoveNonExisting_DoesNotThrow(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
    }

    [Fact]
    public void ShouldNotThrowWhenRemovingNonExistingItem()
    {
        // Arrange
        _fixture.TestDtoStore.Add(new TestDto { Name = "Test" });

        var sut = new CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        // Erstelle ein ViewModel das NICHT in der Collection ist
        var nonExistingDto = new TestDto { Name = "NonExisting" };
        var nonExistingViewModel = _fixture.ViewModelFactory.Create(nonExistingDto);

        // Act & Assert - Sollte keine Exception werfen
        var act = () => sut.Remove(nonExistingViewModel);
        act.Should().NotThrow();

        // Cleanup
        _fixture.ClearTestData();
        sut.Dispose();
    }
}
