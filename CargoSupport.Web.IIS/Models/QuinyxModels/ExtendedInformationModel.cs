namespace CargoSupport.Models.QuinyxModels
{
    public class ExtendedInformationModel
    {
        public int Id { get; internal set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public int StaffCat { get; internal set; }
        public string StaffCatName { get; internal set; }
        public string ReportingTo { get; internal set; }
        public int Active { get; internal set; }
        public int Section { get; internal set; }
        public string SectionName { get; internal set; }
    }
}