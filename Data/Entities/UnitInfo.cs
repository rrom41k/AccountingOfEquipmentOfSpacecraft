using System.Collections.Generic;

namespace EquipmentAccountingIS.App.Data.Entities;

public class UnitInfo
{
    public UnitInfo()
    {
        StoredUnits = new HashSet<StoredUnit>();
    }

    public int UnitInfoId { get; set; }
    public byte[]? Photo { get; set; }
    public string EquipmentName { get; set; } = null!;
    public string EquipmentCodename { get; set; } = null!;
    public string EquipmentPassport { get; set; } = null!;
    public int EquipmentType { get; set; }

    public virtual EquipmentType EquipmentTypeNavigation { get; set; } = null!;
    public virtual ICollection<StoredUnit> StoredUnits { get; set; }
}