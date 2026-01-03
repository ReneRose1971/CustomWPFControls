using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.EditableCollectionViewModel.Commands.EditCommand;

/// <summary>
/// Test: EditCommand.CanExecute() gibt true zurück wenn Item selektiert und EditModel gesetzt ist.
/// </summary>
public sealed class CanExecute_ItemSelectedAndEditModelSet_ReturnsTrue : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly EditableCollectionViewModel<TestDto, TestViewModel> _sut;

    public CanExecute_ItemSelectedAndEditModelSet_ReturnsTrue(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _sut = new EditableCollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        // Setup: EditModel setzen
        _sut.EditModel = model => { };

        // Setup: Item hinzufügen und selektieren
        var model = new TestDto { Name = "Test" };
        _fixture.TestDtoStore.Add(model);
        _sut.SelectedItem = _sut.Items[0];
    }

    [Fact]
    public void EditCommand_CanExecute_ReturnsTrue()
    {
        // Act & Assert
        Assert.True(_sut.EditCommand.CanExecute(null));
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
        _sut?.Dispose();
    }
}
