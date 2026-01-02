using System.Windows;
using CustomWPFControls.TestHelpers.Mocks;
using FluentAssertions;
using Xunit;

namespace CustomWPFControls.Tests.Unit
{
    /// <summary>
    /// Unit-Tests für MockMessageBoxService.
    /// </summary>
    public sealed class MockMessageBoxServiceTests
    {
        [Fact]
        public void ShowMessage_TracksCall()
        {
            // Arrange
            var mock = new MockMessageBoxService();

            // Act
            mock.ShowMessage("Test message", "Test Title");

            // Assert
            mock.Calls.Should().HaveCount(1);
            mock.Calls[0].Type.Should().Be(MessageBoxType.Information);
            mock.Calls[0].Message.Should().Be("Test message");
            mock.Calls[0].Title.Should().Be("Test Title");
        }

        [Fact]
        public void ShowWarning_TracksCall()
        {
            // Arrange
            var mock = new MockMessageBoxService();

            // Act
            mock.ShowWarning("Warning message");

            // Assert
            mock.Calls.Should().HaveCount(1);
            mock.Calls[0].Type.Should().Be(MessageBoxType.Warning);
            mock.Calls[0].Message.Should().Be("Warning message");
        }

        [Fact]
        public void ShowError_TracksCall()
        {
            // Arrange
            var mock = new MockMessageBoxService();

            // Act
            mock.ShowError("Error message");

            // Assert
            mock.Calls.Should().HaveCount(1);
            mock.Calls[0].Type.Should().Be(MessageBoxType.Error);
            mock.Calls[0].Message.Should().Be("Error message");
        }

        [Fact]
        public void ShowConfirmation_ReturnsConfiguredResult()
        {
            // Arrange
            var mock = new MockMessageBoxService();
            mock.NextYesNoResult = true;

            // Act
            var result = mock.ShowConfirmation("Confirm?");

            // Assert
            result.Should().BeTrue();
            mock.Calls.Should().HaveCount(1);
            mock.Calls[0].Type.Should().Be(MessageBoxType.Question);
        }

        [Fact]
        public void AskYesNo_ReturnsConfiguredResult()
        {
            // Arrange
            var mock = new MockMessageBoxService();
            mock.NextYesNoResult = false;

            // Act
            var result = mock.AskYesNo("Question?");

            // Assert
            result.Should().BeFalse();
            mock.Calls.Should().HaveCount(1);
        }

        [Fact]
        public void AskYesNoCancel_ReturnsConfiguredResult()
        {
            // Arrange
            var mock = new MockMessageBoxService();

            // Test Yes
            mock.NextYesNoCancelResult = true;
            var resultYes = mock.AskYesNoCancel("Question?");
            resultYes.Should().BeTrue();

            // Test No
            mock.NextYesNoCancelResult = false;
            var resultNo = mock.AskYesNoCancel("Question?");
            resultNo.Should().BeFalse();

            // Test Cancel
            mock.NextYesNoCancelResult = null;
            var resultCancel = mock.AskYesNoCancel("Question?");
            resultCancel.Should().BeNull();

            // Assert
            mock.Calls.Should().HaveCount(3);
        }

        [Fact]
        public void AskOkCancel_ReturnsConfiguredResult()
        {
            // Arrange
            var mock = new MockMessageBoxService();
            mock.NextOkCancelResult = true;

            // Act
            var result = mock.AskOkCancel("Proceed?");

            // Assert
            result.Should().BeTrue();
            mock.Calls.Should().HaveCount(1);
        }

        [Fact]
        public void ShowMessageBox_ReturnsConfiguredResult()
        {
            // Arrange
            var mock = new MockMessageBoxService();
            mock.NextResult = MessageBoxResult.Cancel;

            // Act
            var result = mock.ShowMessageBox(
                "Message",
                "Title",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Warning);

            // Assert
            result.Should().Be(MessageBoxResult.Cancel);
            mock.Calls.Should().HaveCount(1);
            mock.Calls[0].Buttons.Should().Be(MessageBoxButton.OKCancel);
            mock.Calls[0].Icon.Should().Be(MessageBoxImage.Warning);
        }

        [Fact]
        public void VerifyMessageShown_SucceedsWhenMessageWasShown()
        {
            // Arrange
            var mock = new MockMessageBoxService();
            mock.ShowMessage("Expected message");

            // Act & Assert
            mock.VerifyMessageShown("Expected message");  // Should not throw
        }

        [Fact]
        public void VerifyMessageShown_ThrowsWhenMessageWasNotShown()
        {
            // Arrange
            var mock = new MockMessageBoxService();

            // Act
            Action act = () => mock.VerifyMessageShown("Expected message");

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*wurde nicht angezeigt*");
        }

        [Fact]
        public void VerifyWarningShown_WorksCorrectly()
        {
            // Arrange
            var mock = new MockMessageBoxService();
            mock.ShowWarning("Warning text");

            // Act & Assert
            mock.VerifyWarningShown("Warning text");  // Should not throw
        }

        [Fact]
        public void VerifyErrorShown_WorksCorrectly()
        {
            // Arrange
            var mock = new MockMessageBoxService();
            mock.ShowError("Error text");

            // Act & Assert
            mock.VerifyErrorShown("Error text");  // Should not throw
        }

        [Fact]
        public void VerifyConfirmationShown_WorksCorrectly()
        {
            // Arrange
            var mock = new MockMessageBoxService();
            mock.ShowConfirmation("Confirm this?");

            // Act & Assert
            mock.VerifyConfirmationShown("Confirm this?");  // Should not throw
        }

        [Fact]
        public void VerifyCallCount_WorksCorrectly()
        {
            // Arrange
            var mock = new MockMessageBoxService();
            mock.ShowMessage("Message 1");
            mock.ShowWarning("Warning 1");
            mock.ShowError("Error 1");

            // Act & Assert
            mock.VerifyCallCount(3);  // Should not throw
        }

        [Fact]
        public void VerifyCallCount_ThrowsWhenCountMismatch()
        {
            // Arrange
            var mock = new MockMessageBoxService();
            mock.ShowMessage("Message");

            // Act
            Action act = () => mock.VerifyCallCount(2);

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*Erwartete 2*aber 1*");
        }

        [Fact]
        public void Reset_ClearsAllCallsAndResetsDefaults()
        {
            // Arrange
            var mock = new MockMessageBoxService();
            mock.ShowMessage("Message");
            mock.NextYesNoResult = false;
            mock.NextResult = MessageBoxResult.Cancel;

            // Act
            mock.Reset();

            // Assert
            mock.Calls.Should().BeEmpty();
            mock.NextYesNoResult.Should().BeTrue();
            mock.NextResult.Should().Be(MessageBoxResult.OK);
        }

        [Fact]
        public void MultipleMessages_AreTrackedCorrectly()
        {
            // Arrange
            var mock = new MockMessageBoxService();

            // Act
            mock.ShowMessage("Message 1");
            mock.ShowWarning("Warning 1");
            mock.ShowError("Error 1");
            mock.ShowConfirmation("Confirm 1");

            // Assert
            mock.Calls.Should().HaveCount(4);
            mock.Calls[0].Type.Should().Be(MessageBoxType.Information);
            mock.Calls[1].Type.Should().Be(MessageBoxType.Warning);
            mock.Calls[2].Type.Should().Be(MessageBoxType.Error);
            mock.Calls[3].Type.Should().Be(MessageBoxType.Question);
        }
    }
}
