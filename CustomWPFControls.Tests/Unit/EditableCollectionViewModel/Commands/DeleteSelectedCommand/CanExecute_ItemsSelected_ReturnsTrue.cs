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
        _sut = new EditableCollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        // Setup: Items hinzufügen und selektieren
        _fixture.TestDtoStore.AddRange(new[]
        {
            new TestDto { Name = "Item1" },
            new TestDto { Name = "Item2" }
        });
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
        _fixture.ClearTestData();
        _sut?.Dispose();
    }
}
