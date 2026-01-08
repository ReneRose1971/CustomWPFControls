using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.EditableCollectionViewModel.Commands.EditCommand;

/// <summary>
/// Test: EditCommand.Execute() ruft EditModel nicht auf wenn kein Item selektiert ist.
/// </summary>
public sealed class Execute_NoItemSelected_DoesNotCallEditModel : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;
    private readonly EditableCollectionViewModel<TestDto, TestViewModel> _sut;
    private bool _delegateCalled;

    public Execute_NoItemSelected_DoesNotCallEditModel(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        _sut = new EditableCollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);
        
        _sut.EditModel = _ => _delegateCalled = true;
    }

    [Fact]
    public void Test_Execute_NoItemSelected_DoesNotCallEditModel()
    {
        // Arrange - kein Item selektiert
        _sut.SelectedItem = null;

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
