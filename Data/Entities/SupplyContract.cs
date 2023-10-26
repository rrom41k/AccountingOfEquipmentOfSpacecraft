using System.Collections.Generic;

namespace EquipmentAccountingIS.App.Data.Entities;

public class SupplyContract
{
    public SupplyContract()
    {
        StoredUnits = new HashSet<StoredUnit>();
    }

    public int SupplyContractId { get; set; }
    public string SupplyContractCodename { get; set; } = null!;
    public int ManufacturerShortName { get; set; }

    public virtual Manufacturer ManufacturerShortNameNavigation { get; set; } = null!;
    public virtual ICollection<StoredUnit> StoredUnits { get; set; }
}