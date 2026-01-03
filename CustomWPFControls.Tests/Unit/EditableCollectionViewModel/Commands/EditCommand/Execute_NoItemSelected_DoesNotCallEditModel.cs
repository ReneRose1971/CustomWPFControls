using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.EditableCollectionViewModel.Commands.EditCommand;

/// <summary>
/// Test: EditCommand.Execute() ruft EditModel nicht auf wenn kein Item selektiert ist.
/// </summary>
public sealed class Execute_NoItemSelected_DoesNotCallEditModel : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly EditableCollectionViewModel<TestDto, TestViewModel> _sut;
    private bool _delegateCalled;

    public Execute_NoItemSelected_DoesNotCallEditModel(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _sut = new EditableCollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);

        // Setup: EditModel-Delegate setzen
        _sut.EditModel = model => _delegateCalled = true;

        // Setup: Item hinzufügen aber NICHT selektieren
        var model = new TestDto { Name = "Test" };
        _fixture.TestDtoStore.Add(model);
        _sut.SelectedItem = null;
    }

    [Fact]
    public void EditCommand_Execute_DoesNotCallEditModel()
    {
        // Act
        _sut.EditCommand.Execute(null);

        // Assert
        Assert.False(_delegateCalled);
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
        _sut?.Dispose();
    }
}
