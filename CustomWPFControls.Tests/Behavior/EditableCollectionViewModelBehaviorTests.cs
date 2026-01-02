using System;
using System.Linq;
using CustomWPFControls.Factories;
using CustomWPFControls.Tests.Testing;
using CustomWPFControls.ViewModels;
using DataStores.Abstractions;
using Xunit;

namespace CustomWPFControls.Tests.Behavior
{
    /// <summary>
    /// Behavior-Tests für EditableCollectionViewModel (Commands & Callbacks).
    /// </summary>
    public class EditableCollectionViewModelBehaviorTests
    {
        #region Setup Helpers

        private (EditableCollectionViewModel<TestModel, TestViewModel> viewModel, IDataStore<TestModel> dataStore) CreateEditableCollectionViewModel(
            IDataStore<TestModel>? dataStore = null)
        {
            dataStore ??= WPFCOntrolsTestHelpers.CreateDataStore();
            
            var dataStores = WPFCOntrolsTestHelpers.CreateDataStores(dataStore);
            var comparerService = WPFCOntrolsTestHelpers.CreateComparerService();
            var serviceProvider = WPFCOntrolsTestHelpers.CreateServiceProvider();
            
            var factory = serviceProvider.GetService(typeof(IViewModelFactory<TestModel, TestViewModel>)) 
                as IViewModelFactory<TestModel, TestViewModel>;

            var viewModel = new EditableCollectionViewModel<TestModel, TestViewModel>(
                dataStores, factory!, comparerService);

            return (viewModel, dataStore);
        }

        #endregion

        #region AddCommand Tests

        [Fact]
        public void AddCommand_WithCreateModel_AddsNewModel()
        {
            // Arrange
            var (viewModel, dataStore) = CreateEditableCollectionViewModel();
            var newModel = WPFCOntrolsTestHelpers.CreateTestModel(1, "NewModel");
            viewModel.CreateModel = () => newModel;

            // Act
            viewModel.AddCommand.Execute(null);

            // Assert
            Assert.Equal(1, viewModel.Count);
            Assert.Equal(1, dataStore.Items.Count);
            Assert.Contains(viewModel.Items, vm => vm.Model == newModel);
        }

        [Fact]
        public void AddCommand_WithoutCreateModel_ThrowsInvalidOperationException()
        {
            // Arrange
            var (viewModel, _) = CreateEditableCollectionViewModel();
            viewModel.CreateModel = null;

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => viewModel.AddCommand.Execute(null));
        }

        [Fact]
        public void AddCommand_CanExecute_WhenCreateModelSet()
        {
            // Arrange
            var (viewModel, _) = CreateEditableCollectionViewModel();
            viewModel.CreateModel = () => WPFCOntrolsTestHelpers.CreateTestModel();

            // Act
            var canExecute = viewModel.AddCommand.CanExecute(null);

            // Assert
            Assert.True(canExecute);
        }

        [Fact]
        public void AddCommand_CannotExecute_WhenCreateModelNull()
        {
            // Arrange
            var (viewModel, _) = CreateEditableCollectionViewModel();
            viewModel.CreateModel = null;

            // Act
            var canExecute = viewModel.AddCommand.CanExecute(null);

            // Assert
            Assert.False(canExecute);
        }

        [Fact]
        public void AddCommand_MultipleExecutions_AddsMultipleModels()
        {
            // Arrange
            var (viewModel, _) = CreateEditableCollectionViewModel();
            int counter = 0;
            viewModel.CreateModel = () => WPFCOntrolsTestHelpers.CreateTestModel(++counter, $"Model{counter}");

            // Act
            viewModel.AddCommand.Execute(null);
            viewModel.AddCommand.Execute(null);
            viewModel.AddCommand.Execute(null);

            // Assert
            Assert.Equal(3, viewModel.Count);
        }

        #endregion

        #region DeleteCommand Tests

        [Fact]
        public void DeleteCommand_WithSelectedItem_RemovesItem()
        {
            // Arrange
            var (viewModel, dataStore) = CreateEditableCollectionViewModel();
            var model = WPFCOntrolsTestHelpers.CreateTestModel(1, "Model1");
            viewModel.CreateModel = () => model;
            viewModel.AddCommand.Execute(null);
            viewModel.SelectedItem = viewModel.Items.First();

            // Act
            viewModel.DeleteCommand.Execute(null);

            // Assert
            Assert.Equal(0, viewModel.Count);
            Assert.Equal(0, dataStore.Items.Count);
            Assert.Null(viewModel.SelectedItem);
        }

