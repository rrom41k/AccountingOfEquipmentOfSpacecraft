using System.Collections.Generic;

namespace EquipmentAccountingIS.App.Data.Entities;

public class ResponsibleEmployee
{
    public ResponsibleEmployee()
    {
        StoredUnits = new HashSet<StoredUnit>();
    }

    public int EmployeeId { get; set; }
    public string EmployeeLogin { get; set; } = null!;
    public string EmployeePassword { get; set; } = null!;
    public string EmployeeFirstName { get; set; } = null!;
    public string EmployeeSecondName { get; set; } = null!;
    public string? EmployeeLastName { get; set; }
    public string EmployeeInitials { get; set; } = null!;
    public int AccessType { get; set; }
    public int Post { get; set; }

    public virtual AccessType AccessTypeNavigation { get; set; } = null!;
    public virtual Post PostNavigation { get; set; } = null!;
    public virtual ICollection<StoredUnit> StoredUnits { get; set; }
}