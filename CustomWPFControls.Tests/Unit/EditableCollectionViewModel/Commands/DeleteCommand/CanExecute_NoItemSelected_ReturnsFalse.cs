using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.EditableCollectionViewModel.Commands.DeleteCommand;

/// <summary>
/// Test: DeleteCommand.CanExecute() gibt false zurück wenn kein Item selektiert ist.
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

        // Setup: Item hinzufügen aber NICHT selektieren
        var model = new TestDto { Name = "Test" };
        _fixture.TestDtoStore.Add(model);
        _sut.SelectedItem = null;
    }

    [Fact]
    public void DeleteCommand_CanExecute_ReturnsFalse()
    {
        // Act & Assert
        Assert.False(_sut.DeleteCommand.CanExecute(null));
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
        _sut?.Dispose();
    }
}
