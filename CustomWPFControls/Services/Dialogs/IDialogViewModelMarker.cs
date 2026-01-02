namespace CustomWPFControls.Services.Dialogs
{
    /// <summary>
    /// Marker-Interface für ViewModels, die automatisch im DI-Container registriert werden sollen.
    /// </summary>
    /// <remarks>
    /// <para>
    /// ViewModels, die dieses Interface implementieren, werden automatisch vom
    /// CustomWPFControlsBootstrapDecorator gefunden und im DI-Container als Transient registriert.
    /// </para>
    /// <para>
    /// <b>Warum Transient?</b> Jeder Dialog-Aufruf erstellt eine neue ViewModel-Instanz.
    /// </para>
    /// <para>
    /// <b>Verwendung:</b>
    /// </para>
    /// <code>
    /// public class CustomerEditViewModel : IDialogViewModelMarker
    /// {
    ///     private readonly ICustomerService _customerService;
    ///     
    ///     public CustomerEditViewModel(ICustomerService customerService)
    ///     {
    ///         _customerService = customerService;
    ///     }
    ///     
    ///     public string Name { get; set; }
    ///     public string Email { get; set; }
    /// }
    /// </code>
    /// <para>
    /// <b>Automatische Registrierung:</b>
    /// </para>
    /// <list type="bullet">
    /// <item>Assembly-Scanning findet: CustomerEditViewModel : IDialogViewModelMarker</item>
    /// <item>Registriert: services.AddTransient&lt;CustomerEditViewModel&gt;()</item>
    /// <item>DialogService kann jetzt: ShowDialog&lt;CustomerEditViewModel&gt;() aufrufen</item>
    /// <item>Dependencies werden automatisch über DI aufgelöst</item>
    /// </list>
    /// </remarks>
    public interface IDialogViewModelMarker
    {
        // Marker-Interface - keine Methoden erforderlich
    }
}
