using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Properties;

/// <summary>
/// Tests für Count Property - Nach Remove-Operation.
/// </summary>
/// <remarks>
/// Setup: 3 Items
/// Act: 1 Item entfernen
/// Assert: Count == 2
/// </remarks>
public sealed class CountProperty_AfterRemove : IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly CollectionViewModel<TestDto, TestViewModel> _sut;
    private readonly TestDto[] _dtos;

    public CountProperty_AfterRemove()
    {
        _fixture = new CollectionViewModelFixture();
        
        // Setup: 3 Items
        _dtos = new[]
        {
            new TestDto { Name = "First" },
            new TestDto { Name = "Second" },
            new TestDto { Name = "Third" }
        };
        _fixture.TestDtoStore.AddRange(_dtos);

        _sut = new CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);
    }

    [Fact]
    public void UpdatesCorrectly()
    {
        // Act
        _fixture.TestDtoStore.Remove(_dtos[1]);

        // Assert
        _sut.Count.Should().Be(2);
    }

    public void Dispose()
    {
        _sut?.Dispose();
        _fixture?.Dispose();
    }
}
