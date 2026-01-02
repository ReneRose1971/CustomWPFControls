using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Properties;

/// <summary>
/// Tests für Count Property - Initialer Wert (kein Act, nur Setup).
/// </summary>
/// <remarks>
/// Shared Setup: 3 Items im Store
/// Alle Tests prüfen den initialen Count-Wert nach dem Setup.
/// </remarks>
public sealed class CountProperty_InitialValue : IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly CollectionViewModel<TestDto, TestViewModel> _sut;

    public CountProperty_InitialValue()
    {
        _fixture = new CollectionViewModelFixture();
        
        // Shared Setup: 3 Items
        _fixture.TestDtoStore.AddRange(new[]
        {
            new TestDto { Name = "First" },
            new TestDto { Name = "Second" },
            new TestDto { Name = "Third" }
        });

        _sut = new CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);
    }

    [Fact]
    public void ReturnsCorrectValue()
    {
        // Assert
        _sut.Count.Should().Be(3);
    }

    [Fact]
    public void MatchesItemsCount()
    {
        // Assert
        _sut.Count.Should().Be(_sut.Items.Count);
    }

    public void Dispose()
    {
        _sut?.Dispose();
        _fixture?.Dispose();
    }
}