        [Fact]
        public void DeleteCommand_WithoutSelectedItem_DoesNothing()
        {
            // Arrange
            var (viewModel, dataStore) = CreateEditableCollectionViewModel();
            viewModel.CreateModel = () => WPFCOntrolsTestHelpers.CreateTestModel();
            viewModel.AddCommand.Execute(null);
            viewModel.SelectedItem = null;

            // Act
            viewModel.DeleteCommand.Execute(null);

            // Assert
            Assert.Equal(1, viewModel.Count);
            Assert.Equal(1, dataStore.Items.Count);
        }

        [Fact]
        public void DeleteCommand_CanExecute_WhenSelectedItemSet()
        {
            // Arrange
            var (viewModel, _) = CreateEditableCollectionViewModel();
            viewModel.CreateModel = () => WPFCOntrolsTestHelpers.CreateTestModel();
            viewModel.AddCommand.Execute(null);
            viewModel.SelectedItem = viewModel.Items.First();

            // Act
            var canExecute = viewModel.DeleteCommand.CanExecute(null);

            // Assert
            Assert.True(canExecute);
        }

        [Fact]
        public void DeleteCommand_CannotExecute_WhenSelectedItemNull()
        {
            // Arrange
            var (viewModel, _) = CreateEditableCollectionViewModel();
            viewModel.SelectedItem = null;

            // Act
            var canExecute = viewModel.DeleteCommand.CanExecute(null);

            // Assert
            Assert.False(canExecute);
        }

        #endregion

        #region ClearCommand Tests

        [Fact]
        public void ClearCommand_RemovesAllItems()
        {
            // Arrange
            var (viewModel, dataStore) = CreateEditableCollectionViewModel();
            int counter = 0;
            viewModel.CreateModel = () => WPFCOntrolsTestHelpers.CreateTestModel(++counter);
            
            viewModel.AddCommand.Execute(null);
            viewModel.AddCommand.Execute(null);
            viewModel.AddCommand.Execute(null);

            // Act
            viewModel.ClearCommand.Execute(null);

            // Assert
            Assert.Equal(0, viewModel.Count);
            Assert.Equal(0, dataStore.Items.Count);
            Assert.Empty(viewModel.Items);
        }

        [Fact]
        public void ClearCommand_CanExecute_WhenItemsPresent()
        {
            // Arrange
            var (viewModel, _) = CreateEditableCollectionViewModel();
            viewModel.CreateModel = () => WPFCOntrolsTestHelpers.CreateTestModel();
            viewModel.AddCommand.Execute(null);

            // Act
            var canExecute = viewModel.ClearCommand.CanExecute(null);

            // Assert
            Assert.True(canExecute);
        }

        [Fact]
        public void ClearCommand_CannotExecute_WhenNoItems()
        {
            // Arrange
            var (viewModel, _) = CreateEditableCollectionViewModel();

            // Act
            var canExecute = viewModel.ClearCommand.CanExecute(null);

            // Assert
            Assert.False(canExecute);
        }

        #endregion

        #region EditCommand Tests

        [Fact]
        public void EditCommand_WithSelectedItem_CallsEditModel()
        {
            // Arrange
            var (viewModel, _) = CreateEditableCollectionViewModel();
            var model = WPFCOntrolsTestHelpers.CreateTestModel(1, "Model1");
            viewModel.CreateModel = () => model;
            viewModel.AddCommand.Execute(null);
            viewModel.SelectedItem = viewModel.Items.First();

            TestModel? editedModel = null;
            viewModel.EditModel = m => editedModel = m;

            // Act
            viewModel.EditCommand.Execute(null);

            // Assert
            Assert.NotNull(editedModel);
            Assert.Same(model, editedModel);
        }

        [Fact]
        public void EditCommand_WithoutSelectedItem_DoesNothing()
        {
            // Arrange
            var (viewModel, _) = CreateEditableCollectionViewModel();
            viewModel.SelectedItem = null;

            var editCalled = false;
            viewModel.EditModel = _ => editCalled = true;

            // Act
            viewModel.EditCommand.Execute(null);

            // Assert
            Assert.False(editCalled);
        }

