using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.EditableCollectionViewModel.Commands.DeleteSelectedCommand;

/// <summary>
/// Test: DeleteSelectedCommand.Execute() entfernt alle selektierten Items.
/// </summary>
public sealed class Execute_WithMultipleSelectedItems_RemovesAllSelected : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;
    private readonly EditableCollectionViewModel<TestDto, TestViewModel> _sut;

    public Execute_WithMultipleSelectedItems_RemovesAllSelected(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
        
        _sut = _fixture.CreateEditableCollectionViewModel();

        // Setup: Mehrere Items hinzufügen
        _sut.ModelStore.AddRange(new[]
        {
            new TestDto { Name = "Item1" },
            new TestDto { Name = "Item2" },
            new TestDto { Name = "Item3" }
        });

        // Setup: Zwei Items in SelectedItems hinzufügen
        _sut.SelectedItems.Add(_sut.Items[0]);
        _sut.SelectedItems.Add(_sut.Items[1]);
    }

    [Fact]
    public void DeleteSelectedCommand_Execute_RemovesAllSelectedItems()
    {
        // Act
        _sut.DeleteSelectedCommand.Execute(null);

        // Assert
        Assert.Equal(1, _sut.Count);
        Assert.Single(_sut.Items);
        Assert.Equal("Item3", _sut.Items[0].Name);
    }

    public void Dispose()
    {
        _sut?.Dispose();
    }
}
