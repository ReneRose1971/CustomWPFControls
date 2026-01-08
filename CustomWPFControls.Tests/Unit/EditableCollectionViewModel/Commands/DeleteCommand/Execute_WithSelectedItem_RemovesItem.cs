using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.EditableCollectionViewModel.Commands.DeleteCommand;

public sealed class Execute_WithSelectedItem_RemovesItem : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;
    private readonly EditableCollectionViewModel<TestDto, TestViewModel> _sut;

    public Execute_WithSelectedItem_RemovesItem(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
        
        _sut = _fixture.CreateEditableCollectionViewModel();

        // Setup: Item hinzufügen und selektieren
        var model = new TestDto { Name = "ToDelete" };
        _sut.ModelStore.Add(model);
        _sut.SelectedItem = _sut.Items[0];
    }

    [Fact]
    public void DeleteCommand_Execute_RemovesSelectedItem()
    {
        // Act
        _sut.DeleteCommand.Execute(null);

        // Assert
        Assert.Equal(0, _sut.Count);
        Assert.Empty(_sut.Items);
        Assert.Null(_sut.SelectedItem);
    }

    public void Dispose()
    {
        _sut?.Dispose();
    }
}
