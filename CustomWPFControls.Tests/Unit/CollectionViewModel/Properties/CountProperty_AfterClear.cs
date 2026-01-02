using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Properties;

/// <summary>
/// Tests für Count Property - Nach Clear-Operation.
/// </summary>
/// <remarks>
/// Setup: 3 Items
/// Act: Store clearen
/// Assert: Count == 0
/// </remarks>
public sealed class CountProperty_AfterClear : IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly CollectionViewModel<TestDto, TestViewModel> _sut;

    public CountProperty_AfterClear()
    {
        _fixture = new CollectionViewModelFixture();
        
        // Setup: 3 Items
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
    public void BecomesZero()
    {
        // Act
        _fixture.TestDtoStore.Clear();

        // Assert
        _sut.Count.Should().Be(0);
    }

    public void Dispose()
    {
        _sut?.Dispose();
        _fixture?.Dispose();
    }
}
