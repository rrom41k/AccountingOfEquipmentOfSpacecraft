using System;

namespace EquipmentAccountingIS.App.Data.Entities;

public class StoredUnit
{
    public int SunitId { get; set; }
    public int Quantity { get; set; }
    public DateTime DeliveryDate { get; set; }
    public string Purpose { get; set; } = null!;
    public DateTime? DateOfWriteOff { get; set; }
    public string? Note { get; set; }
    public int WhoAdded { get; set; }
    public int SupplyContract { get; set; }
    public int UnitInfo { get; set; }
    public string? SignOfDeleting { get; set; }

    public virtual SupplyContract SupplyContractNavigation { get; set; } = null!;
    public virtual UnitInfo UnitInfoNavigation { get; set; } = null!;
    public virtual ResponsibleEmployee WhoAddedNavigation { get; set; } = null!;
}