using CustomWPFControls.Tests.Testing;
using CustomWPFControls.Tests.Testing.Bootstrap;
using CustomWPFControls.ViewModels;
using FluentAssertions;
using TestHelper.DataStores.Models;
using Xunit;

namespace CustomWPFControls.Tests.Unit.CollectionViewModel.Constructor;

public sealed class InitializationTests : IClassFixture<PrePopulatedCollectionViewModelFixture>
{
    private readonly PrePopulatedCollectionViewModelFixture _fixture;

    public InitializationTests(PrePopulatedCollectionViewModelFixture fixture)
    {
        _fixture = fixture;
    }

    #region Loading Existing Models Tests

    [Fact]
    public void Constructor_LoadsExistingModels()
    {
        // Arrange & Act
        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        // Assert
        sut.Items.Should().HaveCount(3);
        sut.Count.Should().Be(3);

        // Cleanup
        sut.Dispose();
    }

    [Fact]
    public void Constructor_CreatesViewModelsForAllExistingModels()
    {
        // Arrange & Act
        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        // Assert - Jedes Model hat ein ViewModel
        sut.Items.Should().HaveCount(_fixture.TestData.Count);

        // Cleanup
        sut.Dispose();
    }

    #endregion

    #region ViewModel Creation Tests

    [Fact]
    public void Constructor_CreatesViewModelsViaFactory()
    {
        // Arrange & Act
        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        // Assert - Alle ViewModels sind vom richtigen Typ
        sut.Items.Should().AllBeOfType<TestViewModel>();

        // Cleanup
        sut.Dispose();
    }

    [Fact]
    public void Constructor_ViewModelsHaveCorrectNames()
    {
        // Arrange & Act
        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        // Assert - Namen stimmen überein
        sut.Items.Should().Contain(vm => vm.Name == "FirstItem");
        sut.Items.Should().Contain(vm => vm.Name == "SecondItem");
        sut.Items.Should().Contain(vm => vm.Name == "ThirdItem");

        // Cleanup
        sut.Dispose();
    }

    [Fact]
    public void Constructor_AllViewModelsHaveNonNullModel()
    {
        // Arrange & Act
        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        // Assert
        sut.Items.Should().OnlyContain(vm => vm.Model != null);

        // Cleanup
        sut.Dispose();
    }

    #endregion

    #region Model-ViewModel Mapping Tests

    [Fact]
    public void Constructor_ViewModelsReferenceCorrectModels()
    {
        // Arrange & Act
        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        // Assert - Jedes ViewModel referenziert eines der Test-Models
        foreach (var viewModel in sut.Items)
        {
            _fixture.TestData.Should().Contain(viewModel.Model);
        }

        // Cleanup
        sut.Dispose();
    }

    [Fact]
    public void Constructor_EachModelHasExactlyOneViewModel()
    {
        // Arrange & Act
        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        // Assert - Für jedes Test-Model gibt es genau ein ViewModel
        foreach (var testDto in _fixture.TestData)
        {
            sut.Items.Should().ContainSingle(vm => vm.Model == testDto);
        }

        // Cleanup
        sut.Dispose();
    }

    [Fact]
    public void Constructor_ViewModelPropertiesDelegateToModel()
    {
        // Arrange & Act
        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        // Assert - ViewModel-Properties spiegeln Model-Properties
        foreach (var viewModel in sut.Items)
        {
            viewModel.Name.Should().Be(viewModel.Model.Name);
            viewModel.Id.Should().Be(viewModel.Model.Id);
        }

        // Cleanup
        sut.Dispose();
    }

    #endregion

    #region Order Tests

    [Fact]
    public void Constructor_ViewModelsAreInCorrectOrder()
    {
        // Arrange & Act
        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        // Assert - Reihenfolge entspricht TestData
        sut.Items[0].Name.Should().Be("FirstItem");
        sut.Items[1].Name.Should().Be("SecondItem");
        sut.Items[2].Name.Should().Be("ThirdItem");

        // Cleanup
        sut.Dispose();
    }

    [Fact]
    public void Constructor_ViewModelsMatchTestDataOrder()
    {
        // Arrange & Act
        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        // Assert - ViewModel-Reihenfolge = TestData-Reihenfolge
        for (int i = 0; i < _fixture.TestData.Count; i++)
        {
            sut.Items[i].Model.Should().BeSameAs(_fixture.TestData[i]);
        }

        // Cleanup
        sut.Dispose();
    }

    #endregion

    #region Count Property Tests

    [Fact]
    public void Constructor_CountMatchesNumberOfModels()
    {
        // Arrange & Act
        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        // Assert
        sut.Count.Should().Be(_fixture.TestData.Count);
        sut.Count.Should().Be(3);

        // Cleanup
        sut.Dispose();
    }

    [Fact]
    public void Constructor_CountMatchesItemsCount()
    {
        // Arrange & Act
        var sut = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        // Assert
        sut.Count.Should().Be(sut.Items.Count);

        // Cleanup
        sut.Dispose();
    }

    #endregion

    #region Multiple Instances Tests

    [Fact]
    public void MultipleInstances_ShareSameModels()
    {
        // Arrange & Act - Zwei CollectionViewModel-Instanzen erstellen
        var sut1 = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        var sut2 = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        // Assert - Beide sehen die gleichen Models (gleicher globaler Store)
        sut1.Items.Should().HaveCount(3);
        sut2.Items.Should().HaveCount(3);

        // Assert - Models sind die gleichen Instanzen
        for (int i = 0; i < 3; i++)
        {
            sut2.Items.Should().Contain(vm => vm.Model == sut1.Items[i].Model);
        }

        // Cleanup
        sut1.Dispose();
        sut2.Dispose();
    }

    [Fact]
    public void MultipleInstances_HaveDifferentViewModels()
    {
        // Arrange & Act
        var sut1 = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        var sut2 = new ViewModels.CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);

        // Assert - Jede Instanz hat ihre eigenen ViewModels
        for (int i = 0; i < 3; i++)
        {
            sut1.Items[i].Should().NotBeSameAs(sut2.Items[i], 
                "jede CollectionViewModel-Instanz erstellt ihre eigenen ViewModels");
        }

        // Cleanup
        sut1.Dispose();
        sut2.Dispose();
    }

    #endregion
}
