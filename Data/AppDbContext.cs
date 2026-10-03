using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PermitSystem.Web.Entities;

namespace PermitSystem.Web.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<LookupCategory> LookupCategories => Set<LookupCategory>();
    public DbSet<LookupItem> LookupItems => Set<LookupItem>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Building> Buildings => Set<Building>();
    public DbSet<Gate> Gates => Set<Gate>();
    public DbSet<GateDevice> GateDevices => Set<GateDevice>();
    public DbSet<PermitPath> PermitPaths => Set<PermitPath>();
    public DbSet<PermitPathGate> PermitPathGates => Set<PermitPathGate>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Person> Persons => Set<Person>();
    public DbSet<Permit> Permits => Set<Permit>();
    public DbSet<PermitPerson> PermitPersons => Set<PermitPerson>();
    public DbSet<PermitDevice> PermitDevices => Set<PermitDevice>();
    public DbSet<ExitItem> ExitItems => Set<ExitItem>();
    public DbSet<Blacklist> Blacklists => Set<Blacklist>();
    public DbSet<BannedNationality> BannedNationalities => Set<BannedNationality>();
    public DbSet<Screen> Screens => Set<Screen>();
    public DbSet<RoleScreenPermission> RoleScreenPermissions => Set<RoleScreenPermission>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<Theme> Themes => Set<Theme>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<QueryLog> QueryLogs => Set<QueryLog>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        // ═══ Soft delete filters ═══
        b.Entity<Branch>().HasQueryFilter(e => !e.IsDeleted);
        b.Entity<Building>().HasQueryFilter(e => !e.IsDeleted);
        b.Entity<Gate>().HasQueryFilter(e => !e.IsDeleted);
        b.Entity<Person>().HasQueryFilter(e => !e.IsDeleted);
        b.Entity<Permit>().HasQueryFilter(e => !e.IsDeleted);
        b.Entity<LookupCategory>().HasQueryFilter(e => !e.IsDeleted);
        b.Entity<LookupItem>().HasQueryFilter(e => !e.IsDeleted);
        b.Entity<SystemSetting>().HasQueryFilter(e => !e.IsDeleted);
        b.Entity<Theme>().HasQueryFilter(e => !e.IsDeleted);
        b.Entity<Screen>().HasQueryFilter(e => !e.IsDeleted);

        // ═══ Unique indexes ═══
        b.Entity<Branch>().HasIndex(x => x.Code).IsUnique();
        b.Entity<LookupCategory>().HasIndex(x => x.Key).IsUnique();
        b.Entity<SystemSetting>().HasIndex(x => x.Key).IsUnique();
        b.Entity<Theme>().HasIndex(x => x.Key).IsUnique();
        b.Entity<Screen>().HasIndex(x => x.Key).IsUnique();
        b.Entity<GateDevice>().HasIndex(x => x.MacAddress).IsUnique();
        b.Entity<Permit>().HasIndex(x => x.PermitNumber).IsUnique();

        // ═══ Composite keys ═══
        b.Entity<PermitPathGate>().HasKey(x => new { x.PermitPathId, x.GateId });
        b.Entity<RoleScreenPermission>().HasIndex(x => new { x.RoleId, x.ScreenId }).IsUnique();

        // ═══ Relationships ═══
        b.Entity<Building>()
            .HasOne(x => x.Branch).WithMany(x => x.Buildings)
            .HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);

        b.Entity<Gate>()
            .HasOne(x => x.Building).WithMany(x => x.Gates)
            .HasForeignKey(x => x.BuildingId).OnDelete(DeleteBehavior.Cascade);

        b.Entity<GateDevice>()
            .HasOne(x => x.Gate).WithMany(x => x.Devices)
            .HasForeignKey(x => x.GateId).OnDelete(DeleteBehavior.Cascade);

        b.Entity<Person>()
            .HasOne(x => x.Branch).WithMany(x => x.Persons)
            .HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);

        b.Entity<Person>()
            .HasOne(x => x.PersonType).WithMany()
            .HasForeignKey(x => x.PersonTypeId).OnDelete(DeleteBehavior.Restrict);

        b.Entity<Person>()
            .HasOne(x => x.IdType).WithMany()
            .HasForeignKey(x => x.IdTypeId).OnDelete(DeleteBehavior.Restrict);

        b.Entity<Person>()
            .HasOne(x => x.Nationality).WithMany()
            .HasForeignKey(x => x.NationalityId).OnDelete(DeleteBehavior.Restrict);

        b.Entity<Permit>()
            .HasOne(x => x.Branch).WithMany(x => x.Permits)
            .HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);

        b.Entity<Permit>()
            .HasOne(x => x.Building).WithMany()
            .HasForeignKey(x => x.BuildingId).OnDelete(DeleteBehavior.Restrict);

        b.Entity<Permit>()
            .HasOne(x => x.PermitPath).WithMany()
            .HasForeignKey(x => x.PermitPathId).OnDelete(DeleteBehavior.SetNull);

        b.Entity<Permit>()
            .HasOne(x => x.Department).WithMany()
            .HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.SetNull);

        b.Entity<PermitPerson>()
            .HasOne(x => x.Permit).WithMany(x => x.PermitPersons)
            .HasForeignKey(x => x.PermitId).OnDelete(DeleteBehavior.Cascade);

        b.Entity<PermitPerson>()
            .HasOne(x => x.Person).WithMany()
            .HasForeignKey(x => x.PersonId).OnDelete(DeleteBehavior.Restrict);

        b.Entity<PermitDevice>()
            .HasOne(x => x.Permit).WithMany(x => x.PermitDevices)
            .HasForeignKey(x => x.PermitId).OnDelete(DeleteBehavior.Cascade);

        b.Entity<ExitItem>()
            .HasOne(x => x.Permit).WithMany(x => x.ExitItems)
            .HasForeignKey(x => x.PermitId).OnDelete(DeleteBehavior.Cascade);

        b.Entity<Blacklist>()
            .HasOne(x => x.Branch).WithMany()
            .HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Cascade);

        b.Entity<BannedNationality>()
            .HasOne(x => x.Branch).WithMany()
            .HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Cascade);

        b.Entity<Department>()
            .HasOne(x => x.Parent).WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentId).OnDelete(DeleteBehavior.Restrict);

        b.Entity<RoleScreenPermission>()
            .HasOne(x => x.Screen).WithMany()
            .HasForeignKey(x => x.ScreenId).OnDelete(DeleteBehavior.Cascade);
    }
}