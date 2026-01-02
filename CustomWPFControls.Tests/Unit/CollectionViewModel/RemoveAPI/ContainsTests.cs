using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.RemoveAPI;

/// <summary>
/// Tests für die Contains() Public API.
/// </summary>
/// <remarks>
/// Setup: 1 Item hinzufügen
/// </remarks>
public sealed class ContainsTests : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly CollectionViewModel<TestDto, TestViewModel> _sut;
    private readonly TestViewModel _existingViewModel;

    public ContainsTests(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();

        // Setup: 1 Item hinzufügen
        _fixture.TestDtoStore.Add(new TestDto { Name = "Test" });

        _sut = new CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        _existingViewModel = _sut.Items.Single();
    }

    [Fact]
    public void ReturnsTrueForExistingItem()
    {
        _sut.Contains(_existingViewModel).Should().BeTrue();
    }

    [Fact]
    public void ReturnsFalseForNonExistingItem()
    {
        // Erstelle ein ViewModel das NICHT in der Collection ist
        var nonExistingDto = new TestDto { Name = "NonExisting" };
        var nonExistingViewModel = _fixture.ViewModelFactory.Create(nonExistingDto);

        _sut.Contains(nonExistingViewModel).Should().BeFalse();

        // Kein Dispose nötig - TestViewModel hat kein IDisposable
    }

    [Fact]
    public void ReturnsFalseForNull()
    {
        _sut.Contains(null!).Should().BeFalse();
    }
}
