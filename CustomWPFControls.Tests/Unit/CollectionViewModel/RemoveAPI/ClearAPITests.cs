using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.RemoveAPI;

public sealed class ClearAPITests : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;
    private readonly ViewModels.CollectionViewModel<TestDto, TestViewModel> _sut;

    public ClearAPITests(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        _sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);
        
        // Setup: 3 Items hinzufügen
        _fixture.TestDtoStore.AddRange(new[]
        {
            new TestDto { Name = "First" },
            new TestDto { Name = "Second" },
            new TestDto { Name = "Third" }
        });
    }

    [Fact]
    public void Clear_RemovesAllItems()
    {
        // Act
        _sut.Clear();

        // Assert
        _sut.Items.Should().BeEmpty();
        _sut.Count.Should().Be(0);
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
        _sut?.Dispose();
    }
}
