namespace PropMan.Models.Dto
{
    public class DashboardSummary
    {
        public int TotalProperties { get; set; }
        public int VacantUnits { get; set; }
        public int OccupiedUnits { get; set; }
        public int PendingRepairs { get; set; }
        public List<RecentProperty>? RecentProperties { get; set; } 
    public List<RecentTenant>? RecentTenants { get; set; }
    }
}