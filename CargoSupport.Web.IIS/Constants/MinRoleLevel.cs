using System.Collections.Generic;

namespace CargoSupport.Constants
{
    public static class MinRoleLevel
    {
        public const string SuperUser = "SuperUser";
        public const string Gruppchef = "GC_trp";
        public const string TransportLedare = "TL_trp";
        public const string Medarbetare = "Medarbetare_Trp";
        public const string Plock = "Plock";

        public const string SuperUserAndUp = SuperUser;
        public const string GruppChefAndUp = SuperUser + "," + Gruppchef;
        public const string TransportLedareAndUp = SuperUser + "," + Gruppchef + "," + TransportLedare;
        public const string MedarbetareAndUp = SuperUser + "," + Gruppchef + "," + TransportLedare + "," + Medarbetare;
        public const string PlockAndUp = SuperUser + "," + Plock;

        public static List<string> AllRoles => new List<string>() { SuperUser, Gruppchef, TransportLedare, Medarbetare, Plock };
    }
}