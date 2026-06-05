namespace OccupationalHealth.Api.Models;

// === Generic Response ===
public class ApiResponse<T>
{
    public int Code { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = "success";
}

public class PaginatedData<T>
{
    public List<T> List { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

// === Auth DTOs ===
public class LoginRequest
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Phone { get; set; }
    public string? Code { get; set; }
}

public class SendCodeRequest
{
    public string Phone { get; set; } = "";
}

public class LoginResponse
{
    public string Token { get; set; } = "";
    public UserInfo User { get; set; } = new();
}

public class UserInfo
{
    public int Id { get; set; }
    public string Role { get; set; } = "";
    public string Name { get; set; } = "";
    public string Username { get; set; } = "";
    public string OrgName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Status { get; set; } = "";
}

// === User Management DTOs ===
public class CreateUserRequest
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string Role { get; set; } = "";
    public string Name { get; set; } = "";
    public string? OrgName { get; set; }
    public string? Phone { get; set; }
}

public class UpdateUserRequest
{
    public string? Name { get; set; }
    public string? OrgName { get; set; }
    public string? Phone { get; set; }
}

public class ResetPasswordRequest
{
    public string Password { get; set; } = "";
}

public class UpdateStatusRequest
{
    public string Status { get; set; } = "";
}

// === Hazard Factor DTOs ===
public class CreateHazardFactorRequest
{
    public string Code { get; set; } = "";
    public string Category { get; set; } = "";
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public string? ExamFrequency { get; set; }
}

public class UpdateHazardFactorRequest
{
    public string? Code { get; set; }
    public string? Category { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ExamFrequency { get; set; }
}

// === Occupation DTOs ===
public class CreateOccupationRequest
{
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string? Description { get; set; }
}

public class UpdateOccupationRequest
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public class BindHazardsRequest
{
    public List<int> HazardFactorIds { get; set; } = new();
}

// === Position DTOs ===
public class CreatePositionRequest
{
    public int? ParentId { get; set; }
    public string Name { get; set; } = "";
    public string Level { get; set; } = "";
    public int? OccupationId { get; set; }
}

public class UpdatePositionRequest
{
    public string? Name { get; set; }
    public int? ParentId { get; set; }
    public int? OccupationId { get; set; }
    public int? OrderIndex { get; set; }
}

// === Employee DTOs ===
public class CreateEmployeeRequest
{
    public string Name { get; set; } = "";
    public string? Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public string IdCard { get; set; } = "";
    public string? Phone { get; set; }
    public int? PositionId { get; set; }
    public DateTime? EntryDate { get; set; }
    public DateTime? HazardStartDate { get; set; }
}

public class UpdateEmployeeRequest
{
    public string? Name { get; set; }
    public string? Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? IdCard { get; set; }
    public string? Phone { get; set; }
    public int? PositionId { get; set; }
    public DateTime? EntryDate { get; set; }
    public DateTime? HazardStartDate { get; set; }
}

// === Factory Contact DTOs ===
public class CreateFactoryContactRequest
{
    public string Name { get; set; } = "";
    public string? Position { get; set; }
    public string Phone { get; set; } = "";
}

public class UpdateFactoryContactRequest : CreateFactoryContactRequest { }

// === Exam Task DTOs ===
public class PushExamTaskRequest
{
    public int HealthAgentId { get; set; }
    public int? FactoryContactId { get; set; }
    public List<int> EmployeeIds { get; set; } = new();
    public string ExamType { get; set; } = "periodic";
}

public class ScheduleExamRequest
{
    public DateTime ScheduledDate { get; set; }
}

// === Exam Package DTOs ===
public class CreateExamPackageRequest
{
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ExamItems { get; set; }
}

public class UpdateExamPackageRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public string? ExamItems { get; set; }
    public int? IsActive { get; set; }
}

// === Dashboard DTOs ===
public class AdminDashboard
{
    public int FactoryCount { get; set; }
    public int AgentCount { get; set; }
    public int CUnitCount { get; set; }
    public int EmployeeCount { get; set; }
    public int TaskCount { get; set; }
    public int CompletedCount { get; set; }
    public int ReportCount { get; set; }
}

public class FactoryDashboard
{
    public int EmployeeCount { get; set; }
    public int TaskCount { get; set; }
    public int PushedCount { get; set; }
    public int InProgressCount { get; set; }
    public int CompletedCount { get; set; }
    public List<RecentTask> RecentTasks { get; set; } = new();
}

public class RecentTask
{
    public int Id { get; set; }
    public string ExamType { get; set; } = "";
    public string Status { get; set; } = "";
    public string AgentName { get; set; } = "";
    public string AgentCenter { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}

public class HealthAgentDashboard
{
    public int PendingCount { get; set; }
    public int CompletedCount { get; set; }
    public int ReportCount { get; set; }
    public int MonthCompleted { get; set; }
}

public class CUnitDashboard
{
    public int CompletedCount { get; set; }
    public int ReportCount { get; set; }
    public List<FactorySummary> ByFactory { get; set; } = new();
}

public class FactorySummary
{
    public string FactoryName { get; set; } = "";
    public int TaskCount { get; set; }
}

// === Position Tree DTO ===
public class PositionTreeNode
{
    public int Id { get; set; }
    public int FactoryId { get; set; }
    public int? ParentId { get; set; }
    public string Name { get; set; } = "";
    public string Level { get; set; } = "";
    public int? OccupationId { get; set; }
    public string? OccupationName { get; set; }
    public string? OccupationCode { get; set; }
    public int OrderIndex { get; set; }
    public int ChildCount { get; set; }
    public List<PositionTreeNode> Children { get; set; } = new();
}

// === Employee with position info ===
public class EmployeeDto
{
    public int Id { get; set; }
    public int FactoryId { get; set; }
    public string Name { get; set; } = "";
    public string Gender { get; set; } = "";
    public DateTime? BirthDate { get; set; }
    public string IdCard { get; set; } = "";
    public string Phone { get; set; } = "";
    public int? PositionId { get; set; }
    public string? PositionName { get; set; }
    public string? PositionLevel { get; set; }
    public string? PositionParentName { get; set; }
    public DateTime? EntryDate { get; set; }
    public DateTime? HazardStartDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

// === Import Result ===
public class ImportResult
{
    public int Imported { get; set; }
    public int Total { get; set; }
    public List<string> Errors { get; set; } = new();
}

// === Health Agent Info ===
public class HealthAgentInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string OrgName { get; set; } = "";
    public string Phone { get; set; } = "";
}

// === Exam Task List Item ===
public class ExamTaskDto
{
    public int Id { get; set; }
    public int FactoryId { get; set; }
    public string? FactoryName { get; set; }
    public int HealthAgentId { get; set; }
    public string? HealthAgentName { get; set; }
    public string? HealthAgentCenter { get; set; }
    public string ExamType { get; set; } = "";
    public string Status { get; set; } = "";
    public DateTime? ScheduledDate { get; set; }
    public int EmployeeCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

// === Exam Task Detail ===
public class ExamTaskDetailDto
{
    public int Id { get; set; }
    public int FactoryId { get; set; }
    public string? FactoryName { get; set; }
    public int HealthAgentId { get; set; }
    public string? HealthAgentName { get; set; }
    public string? HealthAgentCenter { get; set; }
    public string ExamType { get; set; } = "";
    public string Status { get; set; } = "";
    public DateTime? ScheduledDate { get; set; }
    public DateTime PushedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<TaskEmployeeInfo> Employees { get; set; } = new();
}

public class TaskEmployeeInfo
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = "";
    public string? IdCard { get; set; }
    public string? Phone { get; set; }
    public string? ExamStatus { get; set; }
}

// === Occupation List Item ===
public class OccupationDto
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int HazardCount { get; set; }
}

// === Hazard with binding info ===
public class HazardWithBinding
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string Category { get; set; } = "";
    public string Name { get; set; } = "";
    public int? BindingId { get; set; }
    public string? Source { get; set; }
}
