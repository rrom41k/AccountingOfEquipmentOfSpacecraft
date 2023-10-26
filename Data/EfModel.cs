using EquipmentAccountingIS.App.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EquipmentAccountingIS.App.Data;

public partial class EfModel : DbContext
{
    private static EfModel _Instance;

    public EfModel()
    {
    }

    public EfModel(DbContextOptions<EfModel> options)
        : base(options)
    {
    }

    public virtual DbSet<AccessType> AccessTypes { get; set; } = null!;
    public virtual DbSet<EquipmentType> EquipmentTypes { get; set; } = null!;
    public virtual DbSet<Manufacturer> Manufacturers { get; set; } = null!;
    public virtual DbSet<Post> Posts { get; set; } = null!;
    public virtual DbSet<ResponsibleEmployee> ResponsibleEmployees { get; set; } = null!;
    public virtual DbSet<StoredUnit> StoredUnits { get; set; } = null!;
    public virtual DbSet<SupplyContract> SupplyContracts { get; set; } = null!;
    public virtual DbSet<UnitInfo> UnitInfos { get; set; } = null!;

    public static EfModel Init()
    {
        if (_Instance == null)
            _Instance = new EfModel();
        return _Instance;
        //singletone для избежания null в экземпляре ef модели + удобство
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https: //go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
            optionsBuilder.UseMySql(
                "server=localhost;port=3306;user id=Admin;password=;database=dbspace;character set=utf8",
                ServerVersion.Parse("8.0.32-mysql"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<AccessType>(entity =>
        {
            entity.Property(e => e.AccessTypeId).HasColumnName("AccessType_id");

            entity.Property(e => e.AccessTypeName).HasMaxLength(20);
        });

        modelBuilder.Entity<EquipmentType>(entity =>
        {
            entity.Property(e => e.EquipmentTypeId).HasColumnName("EquipmentType_id");

            entity.Property(e => e.EquipmentTypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.Property(e => e.ManufacturerId).HasColumnName("Manufacturer_id");

            entity.Property(e => e.ManufacturerFullName).HasMaxLength(150);

            entity.Property(e => e.ManufacturerShortName).HasMaxLength(45);
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.Property(e => e.PostId).HasColumnName("Post_id");

            entity.Property(e => e.PostName).HasMaxLength(60);
        });

        modelBuilder.Entity<ResponsibleEmployee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId)
                .HasName("PRIMARY");

            entity.HasIndex(e => e.AccessType, "fk_ResponsibleEmployees_AccessTypes1_idx");

            entity.HasIndex(e => e.Post, "fk_ResponsibleEmployees_Posts1_idx");

            entity.Property(e => e.EmployeeId).HasColumnName("Employee_id");

            entity.Property(e => e.EmployeeFirstName).HasMaxLength(30);

            entity.Property(e => e.EmployeeInitials).HasMaxLength(40);

            entity.Property(e => e.EmployeeLastName).HasMaxLength(30);

            entity.Property(e => e.EmployeeLogin).HasMaxLength(50);

            entity.Property(e => e.EmployeePassword).HasMaxLength(50);

            entity.Property(e => e.EmployeeSecondName).HasMaxLength(30);

            entity.HasOne(d => d.AccessTypeNavigation)
                .WithMany(p => p.ResponsibleEmployees)
                .HasForeignKey(d => d.AccessType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ResponsibleEmployees_AccessTypes1");

            entity.HasOne(d => d.PostNavigation)
                .WithMany(p => p.ResponsibleEmployees)
                .HasForeignKey(d => d.Post)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ResponsibleEmployees_Posts1");
        });

        modelBuilder.Entity<StoredUnit>(entity =>
        {
            entity.HasKey(e => e.SunitId)
                .HasName("PRIMARY");

            entity.HasIndex(e => e.WhoAdded, "fk_StoredUnits_ResponsibleEmployees1_idx");

            entity.HasIndex(e => e.SupplyContract, "fk_StoredUnits_SupplyContracts1_idx1");

            entity.HasIndex(e => e.UnitInfo, "fk_StoredUnits_UnitInfo1_idx1");

            entity.Property(e => e.SunitId).HasColumnName("SUnit_id");

            entity.Property(e => e.DateOfWriteOff).HasColumnType("datetime");

            entity.Property(e => e.DeliveryDate).HasColumnType("datetime");

            entity.Property(e => e.Note).HasMaxLength(200);

            entity.Property(e => e.Purpose).HasMaxLength(325);

            entity.Property(e => e.SignOfDeleting)
                .HasMaxLength(1)
                .IsFixedLength();

            entity.HasOne(d => d.SupplyContractNavigation)
                .WithMany(p => p.StoredUnits)
                .HasForeignKey(d => d.SupplyContract)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_StoredUnits_SupplyContracts1");

            entity.HasOne(d => d.UnitInfoNavigation)
                .WithMany(p => p.StoredUnits)
                .HasForeignKey(d => d.UnitInfo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_StoredUnits_UnitInfo1");

            entity.HasOne(d => d.WhoAddedNavigation)
                .WithMany(p => p.StoredUnits)
                .HasForeignKey(d => d.WhoAdded)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_StoredUnits_ResponsibleEmployees1");
        });

        modelBuilder.Entity<SupplyContract>(entity =>
        {
            entity.HasIndex(e => e.ManufacturerShortName, "fk_SupplyContracts_Manufacturers1_idx");

            entity.Property(e => e.SupplyContractId).HasColumnName("SupplyContract_id");

            entity.Property(e => e.SupplyContractCodename).HasMaxLength(40);

            entity.HasOne(d => d.ManufacturerShortNameNavigation)
                .WithMany(p => p.SupplyContracts)
                .HasForeignKey(d => d.ManufacturerShortName)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_SupplyContracts_Manufacturers1");
        });

        modelBuilder.Entity<UnitInfo>(entity =>
        {
            entity.ToTable("UnitInfo");

            entity.HasIndex(e => e.EquipmentType, "fk_UnitInfo_EquipmentTypes1_idx");

            entity.Property(e => e.UnitInfoId).HasColumnName("UnitInfo_id");

            entity.Property(e => e.EquipmentCodename).HasMaxLength(30);

            entity.Property(e => e.EquipmentName).HasMaxLength(175);

            entity.Property(e => e.EquipmentPassport).HasMaxLength(40);

            entity.HasOne(d => d.EquipmentTypeNavigation)
                .WithMany(p => p.UnitInfos)
                .HasForeignKey(d => d.EquipmentType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_UnitInfo_EquipmentTypes1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}