        [Fact]
        public void EditCommand_CanExecute_WhenSelectedItemAndEditModelSet()
        {
            // Arrange
            var (viewModel, _) = CreateEditableCollectionViewModel();
            viewModel.CreateModel = () => WPFCOntrolsTestHelpers.CreateTestModel();
            viewModel.AddCommand.Execute(null);
            viewModel.SelectedItem = viewModel.Items.First();
            viewModel.EditModel = _ => { };

            // Act
            var canExecute = viewModel.EditCommand.CanExecute(null);

            // Assert
            Assert.True(canExecute);
        }

        [Fact]
        public void EditCommand_CannotExecute_WhenSelectedItemNull()
        {
            // Arrange
            var (viewModel, _) = CreateEditableCollectionViewModel();
            viewModel.SelectedItem = null;
            viewModel.EditModel = _ => { };

            // Act
            var canExecute = viewModel.EditCommand.CanExecute(null);

            // Assert
            Assert.False(canExecute);
        }

        [Fact]
        public void EditCommand_CannotExecute_WhenEditModelNull()
        {
            // Arrange
            var (viewModel, _) = CreateEditableCollectionViewModel();
            viewModel.CreateModel = () => WPFCOntrolsTestHelpers.CreateTestModel();
            viewModel.AddCommand.Execute(null);
            viewModel.SelectedItem = viewModel.Items.First();
            viewModel.EditModel = null;

            // Act
            var canExecute = viewModel.EditCommand.CanExecute(null);

            // Assert
            Assert.False(canExecute);
        }

        #endregion

        #region Complex Scenarios (Behavior)

        [Fact]
        public void Scenario_AddMultipleItemsSelectOneDelete_WorksCorrectly()
        {
            // Arrange
            var (viewModel, _) = CreateEditableCollectionViewModel();
            int counter = 0;
            viewModel.CreateModel = () => WPFCOntrolsTestHelpers.CreateTestModel(++counter, $"Model{counter}");

            // Act
            viewModel.AddCommand.Execute(null); // Model1
            viewModel.AddCommand.Execute(null); // Model2
            viewModel.AddCommand.Execute(null); // Model3

            viewModel.SelectedItem = viewModel.Items[1]; // Select Model2
            viewModel.DeleteCommand.Execute(null);

            // Assert
            Assert.Equal(2, viewModel.Count);
            Assert.DoesNotContain(viewModel.Items, vm => vm.Name == "Model2");
            Assert.Contains(viewModel.Items, vm => vm.Name == "Model1");
            Assert.Contains(viewModel.Items, vm => vm.Name == "Model3");
        }

        [Fact]
        public void Scenario_AddEditDelete_WorksCorrectly()
        {
            // Arrange
            var (viewModel, _) = CreateEditableCollectionViewModel();
            var model = WPFCOntrolsTestHelpers.CreateTestModel(1, "OriginalName");
            viewModel.CreateModel = () => model;
            
            string? editedName = null;
            viewModel.EditModel = m => editedName = m.Name;

            // Act
            viewModel.AddCommand.Execute(null);
            viewModel.SelectedItem = viewModel.Items.First();
            viewModel.EditCommand.Execute(null);
            viewModel.DeleteCommand.Execute(null);

            // Assert
            Assert.Equal("OriginalName", editedName);
            Assert.Equal(0, viewModel.Count);
        }

        [Fact]
        public void Scenario_AddMultipleItemsClearAll_WorksCorrectly()
        {
            // Arrange
            var (viewModel, _) = CreateEditableCollectionViewModel();
            int counter = 0;
            viewModel.CreateModel = () => WPFCOntrolsTestHelpers.CreateTestModel(++counter);

            // Act
            for (int i = 0; i < 10; i++)
            {
                viewModel.AddCommand.Execute(null);
            }
            viewModel.ClearCommand.Execute(null);

            // Assert
            Assert.Equal(0, viewModel.Count);
            Assert.Empty(viewModel.Items);
        }

