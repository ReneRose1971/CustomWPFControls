using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.EditableCollectionViewModel.Commands.AddCommand;

/// <summary>
/// Test: AddCommand.Execute() mit gültigem CreateModel fügt Model zum Store hinzu.
/// </summary>
public sealed class Execute_WithValidCreateModel_AddsModelToStore : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly EditableCollectionViewModel<TestDto, TestViewModel> _sut;

    public Execute_WithValidCreateModel_AddsModelToStore(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
        
        _sut = _fixture.CreateEditableCollectionViewModel();
        
        // Setup: CreateModel Property setzen (erstellt neues Model)
        _sut.CreateModel = () => new TestDto { Name = "NewItem" };
    }

    [Fact]
    public void AddCommand_Execute_AddsModelToStore()
    {
        // Act
        _sut.AddCommand.Execute(null);

        // Assert: AddCommand verwendet jetzt den lokalen ModelStore
        Assert.Equal(1, _sut.Count);
        Assert.Single(_sut.Items);
        Assert.Equal("NewItem", _sut.Items[0].Name);
    }

    public void Dispose()
    {
        _sut?.Dispose();
    }
}
