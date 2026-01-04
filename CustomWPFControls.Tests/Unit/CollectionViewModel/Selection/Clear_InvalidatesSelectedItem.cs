using System;
using System.Linq;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Selection;

public sealed class Clear_InvalidatesSelectedItem : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly ViewModels.CollectionViewModel<TestDto, TestViewModel> _sut;

    public Clear_InvalidatesSelectedItem(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);
        
        // Setup: Items hinzufügen
        _fixture.TestDtoStore.AddRange(new[]
        {
            new TestDto { Name = "First" },
            new TestDto { Name = "Second" }
        });
    }

    [Fact]
    public void Test_Clear_InvalidatesSelectedItem()
    {
        // Arrange
        _sut.SelectedItem = _sut.Items.First();

        // Act
        _sut.Clear();

        // Assert
        _sut.SelectedItem.Should().BeNull();
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
        _sut?.Dispose();
    }
}
