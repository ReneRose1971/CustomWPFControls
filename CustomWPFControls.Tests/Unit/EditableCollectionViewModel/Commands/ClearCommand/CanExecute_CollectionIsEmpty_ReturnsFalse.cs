using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.EditableCollectionViewModel.Commands.ClearCommand;

/// <summary>
/// Test: ClearCommand.CanExecute() gibt false zurück wenn Collection leer ist.
/// </summary>
public sealed class CanExecute_CollectionIsEmpty_ReturnsFalse : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly EditableCollectionViewModel<TestDto, TestViewModel> _sut;

    public CanExecute_CollectionIsEmpty_ReturnsFalse(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _sut = new EditableCollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);
    }

    [Fact]
    public void ClearCommand_CanExecute_ReturnsFalse()
    {
        // Act & Assert
        Assert.False(_sut.ClearCommand.CanExecute(null));
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
        _sut?.Dispose();
    }
}
