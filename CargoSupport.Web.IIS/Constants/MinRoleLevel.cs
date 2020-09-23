using System.Collections.Generic;

namespace CargoSupport.Constants
{
    public static class MinRoleLevel
    {
        private const string SuperUser = "SuperUser";
        private const string Gruppchef = "GC_trp";
        private const string TransportLedare = "TL_trp";
        private const string Medarbetare = "Medarbetare_Trp";
        private const string Plock = "Plock";

        public const string SuperUserAndUp = SuperUser;
        public const string GruppChefAndUp = SuperUser + "," + Gruppchef;
        public const string TransportLedareAndUp = SuperUser + "," + Gruppchef + "," + TransportLedare;
        public const string MedarbetareAndUp = SuperUser + "," + Gruppchef + "," + TransportLedare + "," + Medarbetare;
        public const string PlockAndUp = SuperUser + "," + Plock;

        public static List<string> AllRoles => new List<string>() { SuperUser, Gruppchef, TransportLedare, Medarbetare, Plock };
    }
}