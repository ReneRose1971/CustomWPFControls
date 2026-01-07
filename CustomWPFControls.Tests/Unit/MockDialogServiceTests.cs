using CustomWPFControls.Services.Dialogs;
using FluentAssertions;
using System.Windows;
using TestHelper.CustomWPFControls.Mocks;
using Xunit;

namespace CustomWPFControls.Tests.Unit
{
    /// <summary>
    /// Unit-Tests für MockDialogService.
    /// </summary>
    public sealed class MockDialogServiceTests
    {
        private readonly MockDialogService _sut;

        public MockDialogServiceTests()
        {
            _sut = new MockDialogService();
        }

        [Fact]
        public void ShowDialog_RecordsCall()
        {
            // Arrange
            var viewModel = new TestViewModel();

            // Act
            _sut.ShowDialog(viewModel, (Window?)null);

            // Assert
            _sut.Calls.Should().HaveCount(1);
            _sut.Calls[0].Type.Should().Be(DialogType.Modal);
            _sut.Calls[0].ViewModel.Should().BeSameAs(viewModel);
            _sut.Calls[0].ViewModelType.Should().Be(typeof(TestViewModel));
        }

        [Fact]
        public void ShowDialog_ReturnsNextDialogResult()
        {
            // Arrange
            var viewModel = new TestViewModel();
            _sut.NextDialogResult = false;

            // Act
            var result = _sut.ShowDialog(viewModel, (Window?)null);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ShowDialog_WithTitle_RecordsTitleAndDimensions()
        {
            // Arrange
            var viewModel = new TestViewModel();

            // Act
            _sut.ShowDialog(viewModel, "Test Title", 640, 480);

            // Assert
            _sut.Calls[0].Title.Should().Be("Test Title");
            _sut.Calls[0].Width.Should().Be(640);
            _sut.Calls[0].Height.Should().Be(480);
        }

        [Fact]
        public void ShowMessage_RecordsCall()
        {
            // Act
            _sut.ShowMessage("Test Message", "Test Title");

            // Assert
            _sut.Calls.Should().HaveCount(1);
            _sut.Calls[0].Type.Should().Be(DialogType.Message);
            _sut.Calls[0].Message.Should().Be("Test Message");
            _sut.Calls[0].Title.Should().Be("Test Title");
            _sut.Calls[0].Icon.Should().Be(MessageBoxImage.Information);
        }

        [Fact]
        public void ShowWarning_RecordsCall()
        {
            // Act
            _sut.ShowWarning("Warning");

            // Assert
            _sut.Calls[0].Type.Should().Be(DialogType.Warning);
            _sut.Calls[0].Icon.Should().Be(MessageBoxImage.Warning);
        }

        [Fact]
        public void ShowError_RecordsCall()
        {
            // Act
            _sut.ShowError("Error");

            // Assert
            _sut.Calls[0].Type.Should().Be(DialogType.Error);
            _sut.Calls[0].Icon.Should().Be(MessageBoxImage.Error);
        }

        [Fact]
        public void ShowConfirmation_ReturnsNextConfirmationResult()
        {
            // Arrange
            _sut.NextConfirmationResult = false;

            // Act
            var result = _sut.ShowConfirmation("Confirm?");

            // Assert
            result.Should().BeFalse();
            _sut.Calls[0].Type.Should().Be(DialogType.Confirmation);
        }

        [Fact]
        public void ShowWindow_RecordsCall()
        {
            // Arrange
            var viewModel = new TestViewModel();

            // Act
            var window = _sut.ShowWindow(viewModel, "Test Window");

            // Assert - Window kann null sein in Mock
            _sut.Calls.Should().HaveCount(1);
            _sut.Calls[0].Type.Should().Be(DialogType.Modeless);
            _sut.Calls[0].ViewModelType.Should().Be(typeof(TestViewModel));
        }

        [Fact]
        public void Reset_ClearsCallsAndResetsDefaults()
        {
            // Arrange
            _sut.ShowMessage("Test");
            _sut.NextDialogResult = false;
            _sut.NextConfirmationResult = false;

            // Act
            _sut.Reset();

            // Assert
            _sut.Calls.Should().BeEmpty();
            _sut.NextDialogResult.Should().BeTrue();
            _sut.NextConfirmationResult.Should().BeTrue();
        }

        [Fact]
        public void VerifyDialogShown_WithMatchingType_DoesNotThrow()
        {
            // Arrange
            _sut.ShowDialog(new TestViewModel(), (Window?)null);

            // Act
            Action act = () => _sut.VerifyDialogShown<TestViewModel>();

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void VerifyDialogShown_WithNonMatchingType_ThrowsAssertionException()
        {
            // Arrange
            _sut.ShowDialog(new TestViewModel(), (Window?)null);

            // Act
            Action act = () => _sut.VerifyDialogShown<AnotherViewModel>();

            // Assert
            act.Should().Throw<MockAssertionException>()
                .WithMessage("*Kein Dialog*");
        }

        [Fact]
        public void VerifyMessageShown_WithMatchingMessage_DoesNotThrow()
        {
            // Arrange
            _sut.ShowMessage("Test Message");

            // Act
            Action act = () => _sut.VerifyMessageShown("Test Message");

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void VerifyMessageShown_ThrowsIfMessageNotShown()
        {
            // Arrange
            var mockDialog = new MockDialogService();

            // Act
            Action act = () => mockDialog.VerifyMessageShown("Expected message");

            // Assert
            act.Should().Throw<MockAssertionException>()
                .WithMessage("*Keine Nachricht*");
        }

        [Fact]
        public void VerifyAnyDialogShown_WithCalls_DoesNotThrow()
        {
            // Arrange
            _sut.ShowMessage("Test");

            // Act
            Action act = () => _sut.VerifyAnyDialogShown();

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void VerifyAnyDialogShown_WithoutCalls_ThrowsAssertionException()
        {
            // Act
            Action act = () => _sut.VerifyAnyDialogShown();

            // Assert
            act.Should().Throw<MockAssertionException>()
                .WithMessage("*Keine Dialoge*");
        }

        [Fact]
        public void GetCallFor_WithMatchingType_ReturnsCall()
        {
            // Arrange
            var viewModel = new TestViewModel();
            _sut.ShowDialog(viewModel, (Window?)null);

            // Act
            var call = _sut.GetCallFor<TestViewModel>();

            // Assert
            call.Should().NotBeNull();
            call!.ViewModel.Should().BeSameAs(viewModel);
        }

        [Fact]
        public void GetCallFor_WithNonMatchingType_ReturnsNull()
        {
            // Arrange
            _sut.ShowDialog(new TestViewModel(), (Window?)null);

            // Act
            var call = _sut.GetCallFor<AnotherViewModel>();

            // Assert
            call.Should().BeNull();
        }

        [Fact]
        public void MultipleCalls_AreRecordedInOrder()
        {
            // Act
            _sut.ShowMessage("Message 1");
            _sut.ShowWarning("Warning 1");
            _sut.ShowError("Error 1");

            // Assert
            _sut.Calls.Should().HaveCount(3);
            _sut.Calls[0].Type.Should().Be(DialogType.Message);
            _sut.Calls[1].Type.Should().Be(DialogType.Warning);
            _sut.Calls[2].Type.Should().Be(DialogType.Error);
        }

        // Test-Klassen
        private class TestViewModel { }
        private class AnotherViewModel { }
    }
}
