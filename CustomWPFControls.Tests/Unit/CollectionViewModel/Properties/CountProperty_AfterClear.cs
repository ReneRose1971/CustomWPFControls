using System;
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
public sealed class CountProperty_AfterClear : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly ViewModels.CollectionViewModel<TestDto, TestViewModel> _sut;

    public CountProperty_AfterClear(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);
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
