using System;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Dispose;

/// <summary>
/// Test: Dispose() leert SelectedItems Collection.
/// </summary>
public sealed class Dispose_ClearsSelectedItems : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;
    private readonly ViewModels.CollectionViewModel<TestDto, TestViewModel> _sut;

    public Dispose_ClearsSelectedItems(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        
        // Setup: Neue unabhängige Instanz für Dispose-Test erstellen
        _sut = _fixture.CreateCollectionViewModel();
        
        // Setup: 2 Items zum Store hinzufügen
        _sut.ModelStore.AddRange(new[]
        {
            new TestDto { Name = "First" },
            new TestDto { Name = "Second" }
        });
        
        // Setup: Items zu SelectedItems hinzufügen
        _sut.SelectedItems.Add(_sut.Items[0]);
        _sut.SelectedItems.Add(_sut.Items[1]);
    }

    [Fact]
    public void Test_Dispose_ClearsSelectedItems()
    {
        // Arrange: Verify SelectedItems ist nicht leer
        Assert.Equal(2, _sut.SelectedItems.Count);

        // Act
        _sut.Dispose();

        // Assert
        Assert.Empty(_sut.SelectedItems);
    }

    public void Dispose()
    {
        // Nichts zu tun - _sut wurde bereits disposed
    }
}
