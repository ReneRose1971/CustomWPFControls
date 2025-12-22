using System;
using CustomWPFControls.Services;
using DataStores.Abstractions;
using Moq;
using Xunit;
using FluentAssertions;

namespace CustomWPFControls.Tests.Unit;

/// <summary>
/// Unit-Tests für <see cref="WindowLayoutService"/>.
/// Fokus: Isolierte Tests der Kernlogik ohne Window-Objekte.
/// </summary>
public sealed class WindowLayoutServiceTests : IDisposable
{
    private readonly Mock<IDataStores> _mockDataStores;
    private readonly Mock<IDataStore<WindowLayoutData>> _mockStore;
    private readonly WindowLayoutService _sut;

    public WindowLayoutServiceTests()
    {
        _mockDataStores = new Mock<IDataStores>();
        _mockStore = new Mock<IDataStore<WindowLayoutData>>();
        
        // Leere Items-Collection
        _mockStore.Setup(s => s.Items).Returns(new System.Collections.ObjectModel.ReadOnlyCollection<WindowLayoutData>(new WindowLayoutData[0]));

        // IDataStores gibt den gemockten Store zurück
        _mockDataStores
            .Setup(ds => ds.GetGlobal<WindowLayoutData>())
            .Returns(_mockStore.Object);

        _sut = new WindowLayoutService(_mockDataStores.Object);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenDataStoresIsNull()
    {
        // Act
        Action act = () => new WindowLayoutService(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("dataStores");
    }

    [Fact]
    public void Constructor_ShouldRequestGlobalDataStore()
    {
        // Assert - Verify that GetGlobal was called
        _mockDataStores.Verify(ds => ds.GetGlobal<WindowLayoutData>(), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Attach_ShouldThrowArgumentException_WhenKeyIsNullOrWhitespace(string? key)
    {
        // Act - Test ohne echtes Window-Objekt
        Action act = () => _sut.Attach(null!, key!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Detach_ShouldNotThrow_WhenKeyIsNull()
    {
        // Act
        Action act = () => _sut.Detach(null!);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Detach_ShouldNotThrow_WhenKeyDoesNotExist()
    {
        // Act
        Action act = () => _sut.Detach("NonExistentKey");

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Dispose_ShouldBeIdempotent()
    {
        // Act
        _sut.Dispose();
        Action act = () => _sut.Dispose();

        // Assert
        act.Should().NotThrow();
    }

    public void Dispose()
    {
        _sut?.Dispose();
    }
}
