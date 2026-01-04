using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.EditableCollectionViewModel.Commands.AddCommand;

/// <summary>
/// Test: AddCommand.Execute() fügt nichts hinzu wenn CreateModel null zurückgibt.
/// </summary>
public sealed class Execute_CreateModelReturnsNull_DoesNotAdd : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly EditableCollectionViewModel<TestDto, TestViewModel> _sut;

    public Execute_CreateModelReturnsNull_DoesNotAdd(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _sut = new EditableCollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);
    }

    [Fact]
    public void AddCommand_Execute_DoesNotAddNull()
    {
        // Act
        _sut.AddCommand.Execute(null);

        // Assert
        Assert.Equal(0, _sut.Count);
        Assert.Empty(_sut.Items);
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
        _sut?.Dispose();
    }
}
