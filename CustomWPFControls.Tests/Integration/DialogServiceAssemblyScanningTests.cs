using System.Windows;
using CustomWPFControls.Services.Dialogs;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using CustomWPFControls.Bootstrap;
using System.Reflection;

namespace CustomWPFControls.Tests.Integration
{
    /// <summary>
    /// Integration-Tests für automatische Dialog-View- und ViewModel-Registrierung.
    /// </summary>
    public sealed class DialogServiceAssemblyScanningTests
    {
        // ????????????????????????????????????????????????????????????
        // Test-ViewModels
        // ????????????????????????????????????????????????????????????

        public class TestDialogViewModel : IDialogViewModelMarker
        {
            public string Name { get; set; } = "Test";
            public string Email { get; set; } = "test@example.com";
        }

        public class AnotherDialogViewModel : IDialogViewModelMarker
        {
            public int Value { get; set; } = 42;
        }

        // ????????????????????????????????????????????????????????????
        // Test-Views
        // ????????????????????????????????????????????????????????????

        public class TestDialogView : Window, IDialogView<TestDialogViewModel>
        {
            public TestDialogView()
            {
                ShowActivated = false;
                Visibility = Visibility.Hidden;
                ShowInTaskbar = false;
            }
        }

        public class AnotherDialogView : Window, IDialogView<AnotherDialogViewModel>
        {
            public AnotherDialogView()
            {
                ShowActivated = false;
                Visibility = Visibility.Hidden;
                ShowInTaskbar = false;
            }
        }

        // ????????????????????????????????????????????????????????????
        // Tests
        // ????????????????????????????????????????????????????????????

        [StaFact]
        public void AddDialogViewsFromAssemblies_RegistersAllDialogViews()
        {
            // Arrange
            var services = new ServiceCollection();
            var assembly = Assembly.GetExecutingAssembly();

            // Act
            services.AddDialogViewsFromAssemblies(assembly);
            var provider = services.BuildServiceProvider();

            // Assert
            var testViewType = typeof(IDialogView<TestDialogViewModel>);
            var testView = provider.GetService(testViewType);
            testView.Should().NotBeNull();
            testView.Should().BeOfType<TestDialogView>();

            var anotherViewType = typeof(IDialogView<AnotherDialogViewModel>);
            var anotherView = provider.GetService(anotherViewType);
            anotherView.Should().NotBeNull();
            anotherView.Should().BeOfType<AnotherDialogView>();
        }

        [Fact]
        public void AddDialogViewModelsFromAssemblies_RegistersAllViewModels()
        {
            // Arrange
            var services = new ServiceCollection();
            var assembly = Assembly.GetExecutingAssembly();

            // Act
            services.AddDialogViewModelsFromAssemblies(assembly);
            var provider = services.BuildServiceProvider();

            // Assert
            var testViewModel = provider.GetService<TestDialogViewModel>();
            testViewModel.Should().NotBeNull();
            testViewModel.Name.Should().Be("Test");

            var anotherViewModel = provider.GetService<AnotherDialogViewModel>();
            anotherViewModel.Should().NotBeNull();
            anotherViewModel.Value.Should().Be(42);
        }

        [StaFact]
        public void DialogService_ShowDialog_WithConvention_CreatesViewAndViewModel()
        {
            // Arrange
            var services = new ServiceCollection();
            var assembly = Assembly.GetExecutingAssembly();

            services.AddDialogService();
            services.AddDialogViewsFromAssemblies(assembly);
            services.AddDialogViewModelsFromAssemblies(assembly);

            var provider = services.BuildServiceProvider();
            var dialogService = provider.GetRequiredService<IDialogService>();

            // Act & Assert - sollte nicht werfen
            // (Dialog wird angezeigt aber sofort geschlossen, da Hidden)
            Action act = () =>
            {
                try
                {
                    dialogService.ShowDialog<TestDialogViewModel>("Test Title");
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("Cannot set Visibility"))
                {
                    // WPF-Visibility-Exception ignorieren (Window ist Hidden)
                }
            };

            act.Should().NotThrow<InvalidOperationException>(because: "View und ViewModel sollten registriert sein");
        }

        [StaFact]
        public void CustomWPFControlsBootstrapDecorator_RegistersDialogServices()
        {
            // Arrange
            var services = new ServiceCollection();
            var assembly = Assembly.GetExecutingAssembly();

            var bootstrap = new CustomWPFControlsBootstrapDecorator(
                new Common.Bootstrap.DefaultBootstrapWrapper());

            // Act
            bootstrap.RegisterServices(services, assembly);
            var provider = services.BuildServiceProvider();

            // Assert
            var dialogService = provider.GetService<IDialogService>();
            dialogService.Should().NotBeNull();

            var testView = provider.GetService(typeof(IDialogView<TestDialogViewModel>));
            testView.Should().NotBeNull();

            var testViewModel = provider.GetService<TestDialogViewModel>();
            testViewModel.Should().NotBeNull();
        }

        [Fact]
        public void DialogService_WithoutRegisteredView_ThrowsInvalidOperationException()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddDialogService();
            var provider = services.BuildServiceProvider();
            var dialogService = provider.GetRequiredService<IDialogService>();

            var viewModel = new TestDialogViewModel();

            // Act
            Action act = () => dialogService.ShowDialog(viewModel, (Window?)null);

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*No service for type*");
        }

        [Fact]
        public void DialogService_WithoutRegisteredViewModel_ThrowsInvalidOperationException()
        {
            // Arrange
            var services = new ServiceCollection();
            var assembly = Assembly.GetExecutingAssembly();

            services.AddDialogService();
            services.AddDialogViewsFromAssemblies(assembly);
            // NICHT: services.AddDialogViewModelsFromAssemblies(assembly);

            var provider = services.BuildServiceProvider();
            var dialogService = provider.GetRequiredService<IDialogService>();

            // Act
            Action act = () => dialogService.ShowDialog<TestDialogViewModel>();

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*No service for type*");
        }
    }
}