        [Fact]
        public void Scenario_CommandChaining_WorksCorrectly()
        {
            // Arrange
            var (viewModel, _) = CreateEditableCollectionViewModel();
            int counter = 0;
            viewModel.CreateModel = () => WPFCOntrolsTestHelpers.CreateTestModel(++counter);

            // Act - Chain: Add, Add, Delete first, Add, Clear
            viewModel.AddCommand.Execute(null); // 1
            viewModel.AddCommand.Execute(null); // 2
            viewModel.SelectedItem = viewModel.Items[0];
            viewModel.DeleteCommand.Execute(null); // Remove 1
            viewModel.AddCommand.Execute(null); // 3
            viewModel.ClearCommand.Execute(null);

            // Assert
            Assert.Equal(0, viewModel.Count);
        }

        #endregion

        #region SelectedItem Auto-Clear Tests

        [Fact]
        public void SelectedItem_AutoSetToNull_WhenItemRemoved()
        {
            // Arrange
            var (viewModel, _) = CreateEditableCollectionViewModel();
            viewModel.CreateModel = () => WPFCOntrolsTestHelpers.CreateTestModel(1, "Model1");
            viewModel.AddCommand.Execute(null);
            viewModel.SelectedItem = viewModel.Items.First();

            // Act
            viewModel.DeleteCommand.Execute(null);

            // Assert
            Assert.Null(viewModel.SelectedItem);
        }

        [Fact]
        public void SelectedItem_AutoSetToNull_WhenClear()
        {
            // Arrange
            var (viewModel, _) = CreateEditableCollectionViewModel();
            int counter = 0;
            viewModel.CreateModel = () => WPFCOntrolsTestHelpers.CreateTestModel(++counter);
            
            viewModel.AddCommand.Execute(null);
            viewModel.AddCommand.Execute(null);
            viewModel.SelectedItem = viewModel.Items[1];

            // Act
            viewModel.ClearCommand.Execute(null);

            // Assert
            Assert.Null(viewModel.SelectedItem);
        }

        [Fact]
        public void SelectedItem_RemainsSet_WhenOtherItemRemoved()
        {
            // Arrange
            var (viewModel, dataStore) = CreateEditableCollectionViewModel();
            int counter = 0;
            viewModel.CreateModel = () => WPFCOntrolsTestHelpers.CreateTestModel(++counter);
            
            viewModel.AddCommand.Execute(null); // Item1
            viewModel.AddCommand.Execute(null); // Item2
            viewModel.SelectedItem = viewModel.Items[1]; // Select Item2

            // Act - Remove Item1 via DataStore
            var itemToRemove = dataStore.Items.First();
            dataStore.Remove(itemToRemove);

            // Assert
            Assert.NotNull(viewModel.SelectedItem); // Still Item2
            Assert.Equal(2, viewModel.SelectedItem.Id);
        }

        #endregion

        #region CreateModel & EditModel Callback Tests

        [Fact]
        public void CreateModel_CanBeChangedAtRuntime()
        {
            // Arrange
            var (viewModel, _) = CreateEditableCollectionViewModel();
            viewModel.CreateModel = () => WPFCOntrolsTestHelpers.CreateTestModel(1, "Type1");

            // Act
            viewModel.AddCommand.Execute(null);
            viewModel.CreateModel = () => WPFCOntrolsTestHelpers.CreateTestModel(2, "Type2");
            viewModel.AddCommand.Execute(null);

            // Assert
            Assert.Equal(2, viewModel.Count);
            Assert.Contains(viewModel.Items, vm => vm.Name == "Type1");
            Assert.Contains(viewModel.Items, vm => vm.Name == "Type2");
        }

        [Fact]
        public void EditModel_ReceivesCorrectModel()
        {
            // Arrange
            var (viewModel, _) = CreateEditableCollectionViewModel();
            var model1 = WPFCOntrolsTestHelpers.CreateTestModel(1, "Model1");
            var model2 = WPFCOntrolsTestHelpers.CreateTestModel(2, "Model2");
            
            viewModel.CreateModel = () => model1;
            viewModel.AddCommand.Execute(null);
            viewModel.CreateModel = () => model2;
            viewModel.AddCommand.Execute(null);

            TestModel? editedModel = null;
            viewModel.EditModel = m => editedModel = m;

            // Act
            viewModel.SelectedItem = viewModel.Items[1]; // Select Model2
            viewModel.EditCommand.Execute(null);

            // Assert
            Assert.Same(model2, editedModel);
        }

        #endregion
    }
}
