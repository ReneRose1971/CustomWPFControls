using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Properties;

/// <summary>
/// Tests für SelectedItem PropertyChanged - kein Event bei gleichem Wert.
/// </summary>
public sealed class SelectedItem_DoesNotRaisePropertyChanged_WhenSetToSameValue : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;

    public SelectedItem_DoesNotRaisePropertyChanged_WhenSetToSameValue(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
    }

    [Fact]
    public void ShouldNotRaiseEvent()
    {
        // Arrange
        _fixture.TestDtoStore.Add(new TestDto { Name = "Test" });

        var sut = new CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        var viewModel = sut.Items.Single();
        sut.SelectedItem = viewModel;

        int propertyChangedCount = 0;
        sut.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(sut.SelectedItem))
                propertyChangedCount++;
        };

        // Act - Setze auf den GLEICHEN Wert
        sut.SelectedItem = viewModel;

        // Assert - Kein Event sollte gefeuert werden
        propertyChangedCount.Should().Be(0);

        // Cleanup
        _fixture.ClearTestData();
        sut.Dispose();
    }
}
