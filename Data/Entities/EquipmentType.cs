using System.Collections.Generic;

namespace EquipmentAccountingIS.App.Data.Entities;

public class EquipmentType
{
    public EquipmentType()
    {
        UnitInfos = new HashSet<UnitInfo>();
    }

    public int EquipmentTypeId { get; set; }
    public string EquipmentTypeName { get; set; } = null!;

    public virtual ICollection<UnitInfo> UnitInfos { get; set; }
}