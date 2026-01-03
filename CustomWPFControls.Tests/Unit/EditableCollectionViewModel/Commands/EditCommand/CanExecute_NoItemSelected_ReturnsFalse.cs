using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.EditableCollectionViewModel.Commands.EditCommand;

/// <summary>
/// Test: EditCommand.CanExecute() gibt false zurück wenn kein Item selektiert ist.
/// </summary>
public sealed class CanExecute_NoItemSelected_ReturnsFalse : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly EditableCollectionViewModel<TestDto, TestViewModel> _sut;

    public CanExecute_NoItemSelected_ReturnsFalse(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _sut = new EditableCollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        // Setup: EditModel setzen
        _sut.EditModel = model => { };

        // Setup: Kein Item selektieren
        _sut.SelectedItem = null;
    }

    [Fact]
    public void EditCommand_CanExecute_ReturnsFalse()
    {
        // Act & Assert
        Assert.False(_sut.EditCommand.CanExecute(null));
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
        _sut?.Dispose();
    }
}
