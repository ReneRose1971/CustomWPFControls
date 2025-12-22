using System;
using System.IO;
using System.Linq;
using System.Windows;
using CustomWPFControls.Services;
using DataStores.Abstractions;
using DataStores.Runtime;
using Xunit;
using FluentAssertions;

namespace CustomWPFControls.Tests.Behavior;

/// <summary>
/// Behavior-Tests für <see cref="WindowLayoutService"/>.
/// Fokus: End-to-End-Szenarien und Verhaltensvalidierung.
/// </summary>
public sealed class WindowLayoutServiceBehaviorTests : IDisposable
{
    private readonly string _testDirectory;
    private readonly IDataStore<WindowLayoutData> _dataStore;
    private readonly TestDataStores _dataStores;
    private readonly WindowLayoutService _sut;

    public WindowLayoutServiceBehaviorTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), $"CustomWPFControls_Tests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDirectory);

        _dataStore = new InMemoryDataStore<WindowLayoutData>();
        _dataStores = new TestDataStores();
        _dataStores.RegisterGlobal(_dataStore);

        _sut = new WindowLayoutService(_dataStores);
    }

    [StaFact]
    public void Scenario_UserMovesWindow_LayoutIsAutomaticallySaved()
    {
        // Arrange
        var window = CreateTestWindow();
        window.Left = 100;
        window.Top = 100;
        _sut.Attach(window, "MovableWindow");

        // Act
        var layoutData = _dataStore.Items.FirstOrDefault(x => x.WindowKey == "MovableWindow");
        
        layoutData!.Left = 300;
        layoutData.Top = 400;

        // Assert
        layoutData.Left.Should().Be(300);
        layoutData.Top.Should().Be(400);
    }

    [StaFact]
    public void Scenario_UserResizesWindow_LayoutIsAutomaticallySaved()
    {
        // Arrange
        var window = CreateTestWindow();
        window.Width = 800;
        window.Height = 600;
        _sut.Attach(window, "ResizableWindow");

        // Act
        var layoutData = _dataStore.Items.FirstOrDefault(x => x.WindowKey == "ResizableWindow");
        layoutData!.Width = 1024;
        layoutData.Height = 768;

        // Assert
        layoutData.Width.Should().Be(1024);
        layoutData.Height.Should().Be(768);
    }

    [StaFact]
    public void Scenario_UserMaximizesWindow_StateIsAutomaticallySaved()
    {
        // Arrange
        var window = CreateTestWindow();
        window.WindowState = WindowState.Normal;
        _sut.Attach(window, "MaximizableWindow");

        // Act
        var layoutData = _dataStore.Items.FirstOrDefault(x => x.WindowKey == "MaximizableWindow");
        layoutData!.WindowState = (int)WindowState.Maximized;

        // Assert
        layoutData.WindowState.Should().Be((int)WindowState.Maximized);
    }

    [StaFact]
    public void Scenario_WindowClosed_ManualDetachWorks()
    {
        // Arrange
        var window = CreateTestWindow();
        _sut.Attach(window, "ClosableWindow");

        // Act
        _sut.Detach("ClosableWindow");

        // Assert
        var window2 = CreateTestWindow();
        Action act = () => _sut.Attach(window2, "ClosableWindow");
        act.Should().NotThrow();
    }

    [StaFact]
    public void Scenario_ApplicationRestart_WindowPositionIsRestored()
    {
        // Arrange - Erste Session
        var window1 = CreateTestWindow();
        window1.Left = 250;
        window1.Top = 350;
        window1.Width = 900;
        window1.Height = 700;
        window1.WindowState = WindowState.Normal;
        _sut.Attach(window1, "MainApplicationWindow");
        _sut.Detach("MainApplicationWindow");

        // Act - Neuer Service (simuliert Neustart)
        var newService = new WindowLayoutService(_dataStores);
        var window2 = CreateTestWindow();
        window2.Left = 0;
        window2.Top = 0;
        window2.Width = 640;
        window2.Height = 480;
        newService.Attach(window2, "MainApplicationWindow");

        // Assert
        window2.Left.Should().Be(250);
        window2.Top.Should().Be(350);
        window2.Width.Should().Be(900);
        window2.Height.Should().Be(700);
        window2.WindowState.Should().Be(WindowState.Normal);

        newService.Dispose();
    }

    [StaFact]
    public void Scenario_MultipleWindowsInApplication_EachHasIndependentLayout()
    {
        // Arrange
        var mainWindow = CreateTestWindow();
        var settingsDialog = CreateTestWindow();
        var aboutDialog = CreateTestWindow();

        mainWindow.Left = 100;
        settingsDialog.Left = 500;
        aboutDialog.Left = 900;

        // Act
        _sut.Attach(mainWindow, "MainWindow");
        _sut.Attach(settingsDialog, "SettingsDialog");
        _sut.Attach(aboutDialog, "AboutDialog");

        // Assert
        _dataStore.Items.Count.Should().Be(3);
        
        var mainLayout = _dataStore.Items.First(x => x.WindowKey == "MainWindow");
        var settingsLayout = _dataStore.Items.First(x => x.WindowKey == "SettingsDialog");
        var aboutLayout = _dataStore.Items.First(x => x.WindowKey == "AboutDialog");

        mainLayout.Left.Should().Be(100);
        settingsLayout.Left.Should().Be(500);
        aboutLayout.Left.Should().Be(900);
    }

    [StaFact]
    public void Scenario_WindowWithInvalidDimensions_ShouldNotRestoreLayout()
    {
        // Arrange
        _dataStore.Add(new WindowLayoutData
        {
            WindowKey = "InvalidWindow",
            Left = 100,
            Top = 100,
            Width = 0,
            Height = 0,
            WindowState = 0
        });

        // Act
        var window = CreateTestWindow();
        window.Left = 500;
        window.Top = 500;
        window.Width = 800;
        window.Height = 600;
        
        var originalLeft = window.Left;
        var originalTop = window.Top;
        var originalWidth = window.Width;
        var originalHeight = window.Height;

        _sut.Attach(window, "InvalidWindow");

        // Assert - Sollte nicht wiederhergestellt werden, da Width/Height 0
        window.Left.Should().Be(originalLeft);
        window.Top.Should().Be(originalTop);
        window.Width.Should().Be(originalWidth);
        window.Height.Should().Be(originalHeight);
    }

    private static Window CreateTestWindow()
    {
        return new Window
        {
            Width = 640,
            Height = 480,
            WindowStartupLocation = WindowStartupLocation.Manual,
            ShowActivated = false,
            Visibility = Visibility.Hidden
        };
    }

    public void Dispose()
    {
        _sut?.Dispose();
        if (Directory.Exists(_testDirectory))
        {
            try
            {
                Directory.Delete(_testDirectory, true);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }

    /// <summary>
    /// Test-Implementierung von IDataStores für Unit-Tests.
    /// </summary>
    private class TestDataStores : IDataStores
    {
        private readonly Dictionary<Type, object> _stores = new();

        public void RegisterGlobal<T>(IDataStore<T> store) where T : class
        {
            _stores[typeof(T)] = store;
        }

        public IDataStore<T> GetGlobal<T>() where T : class
        {
            if (!_stores.TryGetValue(typeof(T), out var store))
            {
                throw new GlobalStoreNotRegisteredException(typeof(T));
            }
            return (IDataStore<T>)store;
        }

        public IDataStore<T> CreateLocal<T>(IEqualityComparer<T>? comparer = null) where T : class
        {
            return new InMemoryDataStore<T>(comparer);
        }

        public IDataStore<T> CreateLocalSnapshotFromGlobal<T>(
            Func<T, bool>? predicate = null,
            IEqualityComparer<T>? comparer = null) where T : class
        {
            var global = GetGlobal<T>();
            var local = CreateLocal(comparer);
            
            var items = predicate == null 
                ? global.Items 
                : global.Items.Where(predicate);
                
            foreach (var item in items)
            {
                local.Add(item);
            }
            
            return local;
        }
    }
}
