using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.EditableCollectionViewModel.Commands.EditCommand;

public sealed class Execute_WithSelectedItem_CallsEditModelDelegate : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;
    private readonly EditableCollectionViewModel<TestDto, TestViewModel> _sut;
    private TestDto? _editedModel;

    public Execute_WithSelectedItem_CallsEditModelDelegate(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
        
        _sut = _fixture.CreateEditableCollectionViewModel();

        // Setup: EditModel-Delegate setzen
        _sut.EditModel = model => _editedModel = model;

        // Setup: Item hinzufügen und selektieren
        var model = new TestDto { Name = "ToEdit" };
        _sut.ModelStore.Add(model);
        _sut.SelectedItem = _sut.Items[0];
    }

    [Fact]
    public void EditCommand_Execute_CallsEditModelDelegate()
    {
        // Arrange
        var expectedModel = _sut.SelectedItem!.Model;

        // Act
        _sut.EditCommand.Execute(null);

        // Assert
        Assert.NotNull(_editedModel);
        Assert.Same(expectedModel, _editedModel);
    }

    public void Dispose()
    {
        _sut?.Dispose();
    }
}
