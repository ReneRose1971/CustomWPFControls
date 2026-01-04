using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.EditableCollectionViewModel.Commands.DeleteCommand;

public sealed class CanExecute_ItemIsSelected_ReturnsTrue : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly EditableCollectionViewModel<TestDto, TestViewModel> _sut;

    public CanExecute_ItemIsSelected_ReturnsTrue(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
        
        _sut = _fixture.CreateEditableCollectionViewModel();

        // Setup: Item hinzufügen und selektieren
        var model = new TestDto { Name = "Test" };
        _sut.ModelStore.Add(model);
        _sut.SelectedItem = _sut.Items[0];
    }

    [Fact]
    public void DeleteCommand_CanExecute_ReturnsTrue()
    {
        // Act & Assert
        Assert.True(_sut.DeleteCommand.CanExecute(null));
    }

    public void Dispose()
    {
        _sut?.Dispose();
    }
}
