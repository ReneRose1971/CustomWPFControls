namespace CustomWPFControls.Controls
{
    /// <summary>
    /// Definiert die Position der Action-Buttons in Editor-Controls.
    /// </summary>
    public enum ButtonPlacement
    {
        /// <summary>
        /// Buttons werden rechts neben dem Control angezeigt (horizontal).
        /// Ideal für ComboBox, TextBox und kompakte Controls.
        /// </summary>
        Right,

        /// <summary>
        /// Buttons werden unter dem Control angezeigt (horizontal).
        /// Standard für ListBox/ListView und große Controls.
        /// </summary>
        Bottom,

        /// <summary>
        /// Buttons werden in einer ToolBar über dem Control angezeigt.
        /// Professioneller Look für komplexe Editoren.
        /// </summary>
        Top
    }
}
