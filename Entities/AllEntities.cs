using Microsoft.AspNetCore.Identity;

namespace PermitSystem.Web.Entities;

// ═══════════════════════════════════════════════════
//  Base Entity
// ═══════════════════════════════════════════════════
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
}

// ═══════════════════════════════════════════════════
//  Lookups
// ═══════════════════════════════════════════════════
public class LookupCategory : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public bool IsSystem { get; set; } = false;
    public int SortOrder { get; set; } = 0;
    public ICollection<LookupItem> Items { get; set; } = new List<LookupItem>();
}

public class LookupItem : BaseEntity
{
    public int CategoryId { get; set; }
    public LookupCategory Category { get; set; } = null!;
    public string Value { get; set; } = string.Empty;
    public string? GroupKey { get; set; }
    public int SortOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}

// ═══════════════════════════════════════════════════
//  Branch
// ═══════════════════════════════════════════════════
public class Branch : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? Manager { get; set; }
    public string? Phone { get; set; }
    public string? WorkingDays { get; set; }
    public TimeSpan? WorkingFrom { get; set; }
    public TimeSpan? WorkingTo { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Building> Buildings { get; set; } = new List<Building>();
    public ICollection<Department> Departments { get; set; } = new List<Department>();
    public ICollection<Person> Persons { get; set; } = new List<Person>();
    public ICollection<Permit> Permits { get; set; } = new List<Permit>();
}

// ═══════════════════════════════════════════════════
//  Building
// ═══════════════════════════════════════════════════
public class Building : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int? BuildingTypeId { get; set; }
    public LookupItem? BuildingType { get; set; }
    public int Floors { get; set; } = 1;
    public string? Manager { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; } = true;
    public int BranchId { get; set; }
    public Branch Branch { get; set; } = null!;

    public ICollection<Gate> Gates { get; set; } = new List<Gate>();
    public ICollection<PermitPath> Paths { get; set; } = new List<PermitPath>();
}

// ═══════════════════════════════════════════════════
//  Gate + Device
// ═══════════════════════════════════════════════════
public class Gate : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int? LocationId { get; set; }
    public LookupItem? Location { get; set; }
    public bool IsActive { get; set; } = true;
    public int BuildingId { get; set; }
    public Building Building { get; set; } = null!;
    public ICollection<GateDevice> Devices { get; set; } = new List<GateDevice>();
}

public class GateDevice : BaseEntity
{
    public string? Name { get; set; }
    public string MacAddress { get; set; } = string.Empty;
    public DateTime? LastSeen { get; set; }
    public bool IsActive { get; set; } = true;
    public int GateId { get; set; }
    public Gate Gate { get; set; } = null!;
}

// ═══════════════════════════════════════════════════
//  Permit Path
// ═══════════════════════════════════════════════════
public class PermitPath : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int BranchId { get; set; }
    public Branch Branch { get; set; } = null!;
    public int BuildingId { get; set; }
    public Building Building { get; set; } = null!;
    public ICollection<PermitPathGate> PathGates { get; set; } = new List<PermitPathGate>();
}

public class PermitPathGate
{
    public int PermitPathId { get; set; }
    public PermitPath PermitPath { get; set; } = null!;
    public int GateId { get; set; }
    public Gate Gate { get; set; } = null!;
}

// ═══════════════════════════════════════════════════
//  Department
// ═══════════════════════════════════════════════════
public class Department : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int Level { get; set; } = 1;
    public string? Manager { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;
    public int BranchId { get; set; }
    public Branch Branch { get; set; } = null!;
    public int? ParentId { get; set; }
    public Department? Parent { get; set; }
    public ICollection<Department> Children { get; set; } = new List<Department>();
}

// ═══════════════════════════════════════════════════
//  Person
// ═══════════════════════════════════════════════════
public class Person : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string IdNumber { get; set; } = string.Empty;
    public string MaskedId { get; set; } = string.Empty;
    public string? Entity { get; set; }
    public int PersonTypeId { get; set; }
    public LookupItem PersonType { get; set; } = null!;
    public int IdTypeId { get; set; }
    public LookupItem IdType { get; set; } = null!;
    public int NationalityId { get; set; }
    public LookupItem Nationality { get; set; } = null!;
    public string? PhotoPath { get; set; }
    public bool IsActive { get; set; } = true;
    public int BranchId { get; set; }
    public Branch Branch { get; set; } = null!;
    public string? AddedByUserId { get; set; }
    public ApplicationUser? AddedByUser { get; set; }
}

// ═══════════════════════════════════════════════════
//  Permit
// ═══════════════════════════════════════════════════
public class Permit : BaseEntity
{
    public string PermitNumber { get; set; } = string.Empty;
    public int Mode { get; set; }
    public int Status { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Sub { get; set; }
    public string? MaskedId { get; set; }

    public string? PlateNumbers { get; set; }
    public string? PlateLetters { get; set; }
    public int? VehicleMakeId { get; set; }
    public LookupItem? VehicleMake { get; set; }
    public int? VehicleModelId { get; set; }
    public LookupItem? VehicleModel { get; set; }
    public int? VehicleYear { get; set; }
    public int? VehicleColorId { get; set; }
    public LookupItem? VehicleColor { get; set; }
    public int? DriverPersonId { get; set; }
    public Person? DriverPerson { get; set; }

    public string? RelatedEntryPermitNumber { get; set; }

    public int BranchId { get; set; }
    public Branch Branch { get; set; } = null!;
    public int BuildingId { get; set; }
    public Building Building { get; set; } = null!;
    public int? PermitPathId { get; set; }
    public PermitPath? PermitPath { get; set; }
    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }
    public string RequestingDeptName { get; set; } = string.Empty;

