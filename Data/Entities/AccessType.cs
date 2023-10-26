using System.Collections.Generic;

namespace EquipmentAccountingIS.App.Data.Entities;

public class AccessType
{
    public AccessType()
    {
        ResponsibleEmployees = new HashSet<ResponsibleEmployee>();
    }

    public int AccessTypeId { get; set; }
    public string AccessTypeName { get; set; } = null!;

    public virtual ICollection<ResponsibleEmployee> ResponsibleEmployees { get; set; }
}