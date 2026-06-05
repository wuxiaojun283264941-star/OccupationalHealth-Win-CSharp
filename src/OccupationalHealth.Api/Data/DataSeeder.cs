using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OccupationalHealth.Api.Models;

namespace OccupationalHealth.Api.Data;

public static class DataSeeder
{
    public static void Seed(AppDbContext db, IServiceProvider sp)
    {
        // === Users ===
        var users = new[]
        {
            new User { Username = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), Role = "admin", Name = "系统管理员", OrgName = "", Phone = "", Status = "active" },
            new User { Username = "factory1", PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"), Role = "factory", Name = "测试工厂A", OrgName = "测试工厂A", Phone = "13900000001", Status = "active" },
            new User { Username = "factory2", PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"), Role = "factory", Name = "测试工厂B", OrgName = "测试工厂B", Phone = "13900000002", Status = "active" },
            new User { Username = "agent1", PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"), Role = "health_agent", Name = "陈医生", OrgName = "市中心体检中心", Phone = "13800138001", Status = "active" },
            new User { Username = "agent2", PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"), Role = "health_agent", Name = "李医生", OrgName = "市第二体检中心", Phone = "13800138002", Status = "active" },
            new User { Username = "cunit1", PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"), Role = "c_unit", Name = "刘主任", OrgName = "市卫生托管中心", Phone = "13700137001", Status = "active" },
        };
        db.Users.AddRange(users);
        db.SaveChanges();

        // === Hazard Factors (46 items) ===
        var hazards = new List<HazardFactor>
        {
            // 粉尘类 10
            new() { Code = "GBZ188-F001", Category = "粉尘", Name = "矽尘（游离二氧化硅粉尘）", Description = "吸入含游离二氧化硅的粉尘引起的职业病", ExamFrequency = "1年" },
            new() { Code = "GBZ188-F002", Category = "粉尘", Name = "煤尘（煤工尘肺）", Description = "长期吸入煤尘引起的尘肺病", ExamFrequency = "2年" },
            new() { Code = "GBZ188-F003", Category = "粉尘", Name = "石棉粉尘", Description = "吸入石棉纤维引起的职业病", ExamFrequency = "1年" },
            new() { Code = "GBZ188-F004", Category = "粉尘", Name = "滑石粉尘", Description = "长期吸入滑石粉尘引起的尘肺", ExamFrequency = "2年" },
            new() { Code = "GBZ188-F005", Category = "粉尘", Name = "水泥粉尘", Description = "水泥生产过程中产生的粉尘危害", ExamFrequency = "2年" },
            new() { Code = "GBZ188-F006", Category = "粉尘", Name = "云母粉尘", Description = "云母开采和加工过程中的粉尘", ExamFrequency = "2年" },
            new() { Code = "GBZ188-F007", Category = "粉尘", Name = "陶土粉尘", Description = "陶瓷制造过程中的粉尘暴露", ExamFrequency = "2年" },
            new() { Code = "GBZ188-F008", Category = "粉尘", Name = "铝尘", Description = "铝冶炼和加工过程中的粉尘", ExamFrequency = "1年" },
            new() { Code = "GBZ188-F009", Category = "粉尘", Name = "电焊烟尘", Description = "电焊作业产生的金属烟尘", ExamFrequency = "1年" },
            new() { Code = "GBZ188-F010", Category = "粉尘", Name = "铸造粉尘", Description = "铸造生产过程中的粉尘", ExamFrequency = "1年" },
            // 化学类 16
            new() { Code = "GBZ188-C001", Category = "化学", Name = "铅及其无机化合物", Description = "铅中毒引起的职业病", ExamFrequency = "1年" },
            new() { Code = "GBZ188-C002", Category = "化学", Name = "汞及其无机化合物", Description = "汞中毒引起的职业危害", ExamFrequency = "1年" },
            new() { Code = "GBZ188-C003", Category = "化学", Name = "锰及其无机化合物", Description = "锰中毒引起神经系统损害", ExamFrequency = "1年" },
            new() { Code = "GBZ188-C004", Category = "化学", Name = "苯（甲苯、二甲苯）", Description = "有机溶剂引起的职业中毒", ExamFrequency = "1年" },
            new() { Code = "GBZ188-C005", Category = "化学", Name = "正己烷", Description = "有机溶剂引起的周围神经损伤", ExamFrequency = "1年" },
            new() { Code = "GBZ188-C006", Category = "化学", Name = "三氯乙烯", Description = "有机溶剂引起的职业病", ExamFrequency = "1年" },
            new() { Code = "GBZ188-C007", Category = "化学", Name = "氯乙烯", Description = "塑料工业中的职业病危害", ExamFrequency = "1年" },
            new() { Code = "GBZ188-C008", Category = "化学", Name = "一氧化碳", Description = "有毒气体中毒", ExamFrequency = "1年" },
            new() { Code = "GBZ188-C009", Category = "化学", Name = "硫化氢", Description = "有毒气体中毒", ExamFrequency = "1年" },
            new() { Code = "GBZ188-C010", Category = "化学", Name = "氨", Description = "刺激性气体引起呼吸道损伤", ExamFrequency = "1年" },
            new() { Code = "GBZ188-C011", Category = "化学", Name = "硫酸", Description = "腐蚀性化学品", ExamFrequency = "1年" },
            new() { Code = "GBZ188-C012", Category = "化学", Name = "盐酸", Description = "腐蚀性化学品", ExamFrequency = "1年" },
            new() { Code = "GBZ188-C013", Category = "化学", Name = "铬及其化合物", Description = "铬引起皮肤和呼吸道损害", ExamFrequency = "1年" },
            new() { Code = "GBZ188-C014", Category = "化学", Name = "镉及其化合物", Description = "镉中毒引起肾损伤", ExamFrequency = "1年" },
            new() { Code = "GBZ188-C015", Category = "化学", Name = "有机磷农药", Description = "农药中毒", ExamFrequency = "1年" },
            new() { Code = "GBZ188-C016", Category = "化学", Name = "丙烯酰胺", Description = "化学中毒", ExamFrequency = "1年" },
            // 物理类 10
            new() { Code = "GBZ188-P001", Category = "物理", Name = "噪声", Description = "噪声引起听力损伤", ExamFrequency = "1年" },
            new() { Code = "GBZ188-P002", Category = "物理", Name = "高温", Description = "高温作业引起的职业病", ExamFrequency = "1年" },
            new() { Code = "GBZ188-P003", Category = "物理", Name = "振动（局部）", Description = "手臂振动引起白指病", ExamFrequency = "1年" },
            new() { Code = "GBZ188-P004", Category = "物理", Name = "紫外辐射", Description = "电焊弧光等引起眼部和皮肤损伤", ExamFrequency = "1年" },
            new() { Code = "GBZ188-P005", Category = "物理", Name = "微波辐射", Description = "微波辐射引起健康影响", ExamFrequency = "2年" },
            new() { Code = "GBZ188-P006", Category = "物理", Name = "电离辐射", Description = "放射性作业引起的辐射损伤", ExamFrequency = "1年" },
            new() { Code = "GBZ188-P007", Category = "物理", Name = "高气压", Description = "潜水等高压作业", ExamFrequency = "1年" },
            new() { Code = "GBZ188-P008", Category = "物理", Name = "低气压", Description = "高空等低压作业", ExamFrequency = "1年" },
            new() { Code = "GBZ188-P009", Category = "物理", Name = "激光辐射", Description = "激光作业引起的眼损伤", ExamFrequency = "1年" },
            new() { Code = "GBZ188-P010", Category = "物理", Name = "工频电磁场", Description = "电力行业电磁场暴露", ExamFrequency = "2年" },
            // 生物类 5
            new() { Code = "GBZ188-B001", Category = "生物", Name = "布鲁氏杆菌", Description = "畜牧业接触引起的感染", ExamFrequency = "1年" },
            new() { Code = "GBZ188-B002", Category = "生物", Name = "炭疽杆菌", Description = "皮毛加工等引起的感染", ExamFrequency = "1年" },
            new() { Code = "GBZ188-B003", Category = "生物", Name = "森林脑炎病毒", Description = "林业作业引起的感染", ExamFrequency = "1年" },
            new() { Code = "GBZ188-B004", Category = "生物", Name = "艾滋病病毒（职业暴露）", Description = "医护人员职业暴露", ExamFrequency = "1年" },
            new() { Code = "GBZ188-B005", Category = "生物", Name = "肝炎病毒（职业暴露）", Description = "医护人员职业暴露", ExamFrequency = "1年" },
        };
        // Radioactive
        hazards.Add(new() { Code = "GBZ188-R001", Category = "放射性", Name = "铀", Description = "铀矿开采和加工", ExamFrequency = "1年" });
        hazards.Add(new() { Code = "GBZ188-R002", Category = "放射性", Name = "钚", Description = "核工业放射性物质", ExamFrequency = "1年" });
        hazards.Add(new() { Code = "GBZ188-R003", Category = "放射性", Name = "氡", Description = "矿山等场所氡暴露", ExamFrequency = "1年" });
        hazards.Add(new() { Code = "GBZ188-R004", Category = "放射性", Name = "钴-60", Description = "工业辐照和医疗", ExamFrequency = "1年" });
        hazards.Add(new() { Code = "GBZ188-R005", Category = "放射性", Name = "铯-137", Description = "工业检测用放射源", ExamFrequency = "1年" });

        db.HazardFactors.AddRange(hazards);
        db.SaveChanges();

        // === Occupations (8) ===
        var occupations = new[]
        {
            new Occupation { Code = "OCC001", Name = "焊工", Description = "从事电焊、气焊等焊接作业" },
            new Occupation { Code = "OCC002", Name = "油漆工", Description = "从事喷漆、涂装作业" },
            new Occupation { Code = "OCC003", Name = "打磨工", Description = "从事金属表面打磨作业" },
            new Occupation { Code = "OCC004", Name = "电工", Description = "从事电气设备安装维修" },
            new Occupation { Code = "OCC005", Name = "钳工", Description = "从事机械装配和修理" },
            new Occupation { Code = "OCC006", Name = "铸造工", Description = "从事金属铸造生产" },
            new Occupation { Code = "OCC007", Name = "化验员", Description = "从事化学分析和检验" },
            new Occupation { Code = "OCC008", Name = "锅炉工", Description = "从事锅炉运行和维护" },
        };
        db.Occupations.AddRange(occupations);
        db.SaveChanges();

        // === Occupation-Hazard bindings ===
        // 使用 Code 查找确保不受插入顺序影响
        var hf = db.HazardFactors.ToDictionary(h => h.Code);
        var occ = db.Occupations.ToDictionary(o => o.Code);

        var ohBindings = new List<OccupationHazard>
        {
            // 焊工 -> 电焊烟尘 + 紫外辐射 + 噪声 + 锰
            new() { OccupationId = occ["OCC001"].Id, HazardFactorId = hf["GBZ188-F009"].Id }, // 电焊烟尘
            new() { OccupationId = occ["OCC001"].Id, HazardFactorId = hf["GBZ188-P004"].Id }, // 紫外辐射
            new() { OccupationId = occ["OCC001"].Id, HazardFactorId = hf["GBZ188-P001"].Id }, // 噪声
            new() { OccupationId = occ["OCC001"].Id, HazardFactorId = hf["GBZ188-C003"].Id }, // 锰
            // 油漆工 -> 苯 + 正己烷
            new() { OccupationId = occ["OCC002"].Id, HazardFactorId = hf["GBZ188-C004"].Id }, // 苯
            new() { OccupationId = occ["OCC002"].Id, HazardFactorId = hf["GBZ188-C005"].Id }, // 正己烷
            // 打磨工 -> 矽尘 + 噪声
            new() { OccupationId = occ["OCC003"].Id, HazardFactorId = hf["GBZ188-F001"].Id }, // 矽尘
            new() { OccupationId = occ["OCC003"].Id, HazardFactorId = hf["GBZ188-P001"].Id }, // 噪声
            // 铸造工 -> 铸造粉尘 + 高温 + 噪声
            new() { OccupationId = occ["OCC006"].Id, HazardFactorId = hf["GBZ188-F010"].Id }, // 铸造粉尘
            new() { OccupationId = occ["OCC006"].Id, HazardFactorId = hf["GBZ188-P002"].Id }, // 高温
            new() { OccupationId = occ["OCC006"].Id, HazardFactorId = hf["GBZ188-P001"].Id }, // 噪声
            // 化验员 -> 苯 + 锰
            new() { OccupationId = occ["OCC007"].Id, HazardFactorId = hf["GBZ188-C004"].Id }, // 苯
            new() { OccupationId = occ["OCC007"].Id, HazardFactorId = hf["GBZ188-C003"].Id }, // 锰
            // 锅炉工 -> 高温 + 噪声
            new() { OccupationId = occ["OCC008"].Id, HazardFactorId = hf["GBZ188-P002"].Id }, // 高温
            new() { OccupationId = occ["OCC008"].Id, HazardFactorId = hf["GBZ188-P001"].Id }, // 噪声
        };
        db.OccupationHazards.AddRange(ohBindings);
        db.SaveChanges();

        // === Positions: 一车间 -> [焊接岗, 喷漆岗, 打磨岗], 二车间 ===
        var w1 = new Position { FactoryId = 2, Name = "一车间", Level = "workshop", OrderIndex = 0 };
        var w2 = new Position { FactoryId = 2, Name = "二车间", Level = "workshop", OrderIndex = 1 };
        db.Positions.AddRange(w1, w2);
        db.SaveChanges();

        var p1 = new Position { FactoryId = 2, ParentId = w1.Id, Name = "焊接岗", Level = "position", OccupationId = 1, OrderIndex = 0 };
        var p2 = new Position { FactoryId = 2, ParentId = w1.Id, Name = "喷漆岗", Level = "position", OccupationId = 2, OrderIndex = 1 };
        var p3 = new Position { FactoryId = 2, ParentId = w1.Id, Name = "打磨岗", Level = "position", OccupationId = 3, OrderIndex = 2 };
        db.Positions.AddRange(p1, p2, p3);
        db.SaveChanges();

        // Position hazards (使用 Code 查找，确保与职业绑定一致)
        var phazards = new List<PositionHazard>
        {
            // 焊接岗继承自焊工
            new() { PositionId = p1.Id, HazardFactorId = hf["GBZ188-F009"].Id, Source = "occupation" },
            new() { PositionId = p1.Id, HazardFactorId = hf["GBZ188-P004"].Id, Source = "occupation" },
            new() { PositionId = p1.Id, HazardFactorId = hf["GBZ188-P001"].Id, Source = "occupation" },
            new() { PositionId = p1.Id, HazardFactorId = hf["GBZ188-C003"].Id, Source = "occupation" },
            // 喷漆岗继承自油漆工
            new() { PositionId = p2.Id, HazardFactorId = hf["GBZ188-C004"].Id, Source = "occupation" },
            new() { PositionId = p2.Id, HazardFactorId = hf["GBZ188-C005"].Id, Source = "occupation" },
            // 打磨岗继承自打磨工
            new() { PositionId = p3.Id, HazardFactorId = hf["GBZ188-F001"].Id, Source = "occupation" },
            new() { PositionId = p3.Id, HazardFactorId = hf["GBZ188-P001"].Id, Source = "occupation" },
        };
        db.PositionHazards.AddRange(phazards);
        db.SaveChanges();

        // === Factory Contacts ===
        db.FactoryContacts.AddRange(
            new FactoryContact { FactoryId = 2, Name = "张三", Position = "安全主管", Phone = "13900139001" },
            new FactoryContact { FactoryId = 2, Name = "李四", Position = "人事经理", Phone = "13900139002" }
        );
        db.SaveChanges();

        // === Employees ===
        var employees = new[]
        {
            new Employee { FactoryId = 2, Name = "王五", Gender = "male", BirthDate = new DateTime(1990,1,15), IdCard = "110101199001151234", Phone = "13700137001", PositionId = p1.Id, EntryDate = new DateTime(2018,3,1), HazardStartDate = new DateTime(2018,3,1) },
            new Employee { FactoryId = 2, Name = "赵六", Gender = "male", BirthDate = new DateTime(1985,6,20), IdCard = "110101198506201235", Phone = "13700137002", PositionId = p2.Id, EntryDate = new DateTime(2016,7,1), HazardStartDate = new DateTime(2016,7,1) },
            new Employee { FactoryId = 2, Name = "孙七", Gender = "female", BirthDate = new DateTime(1992,9,10), IdCard = "110101199209101236", Phone = "13700137003", PositionId = p3.Id, EntryDate = new DateTime(2020,1,15), HazardStartDate = new DateTime(2020,1,15) },
        };
        db.Employees.AddRange(employees);
        db.SaveChanges();

        // === Exam Packages ===
        db.ExamPackages.AddRange(
            new ExamPackage { HealthAgentId = 4, Name = "职业健康基础套餐", Description = "包含常规检查项目", Price = 280.00m, ExamItems = "体格检查,血常规,尿常规,肝功能,肾功能,心电图" },
            new ExamPackage { HealthAgentId = 4, Name = "职业健康高级套餐", Description = "包含基础和专项检查", Price = 580.00m, ExamItems = "体格检查,血常规,尿常规,肝功能,肾功能,心电图,肺功能,听力测试,胸部X光" }
        );
        db.SaveChanges();

        // === Exam Task (sample) ===
        var task = new ExamTask
        {
            FactoryId = 2, HealthAgentId = 4, FactoryContactId = 1,
            ExamType = "periodic", Status = "pushed", PushedAt = DateTime.UtcNow
        };
        db.ExamTasks.Add(task);
        db.SaveChanges();

        db.ExamTaskEmployees.AddRange(
            new ExamTaskEmployee { ExamTaskId = task.Id, EmployeeId = 1 },
            new ExamTaskEmployee { ExamTaskId = task.Id, EmployeeId = 2 }
        );
        db.SaveChanges();

        Console.WriteLine($"Seed complete: {users.Length} users, {hazards.Count} hazards, {occupations.Length} occupations, 2 workshops, 3 positions, 2 contacts, 3 employees, 1 task, 2 packages");
    }
}
