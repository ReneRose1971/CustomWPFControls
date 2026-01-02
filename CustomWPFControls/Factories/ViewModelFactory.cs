using System;
using Microsoft.Extensions.DependencyInjection;

namespace CustomWPFControls.Factories
{
    /// <summary>
    /// Generische Factory, die ViewModels via ActivatorUtilities erstellt.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Unterstützte Constructor-Signaturen:</b>
    /// </para>
    /// <list type="bullet">
    /// <item><c>(TModel model)</c> - Nur Model</item>
    /// <item><c>(TModel model, IServiceProvider serviceProvider)</c> - Model + ServiceProvider</item>
    /// <item><c>(TModel model, IService1 service1, ...)</c> - Model + Services via DI</item>
    /// <item><c>(TModel model, IServiceProvider serviceProvider, IService1 service1, ...)</c> - Alle Kombinationen</item>
    /// </list>
    /// <para>
    /// <b>ActivatorUtilities</b> löst automatisch alle Constructor-Parameter via DI auf,
    /// die nicht explizit übergeben werden (nur TModel wird explizit übergeben).
    /// </para>
    /// </remarks>
    public sealed class ViewModelFactory<TModel, TViewModel> : IViewModelFactory<TModel, TViewModel>
        where TModel : class
        where TViewModel : class
    {
        private readonly IServiceProvider _serviceProvider;

        public ViewModelFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        /// Erstellt ein ViewModel für das gegebene Model.
        /// </summary>
        /// <param name="model">Das Model, für das ein ViewModel erstellt werden soll.</param>
        /// <returns>Eine neue ViewModel-Instanz.</returns>
        /// <exception cref="ArgumentNullException">Wenn <paramref name="model"/> null ist.</exception>
        /// <exception cref="InvalidOperationException">
        /// Wenn das ViewModel nicht erstellt werden kann (z.B. fehlende Services oder falscher Constructor).
        /// </exception>
        public TViewModel Create(TModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            try
            {
                // ActivatorUtilities erstellt das ViewModel:
                // - Übergibt 'model' als expliziten Parameter
                // - Löst alle anderen Constructor-Parameter via DI auf (inkl. IServiceProvider)
                return ActivatorUtilities.CreateInstance<TViewModel>(_serviceProvider, model);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Fehler beim Erstellen von {typeof(TViewModel).Name} für Model {typeof(TModel).Name}. " +
                    $"Stellen Sie sicher, dass der ViewModel-Constructor eine der folgenden Signaturen hat:\n" +
                    $"  - (TModel model)\n" +
                    $"  - (TModel model, IServiceProvider serviceProvider)\n" +
                    $"  - (TModel model, IServiceProvider serviceProvider, ...weitere Services)\n" +
                    $"Alle Services (außer TModel) müssen im DI-Container registriert sein.",
                    ex);
            }
        }
    }
}
