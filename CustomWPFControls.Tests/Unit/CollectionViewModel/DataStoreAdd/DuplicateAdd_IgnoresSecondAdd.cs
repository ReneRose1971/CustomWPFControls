using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.DataStoreAdd;

/// <summary>
/// Tests für Duplikat-Erkennung beim Hinzufügen.
/// </summary>
/// <remarks>
/// Das zweite Add des gleichen DTOs wirft eine Exception.
/// </remarks>
public sealed class DuplicateAdd_ThrowsException : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;

    public DuplicateAdd_ThrowsException(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
    }

    [Fact]
    public void ShouldThrowInvalidOperationException()
    {
        // Arrange
        var sut = new CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        var dto = new TestDto { Name = "Test" };
        _fixture.TestDtoStore.Add(dto);

        // Act & Assert - Zweites Add sollte Exception werfen
        var act = () => _fixture.TestDtoStore.Add(dto);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Duplicate item rejected*");

        // Cleanup
        _fixture.ClearTestData();
        sut.Dispose();
    }
}
