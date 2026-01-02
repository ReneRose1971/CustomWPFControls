using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Properties;

/// <summary>
/// Tests für Count Property - Nach Add-Operation.
/// </summary>
/// <remarks>
/// Setup: 3 Items
/// Act: 1 Item hinzufügen
/// Assert: Count == 4
/// </remarks>
public sealed class CountProperty_AfterAdd : IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly CollectionViewModel<TestDto, TestViewModel> _sut;

    public CountProperty_AfterAdd()
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
    public void UpdatesCorrectly()
    {
        // Act
        _fixture.TestDtoStore.Add(new TestDto { Name = "Fourth" });

        // Assert
        _sut.Count.Should().Be(4);
    }

    public void Dispose()
    {
        _sut?.Dispose();
        _fixture?.Dispose();
    }
}
