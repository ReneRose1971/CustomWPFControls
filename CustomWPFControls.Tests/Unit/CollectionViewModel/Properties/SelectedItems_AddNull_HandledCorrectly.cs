using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Properties;

/// <summary>
/// Test: SelectedItems erlaubt Add(null) ohne Exception (Standard ObservableCollection-Verhalten).
/// </summary>
public sealed class SelectedItems_AddNull_HandledCorrectly : IClassFixture<TestHelperCustomWPFControlsTestFixture>
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;

    public SelectedItems_AddNull_HandledCorrectly(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Test_SelectedItems_AddNull_HandledCorrectly()
    {
        // Arrange
        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        // Act & Assert - ObservableCollection<T> erlaubt null ohne Exception
        var act = () => sut.SelectedItems.Add(null!);
        act.Should().NotThrow("ObservableCollection<T> erlaubt null-Werte");

        // Verify null wurde hinzugefügt
        sut.SelectedItems.Should().ContainSingle();
        sut.SelectedItems[0].Should().BeNull();

        // Cleanup
        _fixture.ClearTestData();
        sut.Dispose();
    }
}
