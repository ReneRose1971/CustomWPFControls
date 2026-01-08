using System;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Constructor;

/// <summary>
/// Szenario: CollectionViewModel wurde frisch erstellt (keine Items im Store).
/// </summary>
/// <remarks>
/// Testet den Initialzustand einer neuen CollectionViewModel-Instanz.
/// Alle Tests prüfen verschiedene Aspekte des leeren Zustands.
/// </remarks>
public sealed class CollectionViewModel_Constructor_CreatesEmptyCollection : IClassFixture<TestHelperCustomWPFControlsTestFixture>, IDisposable
{
    private readonly TestHelperCustomWPFControlsTestFixture _fixture;

    public CollectionViewModel_Constructor_CreatesEmptyCollection(TestHelperCustomWPFControlsTestFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
    }

    [Fact]
    public void Items_IsNotNull()
    {
        // Assert
        _fixture.Sut.Items.Should().NotBeNull();
    }

    [Fact]
    public void Count_IsZero()
    {
        // Assert
        _fixture.Sut.Count.Should().Be(0);
    }

    [Fact]
    public void Items_IsEmpty()
    {
        // Assert
        _fixture.Sut.Items.Should().BeEmpty();
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
