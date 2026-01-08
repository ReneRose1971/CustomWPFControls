using System;
using System.Linq;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Selection;

public sealed class Clear_InvalidatesSelectedItem : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;

    public Clear_InvalidatesSelectedItem(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
        
        // Setup: Items hinzufügen
        _fixture.Sut.ModelStore.AddRange(new[]
        {
            new TestDto { Name = "First" },
            new TestDto { Name = "Second" }
        });
    }

    [Fact]
    public void Test_Clear_InvalidatesSelectedItem()
    {
        // Arrange
        _fixture.Sut.SelectedItem = _fixture.Sut.Items.First();

        // Act
        _fixture.Sut.Clear();

        // Assert
        _fixture.Sut.SelectedItem.Should().BeNull();
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
