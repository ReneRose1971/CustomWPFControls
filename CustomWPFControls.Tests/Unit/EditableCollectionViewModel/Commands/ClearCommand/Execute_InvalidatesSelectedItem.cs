using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.EditableCollectionViewModel.Commands.ClearCommand;

/// <summary>
/// Test: ClearCommand.Execute() invalidiert SelectedItem.
/// </summary>
public sealed class Execute_InvalidatesSelectedItem : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly EditableCollectionViewModel<TestDto, TestViewModel> _sut;

    public Execute_InvalidatesSelectedItem(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _sut = new EditableCollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);
    }

    [Fact]
    public void ClearCommand_Execute_InvalidatesSelectedItem()
    {
        // Act
        _sut.ClearCommand.Execute(null);

        // Assert
        Assert.Null(_sut.SelectedItem);
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
        _sut?.Dispose();
    }
}
