namespace CargoSupport.Enums
{
    /// <summary>
    /// Key status of route
    /// </summary>
    public enum KeyStatus
    {
        /// <summary>
        /// Not started
        /// </summary>
        Ej_påbörjad,

        /// <summary>
        /// Is loading
        /// </summary>
        Lastas,

        /// <summary>
        /// Route is completed and home
        /// </summary>
        Kontor,

        /// <summary>
        /// Route is not started, but ready outside
        /// </summary>
        Tomgång
    }
}