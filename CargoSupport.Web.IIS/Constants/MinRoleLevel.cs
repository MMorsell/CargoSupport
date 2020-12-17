using System.Collections.Generic;

namespace CargoSupport.Constants
{
    /// <summary>
    /// Role levels in the application
    /// </summary>
    public static class MinRoleLevel
    {
        #region Role keys

        /// <summary>
        /// SuperUser role Type key
        /// </summary>
        public const string SuperUser = "SuperUser";

        /// <summary>
        /// Role type key for Group bosses in transport
        /// </summary>
        public const string Gruppchef = "GC_trp";

        /// <summary>
        /// Role type key for Transport bosses
        /// </summary>
        public const string TransportLedare = "TL_trp";

        /// <summary>
        /// Role type key for the public transport members
        /// </summary>
        public const string Medarbetare = "Medarbetare_Trp";

        /// <summary>
        /// Role type key for Group bosses in the warehouse
        /// </summary>
        public const string PlockAnalys = "PlockAnalys";

        /// <summary>
        /// Role type key for the public warehouse members
        /// </summary>
        public const string Plock = "Plock";

        #endregion Role keys

        #region Prefixed grouped role keys

        /// <summary>
        /// Prefixed key constant for <see cref="SuperUser"/> And roles over
        /// </summary>
        public const string SuperUserAndUp = SuperUser;

        /// <summary>
        /// Prefixed key constant for <see cref="SuperUser"/> And <see cref="PlockAnalys"/>
        /// </summary>
        public const string SuperUserAndPlockAnalys = SuperUser + ", " + PlockAnalys;

        /// <summary>
        /// Prefixed key constant for <see cref="Gruppchef"/> And roles over
        /// </summary>
        public const string GruppChefAndUp = SuperUser + ", " + Gruppchef;

        /// <summary>
        /// Prefixed key constant for <see cref="TransportLedare"/> And roles over
        /// </summary>
        public const string TransportLedareAndUp = SuperUser + ", " + Gruppchef + ", " + TransportLedare;

        /// <summary>
        /// Prefixed key constant for <see cref="Medarbetare"/> And roles over
        /// </summary>
        public const string MedarbetareAndUp = SuperUser + ", " + Gruppchef + ", " + TransportLedare + ", " + Medarbetare;

        /// <summary>
        /// Prefixed key constant for <see cref="Plock"/> And roles over
        /// </summary>
        public const string PlockAndUp = SuperUser + ", " + Plock;

        #endregion Prefixed grouped role keys

        /// <summary>
        /// All available roles
        /// </summary>
        public static List<string> AllRoles => new List<string>() { SuperUser, Gruppchef, TransportLedare, Medarbetare, Plock, PlockAnalys };
    }
}