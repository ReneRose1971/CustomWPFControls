using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.EdgeCases;

/// <summary>
/// Tests für Remove-Rückgabewert bei nicht vorhandenem ViewModel.
/// </summary>
public sealed class RemoveNonExisting_ReturnsFalse : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;

    public RemoveNonExisting_ReturnsFalse(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
    }

    [Fact]
    public void ShouldReturnFalse()
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

        // Act
        var result = sut.Remove(nonExistingViewModel);

        // Assert
        result.Should().BeFalse();

        // Cleanup
        _fixture.ClearTestData();
        sut.Dispose();
    }
}