    public DateTime VisitDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string? RequesterNotes { get; set; }
    public string? GuardNotes { get; set; }
    public string? ApprovalNote { get; set; }
    public string? SuspendReason { get; set; }
    public string? GroupReference { get; set; }

    public string? SubmitterUserId { get; set; }
    public ApplicationUser? SubmitterUser { get; set; }
    public string? ApprovedByUserId { get; set; }
    public ApplicationUser? ApprovedByUser { get; set; }
    public DateTime? ApprovedAt { get; set; }

    public ICollection<PermitPerson> PermitPersons { get; set; } = new List<PermitPerson>();
    public ICollection<PermitDevice> PermitDevices { get; set; } = new List<PermitDevice>();
    public ICollection<ExitItem> ExitItems { get; set; } = new List<ExitItem>();
}

public class PermitPerson : BaseEntity
{
    public int PermitId { get; set; }
    public Permit Permit { get; set; } = null!;
    public int PersonId { get; set; }
    public Person Person { get; set; } = null!;
    public bool IsApproved { get; set; } = false;
    public string? ApproverNote { get; set; }
}

public class PermitDevice : BaseEntity
{
    public int PermitId { get; set; }
    public Permit Permit { get; set; } = null!;
    public int? DeviceLookupId { get; set; }
    public LookupItem? DeviceLookup { get; set; }
}

public class ExitItem : BaseEntity
{
    public int PermitId { get; set; }
    public Permit Permit { get; set; } = null!;
    public int? CategoryId { get; set; }
    public LookupItem? Category { get; set; }
    public int? TypeId { get; set; }
    public LookupItem? Type { get; set; }
    public int Quantity { get; set; } = 1;
    public string? Detail { get; set; }
}

// ═══════════════════════════════════════════════════
//  Blacklist
// ═══════════════════════════════════════════════════
public class Blacklist : BaseEntity
{
    public int BranchId { get; set; }
    public Branch Branch { get; set; } = null!;
    public string IdNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int? NationalityId { get; set; }
    public LookupItem? Nationality { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? AddedByUserId { get; set; }
}

public class BannedNationality : BaseEntity
{
    public int BranchId { get; set; }
    public Branch Branch { get; set; } = null!;
    public int NationalityId { get; set; }
    public LookupItem Nationality { get; set; } = null!;
}

// ═══════════════════════════════════════════════════
//  Application User (Identity)
// ═══════════════════════════════════════════════════
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string? JobTitle { get; set; }
    public string? Avatar { get; set; }
    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }
    public string RoleType { get; set; } = "requester";
    public bool IsActive { get; set; } = true;
}

// ═══════════════════════════════════════════════════
//  Screen (for permissions)
// ═══════════════════════════════════════════════════
public class Screen : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? GroupName { get; set; }
    public string? Category { get; set; }
    public int SortOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}

public class RoleScreenPermission
{
    public int Id { get; set; }
    public string RoleId { get; set; } = null!;
    public int ScreenId { get; set; }
    public Screen Screen { get; set; } = null!;
}

// ═══════════════════════════════════════════════════
//  System Settings + Theme
// ═══════════════════════════════════════════════════
public class SystemSetting : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string? Value { get; set; }
    public string DataType { get; set; } = "string";
    public string? Group { get; set; }
    public string? DisplayName { get; set; }
}

public class Theme : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ColorDark { get; set; } = "#0f2d1f";
    public string ColorPrimary { get; set; } = "#15573a";
    public string ColorPrimaryLight { get; set; } = "#1e7a4f";
    public string ColorPale { get; set; } = "#edf7f2";
    public string ColorLight { get; set; } = "#d6f0e2";
    public string ColorGold { get; set; } = "#a07820";
    public string ColorGoldLight { get; set; } = "#c9941e";
    public string ColorGoldBg { get; set; } = "#fdf8ee";
    public bool IsDefault { get; set; } = false;
    public bool IsActive { get; set; } = true;
}

// ═══════════════════════════════════════════════════
//  Logs
// ═══════════════════════════════════════════════════
public class AuditLog
{
    public long Id { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class QueryLog
{
    public long Id { get; set; }
    public string SearchValue { get; set; } = string.Empty;
    public string? PersonName { get; set; }
    public string? Nationality { get; set; }
    public int? GateId { get; set; }
    public string? GateName { get; set; }
    public string ResultType { get; set; } = string.Empty;
    public string? ResultLabel { get; set; }
    public string? OfficerId { get; set; }
    public string? OfficerName { get; set; }
    public int BranchId { get; set; }
    public string? ActionTaken { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}