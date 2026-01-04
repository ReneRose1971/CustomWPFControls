using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.EditableCollectionViewModel.Commands.DeleteSelectedCommand;

/// <summary>
/// Test: DeleteSelectedCommand.CanExecute() gibt true zurück wenn Items selektiert sind.
/// </summary>
public sealed class CanExecute_ItemsSelected_ReturnsTrue : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly EditableCollectionViewModel<TestDto, TestViewModel> _sut;

    public CanExecute_ItemsSelected_ReturnsTrue(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
        
        _sut = _fixture.CreateEditableCollectionViewModel();
        
        // Setup: Items zum Store hinzufügen
        _sut.ModelStore.AddRange(new[]
        {
            new TestDto { Name = "First" },
            new TestDto { Name = "Second" }
        });
        
        // Setup: Items zu SelectedItems hinzufügen
        _sut.SelectedItems.Add(_sut.Items[0]);
    }

    [Fact]
    public void DeleteSelectedCommand_CanExecute_ReturnsTrue()
    {
        // Act & Assert
        Assert.True(_sut.DeleteSelectedCommand.CanExecute(null));
    }

    public void Dispose()
    {
        _sut?.Dispose();
    }
}
