using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.EditableCollectionViewModel.Commands.ClearCommand;

/// <summary>
/// Test: ClearCommand.CanExecute() gibt true zurück wenn Items existieren.
/// </summary>
public sealed class CanExecute_ItemsExist_ReturnsTrue : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly EditableCollectionViewModel<TestDto, TestViewModel> _sut;

    public CanExecute_ItemsExist_ReturnsTrue(CollectionViewModelFixture fixture)
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
    }

    [Fact]
    public void ClearCommand_CanExecute_ReturnsTrue()
    {
        // Act & Assert
        Assert.True(_sut.ClearCommand.CanExecute(null));
    }

    public void Dispose()
    {
        _sut?.Dispose();
    }
}
