using System;
using CustomWPFControls.Tests.Testing;
using FluentAssertions;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.LoadModelsAPI;

/// <summary>
/// Test: LoadModels mit null wirft ArgumentNullException.
/// </summary>
public sealed class LoadModels_WithNull_ThrowsArgumentNullException : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;

    public LoadModels_WithNull_ThrowsArgumentNullException(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
    }

    [Fact]
    public void Test_LoadModels_WithNull_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => _fixture.Sut.LoadModels(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("models");
    }

    public void Dispose()
    {
        _fixture.ClearTestData();
    }
}
