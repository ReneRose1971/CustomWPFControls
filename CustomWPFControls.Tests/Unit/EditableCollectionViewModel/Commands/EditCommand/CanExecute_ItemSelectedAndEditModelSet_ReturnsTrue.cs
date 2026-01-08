using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.EditableCollectionViewModel.Commands.EditCommand;

/// <summary>
/// Test: EditCommand.CanExecute() gibt true zurück wenn Item selektiert und EditModel gesetzt ist.
/// </summary>
public sealed class CanExecute_ItemSelectedAndEditModelSet_ReturnsTrue : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;
    private readonly EditableCollectionViewModel<TestDto, TestViewModel> _sut;

    public CanExecute_ItemSelectedAndEditModelSet_ReturnsTrue(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
        
        _sut = _fixture.CreateEditableCollectionViewModel();

        // Setup: EditModel setzen
        _sut.EditModel = model => { };

        // Setup: Item hinzufügen und selektieren
        var model = new TestDto { Name = "Test" };
        _sut.ModelStore.Add(model);
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
        _sut?.Dispose();
    }
}
