using CustomWPFControls.ViewModels;
using TestHelper.DataStores.Models;

namespace CustomWPFControls.Tests.Testing
{
    /// <summary>
    /// Test-ViewModel für Unit/Integration-Tests.
    /// </summary>
    public class TestViewModel : ViewModelBase<TestDto>
    {
        private bool _isSelected;
        private bool _isExpanded;

        public TestViewModel(TestDto model) : base(model)
        {
        }

        // Domain-Properties (delegiert an Model)
        public System.Guid Id => Model.Id;
        public string Name => Model.Name;

        // UI-Properties mit manuellem PropertyChanged für Tests
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged(nameof(IsExpanded));
                }
            }
        }

        // Test-Hilfsproperty zum Tracken von Dispose-Aufrufen
        public bool IsDisposed { get; private set; }

        ~TestViewModel()
        {
            IsDisposed = true;
        }
    }
}
