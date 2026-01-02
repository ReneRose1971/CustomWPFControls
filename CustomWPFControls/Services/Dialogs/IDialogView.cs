using System.Windows;

namespace CustomWPFControls.Services.Dialogs
{
    /// <summary>
    /// Marker-Interface für Windows, die als Dialoge verwendet werden.
    /// Definiert die Zuordnung zum ViewModel über den generischen Typ-Parameter.
    /// </summary>
    /// <typeparam name="TViewModel">Typ des zugeordneten ViewModels</typeparam>
    /// <remarks>
    /// <para>
    /// Windows, die dieses Interface implementieren, werden automatisch vom
    /// CustomWPFControlsBootstrapDecorator gefunden und im DI-Container registriert.
    /// </para>
    /// <para>
    /// Der generische Typ-Parameter definiert die Zuordnung ViewModel ? View.
    /// </para>
    /// <para>
    /// <b>Verwendung:</b>
    /// </para>
    /// <code>
    /// public partial class CustomerEditDialog : Window, IDialogView&lt;CustomerEditViewModel&gt;
    /// {
    ///     public CustomerEditDialog()
    ///     {
    ///         InitializeComponent();
    ///     }
    /// }
    /// </code>
    /// <para>
    /// <b>Automatische Registrierung:</b>
    /// </para>
    /// <list type="bullet">
    /// <item>Assembly-Scanning findet: CustomerEditDialog : IDialogView&lt;CustomerEditViewModel&gt;</item>
    /// <item>Registriert: services.AddTransient&lt;IDialogView&lt;CustomerEditViewModel&gt;, CustomerEditDialog&gt;()</item>
    /// <item>DialogService kann jetzt: ShowDialog&lt;CustomerEditViewModel&gt;() aufrufen</item>
    /// </list>
    /// </remarks>
    public interface IDialogView<TViewModel> where TViewModel : class
    {
        // Marker-Interface - Generic-Argument definiert ViewModel-Zuordnung
        // Keine Methoden erforderlich
    }
}
