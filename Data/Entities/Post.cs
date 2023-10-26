using System.Collections.Generic;

namespace EquipmentAccountingIS.App.Data.Entities;

public class Post
{
    public Post()
    {
        ResponsibleEmployees = new HashSet<ResponsibleEmployee>();
    }

    public int PostId { get; set; }
    public string PostName { get; set; } = null!;

    public virtual ICollection<ResponsibleEmployee> ResponsibleEmployees { get; set; }
}