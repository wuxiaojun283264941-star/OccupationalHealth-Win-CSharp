using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OccupationalHealth.Api.Models;

// ============================================================
// 统一用户表
// ============================================================
public class User
{
    [Key]
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Role { get; set; } = ""; // admin, factory, health_agent, c_unit
    public string Name { get; set; } = "";
    public string OrgName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Status { get; set; } = "active";
    public int? OrgId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<FactoryContact> FactoryContacts { get; set; } = new List<FactoryContact>();
    public ICollection<Position> Positions { get; set; } = new List<Position>();
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}

// ============================================================
// 工厂联系人表
// ============================================================
public class FactoryContact
{
    [Key]
    public int Id { get; set; }
    public int FactoryId { get; set; }
    public string Name { get; set; } = "";
    public string Position { get; set; } = "";
    public string Phone { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("FactoryId")]
    public User? Factory { get; set; }
}

// ============================================================
// 岗位表 (自引用树)
// ============================================================
public class Position
{
    [Key]
    public int Id { get; set; }
    public int FactoryId { get; set; }
    public int? ParentId { get; set; }
    public string Name { get; set; } = "";
    public string Level { get; set; } = ""; // workshop, section, position
    public int? OccupationId { get; set; }
    public int OrderIndex { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("FactoryId")]
    public User? Factory { get; set; }
    [ForeignKey("ParentId")]
    public Position? Parent { get; set; }
    public ICollection<Position> Children { get; set; } = new List<Position>();
    [ForeignKey("OccupationId")]
    public Occupation? Occupation { get; set; }
    public ICollection<PositionHazard> PositionHazards { get; set; } = new List<PositionHazard>();
}

// ============================================================
// 危害因素表
// ============================================================
public class HazardFactor
{
    [Key]
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string Category { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string ExamFrequency { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// ============================================================
// 职业字典表
// ============================================================
public class Occupation
{
    [Key]
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<OccupationHazard> OccupationHazards { get; set; } = new List<OccupationHazard>();
}

// ============================================================
// 职业-危害因素关联表
// ============================================================
public class OccupationHazard
{
    [Key]
    public int Id { get; set; }
    public int OccupationId { get; set; }
    public int HazardFactorId { get; set; }

    [ForeignKey("OccupationId")]
    public Occupation? Occupation { get; set; }
    [ForeignKey("HazardFactorId")]
    public HazardFactor? HazardFactor { get; set; }
}

// ============================================================
// 岗位-危害因素绑定表
// ============================================================
public class PositionHazard
{
    [Key]
    public int Id { get; set; }
    public int PositionId { get; set; }
    public int HazardFactorId { get; set; }
    public string Source { get; set; } = "direct"; // direct, occupation, workshop

    [ForeignKey("PositionId")]
    public Position? Position { get; set; }
    [ForeignKey("HazardFactorId")]
    public HazardFactor? HazardFactor { get; set; }
}

// ============================================================
// 员工表
// ============================================================
public class Employee
{
    [Key]
    public int Id { get; set; }
    public int FactoryId { get; set; }
    public string Name { get; set; } = "";
    public string Gender { get; set; } = "";
    public DateTime? BirthDate { get; set; }
    public string IdCard { get; set; } = "";
    public string Phone { get; set; } = "";
    public int? PositionId { get; set; }
    public DateTime? EntryDate { get; set; }
    public DateTime? HazardStartDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("FactoryId")]
    public User? Factory { get; set; }
    [ForeignKey("PositionId")]
    public Position? Position { get; set; }
}

// ============================================================
// 体检任务表
// ============================================================
public class ExamTask
{
    [Key]
    public int Id { get; set; }
    public int FactoryId { get; set; }
    public int HealthAgentId { get; set; }
    public int? FactoryContactId { get; set; }
    public string ExamType { get; set; } = "periodic";
    public string Status { get; set; } = "pushed";
    public DateTime PushedAt { get; set; } = DateTime.UtcNow;
    public DateTime? AcceptedAt { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("FactoryId")]
    public User? Factory { get; set; }
    [ForeignKey("HealthAgentId")]
    public User? HealthAgent { get; set; }
    [ForeignKey("FactoryContactId")]
    public FactoryContact? FactoryContact { get; set; }
    public ICollection<ExamTaskEmployee> ExamTaskEmployees { get; set; } = new List<ExamTaskEmployee>();
}

// ============================================================
// 体检任务-员工关联表
// ============================================================
public class ExamTaskEmployee
{
    [Key]
    public int Id { get; set; }
    public int ExamTaskId { get; set; }
    public int EmployeeId { get; set; }
    public string ExamStatus { get; set; } = "pending";

    [ForeignKey("ExamTaskId")]
    public ExamTask? ExamTask { get; set; }
    [ForeignKey("EmployeeId")]
    public Employee? Employee { get; set; }
}

// ============================================================
// 体检报告表
// ============================================================
public class ExamReport
{
    [Key]
    public int Id { get; set; }
    public int ExamTaskId { get; set; }
    public int EmployeeId { get; set; }
    public string FilePath { get; set; } = "";
    public string OriginalName { get; set; } = "";
    public long FileSize { get; set; }
    public int UploadedBy { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("ExamTaskId")]
    public ExamTask? ExamTask { get; set; }
    [ForeignKey("EmployeeId")]
    public Employee? Employee { get; set; }
    [ForeignKey("UploadedBy")]
    public User? Uploader { get; set; }
}

// ============================================================
// 体检套餐模板表
// ============================================================
public class ExamPackage
{
    [Key]
    public int Id { get; set; }
    public int HealthAgentId { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Price { get; set; }
    public string ExamItems { get; set; } = "";
    public int IsActive { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("HealthAgentId")]
    public User? HealthAgent { get; set; }
}

// ============================================================
// 短信验证码表
// ============================================================
public class SmsCode
{
    [Key]
    public int Id { get; set; }
    public string Phone { get; set; } = "";
    public string Code { get; set; } = "";
    public DateTime ExpireAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
