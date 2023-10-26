using System.Collections.Generic;

namespace EquipmentAccountingIS.App.Data.Entities;

public class Manufacturer
{
    public Manufacturer()
    {
        SupplyContracts = new HashSet<SupplyContract>();
    }

    public int ManufacturerId { get; set; }
    public string ManufacturerShortName { get; set; } = null!;
    public string? ManufacturerFullName { get; set; }

    public virtual ICollection<SupplyContract> SupplyContracts { get; set; }
}