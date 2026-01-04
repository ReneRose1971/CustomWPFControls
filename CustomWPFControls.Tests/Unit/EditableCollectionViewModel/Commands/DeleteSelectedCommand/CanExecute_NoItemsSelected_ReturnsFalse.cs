using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.EditableCollectionViewModel.Commands.DeleteSelectedCommand;

/// <summary>
/// Test: DeleteSelectedCommand.CanExecute() gibt false zurück wenn keine Items selektiert sind.
/// </summary>
public sealed class CanExecute_NoItemsSelected_ReturnsFalse : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly EditableCollectionViewModel<TestDto, TestViewModel> _sut;

    public CanExecute_NoItemsSelected_ReturnsFalse(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _sut = new EditableCollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);
    }

    [Fact]
    public void DeleteSelectedCommand_CanExecute_ReturnsFalse()
    {
        // Act & Assert
        Assert.False(_sut.DeleteSelectedCommand.CanExecute(null));
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
        _sut?.Dispose();
    }
}
