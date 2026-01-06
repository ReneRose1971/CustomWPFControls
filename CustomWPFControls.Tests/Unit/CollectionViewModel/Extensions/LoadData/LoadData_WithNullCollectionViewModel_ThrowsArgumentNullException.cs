using System;
using CustomWPFControls.Extensions;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Extensions.LoadData;

/// <summary>
/// Tests für LoadData-Extension mit null CollectionViewModel-Instanz.
/// </summary>
/// <remarks>
/// Szenario: LoadData wird auf null-Instanz aufgerufen.
/// ACT: LoadData(null collectionViewModel) im Test.
/// Validiert Pflicht-Parameter-Validierung.
/// </remarks>
public sealed class LoadData_WithNullCollectionViewModel_ThrowsArgumentNullException
{
    [Fact]
    public void ThrowsArgumentNullException()
    {
        // ACT & ASSERT
        var testData = new[] { new TestDto { Name = "Test" } };
        
        Action act = () => CollectionViewModelExtensions.LoadData<TestDto, TestViewModel>(
            null!, 
            testData);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("collectionViewModel");
    }
}
