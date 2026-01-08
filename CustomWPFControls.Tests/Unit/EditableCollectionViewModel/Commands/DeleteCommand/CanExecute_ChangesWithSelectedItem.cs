using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.EditableCollectionViewModel.Commands.DeleteCommand;

/// <summary>
/// Szenario: DeleteCommand CanExecute ändert sich basierend auf SelectedItem.
/// Setup: ViewModel mit einem Item.
/// Test: CanExecute gibt false zurück wenn SelectedItem null ist, true wenn gesetzt.
/// </summary>
public sealed class CanExecute_ChangesWithSelectedItem : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;
    private readonly EditableCollectionViewModel<TestDto, TestViewModel> _sut;
    private readonly TestDto _testDto;

    public CanExecute_ChangesWithSelectedItem(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        
        // Eigene ViewModel-Instanz für vollständige Test-Isolation
        _sut = new EditableCollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        // Setup: Ein Item hinzufügen
        _testDto = new TestDto { Name = "TestItem" };
        _sut.ModelStore.Add(_testDto);
    }

    [Fact]
    public void CanExecute_ReturnsFalse_WhenSelectedItemIsNull()
    {
        // Arrange
        _sut.SelectedItem = null;

        // Act & Assert
        Assert.False(_sut.DeleteCommand.CanExecute(null));
    }

    [Fact]
    public void CanExecute_ReturnsTrue_WhenSelectedItemIsSet()
    {
        // Arrange
        _sut.SelectedItem = _sut.Items[0];

        // Act & Assert
        Assert.True(_sut.DeleteCommand.CanExecute(null));
    }

    [Fact]
    public void CanExecute_ReturnsFalse_AfterSettingSelectedItemToNull()
    {
        // Arrange
        _sut.SelectedItem = _sut.Items[0];

        // Act
        _sut.SelectedItem = null;

        // Assert
        Assert.False(_sut.DeleteCommand.CanExecute(null));
    }

    public void Dispose()
    {
        _sut?.Dispose();
    }
}
