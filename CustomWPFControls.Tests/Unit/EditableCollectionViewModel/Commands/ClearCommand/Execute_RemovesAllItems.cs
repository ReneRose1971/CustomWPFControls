using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.EditableCollectionViewModel.Commands.ClearCommand;

/// <summary>
/// Test: ClearCommand.Execute() entfernt alle Items.
/// </summary>
public sealed class Execute_RemovesAllItems : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly EditableCollectionViewModel<TestDto, TestViewModel> _sut;

    public Execute_RemovesAllItems(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _sut = new EditableCollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        // Setup: Mehrere Items hinzufügen
        _fixture.TestDtoStore.AddRange(new[]
        {
            new TestDto { Name = "Item1" },
            new TestDto { Name = "Item2" },
            new TestDto { Name = "Item3" }
        });
    }

    [Fact]
    public void ClearCommand_Execute_RemovesAllItems()
    {
        // Act
        _sut.ClearCommand.Execute(null);

        // Assert
        Assert.Equal(0, _sut.Count);
        Assert.Empty(_sut.Items);
        Assert.Empty(_fixture.TestDtoStore.Items);
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
        _sut?.Dispose();
    }
}
