using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PermitSystem.Web.Entities;

namespace PermitSystem.Web.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, UserManager<ApplicationUser> um, RoleManager<IdentityRole> rm)
    {
        // ═══ Roles ═══
        foreach (var r in new[] { "Admin", "Approver", "Requester", "Guard" })
        {
            if (!await rm.RoleExistsAsync(r))
                await rm.CreateAsync(new IdentityRole(r));
        }

        // ═══ Themes ═══
        if (!await db.Themes.AnyAsync())
        {
            db.Themes.AddRange(
                new Theme { Key = "default", Name = "الأخضر التقليدي", IsDefault = true },
                new Theme { Key = "royal", Name = "الأزرق الملكي", ColorDark = "#0c1e3d", ColorPrimary = "#1a3a6c", ColorPrimaryLight = "#2a5aa0", ColorPale = "#eef4fc", ColorLight = "#d6e4f7", ColorGold = "#b8860b", ColorGoldLight = "#d4a017", ColorGoldBg = "#fef8ec" },
                new Theme { Key = "burgundy", Name = "العنابي", ColorDark = "#3d0c1e", ColorPrimary = "#6c1a3a", ColorPrimaryLight = "#a02a5a", ColorPale = "#fceef4", ColorLight = "#f7d6e4" },
                new Theme { Key = "navy", Name = "الكحلي", ColorDark = "#0f172a", ColorPrimary = "#1e293b", ColorPrimaryLight = "#334155", ColorPale = "#f1f5f9", ColorLight = "#e2e8f0" },
                new Theme { Key = "emerald", Name = "الزمردي", ColorDark = "#064e3b", ColorPrimary = "#065f46", ColorPrimaryLight = "#059669", ColorPale = "#ecfdf5", ColorLight = "#d1fae5" },
                new Theme { Key = "purple", Name = "البنفسجي", ColorDark = "#2e1065", ColorPrimary = "#4c1d95", ColorPrimaryLight = "#7c3aed", ColorPale = "#f5f3ff", ColorLight = "#ede9fe" }
            );
            await db.SaveChangesAsync();
        }

        // ═══ System Settings ═══
        if (!await db.SystemSettings.AnyAsync())
        {
            db.SystemSettings.AddRange(
                new SystemSetting { Key = "System.Name", Value = "نظام تصاريح الدخول", DataType = "string", Group = "General" },
                new SystemSetting { Key = "System.Version", Value = "1.0", DataType = "string", Group = "General" },
                new SystemSetting { Key = "System.CurrentTheme", Value = "default", DataType = "string", Group = "Theme" },
                new SystemSetting { Key = "WorkingHours.Default.Days", Value = "sun,mon,tue,wed,thu", DataType = "csv", Group = "WorkingHours" },
                new SystemSetting { Key = "WorkingHours.Default.From", Value = "07:00", DataType = "time", Group = "WorkingHours" },
                new SystemSetting { Key = "WorkingHours.Default.To", Value = "17:00", DataType = "time", Group = "WorkingHours" },
                new SystemSetting { Key = "Permit.NumberPrefix.Single", Value = "TSR-", DataType = "string", Group = "Permit" },
                new SystemSetting { Key = "Permit.NumberPrefix.Vehicle", Value = "TSR-V-", DataType = "string", Group = "Permit" },
                new SystemSetting { Key = "Permit.NumberPrefix.Exit", Value = "TSR-X-", DataType = "string", Group = "Permit" },
                new SystemSetting { Key = "Query.MinSearchLength", Value = "4", DataType = "int", Group = "Gate" }
            );
            await db.SaveChangesAsync();
        }

        // ═══ Screens ═══
        if (!await db.Screens.AnyAsync())
        {
            var screens = new[]
            {
                new Screen { Key = "dashboard", Title = "لوحة التحكم", Icon = "🏠", GroupName = "الرئيسية", Category = "main", SortOrder = 1 },
                new Screen { Key = "new-request", Title = "طلب تصريح", Icon = "➕", GroupName = "الأشخاص والتصاريح", Category = "main", SortOrder = 2 },
                new Screen { Key = "vehicle", Title = "تصريح مركبة", Icon = "🚗", GroupName = "الأشخاص والتصاريح", Category = "main", SortOrder = 3 },
                new Screen { Key = "exit-permit", Title = "تصريح خروج", Icon = "🚪", GroupName = "الأشخاص والتصاريح", Category = "main", SortOrder = 4 },
                new Screen { Key = "my-requests", Title = "طلباتي", Icon = "📋", GroupName = "الأشخاص والتصاريح", Category = "main", SortOrder = 5 },
                new Screen { Key = "registry", Title = "سجل الأشخاص", Icon = "🧑", GroupName = "الأشخاص والتصاريح", Category = "main", SortOrder = 6 },
                new Screen { Key = "permitslog", Title = "سجل التصاريح", Icon = "📋", GroupName = "الأشخاص والتصاريح", Category = "main", SortOrder = 7 },
                new Screen { Key = "approvals", Title = "طلبات التصريح", Icon = "📨", GroupName = "الأشخاص والتصاريح", Category = "main", SortOrder = 8 },
                new Screen { Key = "gate", Title = "شاشة الاستعلام", Icon = "🛡️", GroupName = "نقاط الدخول", Category = "main", SortOrder = 9 },
                new Screen { Key = "querylog", Title = "سجل الاستعلامات", Icon = "🧾", GroupName = "نقاط الدخول", Category = "main", SortOrder = 10 },
                new Screen { Key = "cardprint", Title = "طباعة البطاقات", Icon = "🖨️", GroupName = "نقاط الدخول", Category = "main", SortOrder = 11 },
                new Screen { Key = "blacklist", Title = "القائمة السوداء", Icon = "⛔", GroupName = "نقاط الدخول", Category = "main", SortOrder = 12 },
                new Screen { Key = "branches", Title = "الفروع", Icon = "🏢", GroupName = "الإعدادات", Category = "settings", SortOrder = 13 },
                new Screen { Key = "buildings", Title = "المباني", Icon = "🏬", GroupName = "الإعدادات", Category = "settings", SortOrder = 14 },
                new Screen { Key = "paths", Title = "البوابات والمسارات", Icon = "🛣️", GroupName = "الإعدادات", Category = "settings", SortOrder = 15 },
                new Screen { Key = "departments", Title = "الإدارات", Icon = "🏛️", GroupName = "الإعدادات", Category = "settings", SortOrder = 16 },
                new Screen { Key = "lookups", Title = "قوائم الاختيار", Icon = "📚", GroupName = "الإعدادات", Category = "settings", SortOrder = 17 },
                new Screen { Key = "users", Title = "المستخدمون", Icon = "👤", GroupName = "الإعدادات", Category = "settings", SortOrder = 18 },
                new Screen { Key = "reports", Title = "التقارير الشاملة", Icon = "📊", GroupName = "التقارير", Category = "reports", SortOrder = 19 },
                new Screen { Key = "auditlog", Title = "سجل النظام", Icon = "📜", GroupName = "التقارير", Category = "reports", SortOrder = 20 }
            };
            db.Screens.AddRange(screens);
            await db.SaveChangesAsync();

            var all = await db.Screens.ToListAsync();
            var roleScreens = new Dictionary<string, string[]>
            {
                ["Admin"] = all.Select(s => s.Key).ToArray(),
                ["Approver"] = new[] { "dashboard", "new-request", "vehicle", "exit-permit", "registry", "my-requests", "permitslog", "approvals", "auditlog", "reports" },
                ["Requester"] = new[] { "dashboard", "new-request", "vehicle", "exit-permit", "registry", "my-requests", "permitslog" },
                ["Guard"] = new[] { "gate", "querylog", "cardprint" }
            };

            foreach (var (roleName, keys) in roleScreens)
            {
                var role = await rm.FindByNameAsync(roleName);
                if (role == null) continue;
                foreach (var key in keys)
                {
                    var s = all.First(x => x.Key == key);
                    db.RoleScreenPermissions.Add(new RoleScreenPermission { RoleId = role.Id, ScreenId = s.Id });
                }
            }
            await db.SaveChangesAsync();
        }

        // ═══ Lookups ═══
        if (!await db.LookupCategories.AnyAsync())
        {
            await SeedLookupsAsync(db);
        }

        // ═══ Admin User ═══
        if (!await db.Users.AnyAsync())
        {
            var admin = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@system.local",
                FullName = "مدير النظام",
                RoleType = "admin",
                EmailConfirmed = true,
                IsActive = true,
                Avatar = "مد"
            };
            await um.CreateAsync(admin, "Admin@123");
            await um.AddToRoleAsync(admin, "Admin");
        }
    }

    private static async Task SeedLookupsAsync(AppDbContext db)
    {
        var defs = new (string Key, string Title, string Icon, bool Grouped, string[] Items)[]
        {
            ("nationalities", "🌍 الجنسيات", "🌍", false, new[] {
                "سعودي","إماراتي","قطري","كويتي","بحريني","عماني","يمني","مصري","سوداني","أردني",
                "لبناني","سوري","عراقي","فلسطيني","مغربي","تونسي","جزائري","ليبي","باكستاني","هندي",
                "بنغلاديشي","سريلانكي","نيبالي","فلبيني","إندونيسي","ماليزي","صيني","ياباني","كوري",
                "تايلاندي","تركي","إيراني","أمريكي","بريطاني","فرنسي","ألماني","إيطالي","إسباني","هولندي",
                "بلجيكي","سويسري","سويدي","نرويجي","دنماركي","فنلندي","نمساوي","إيرلندي","برتغالي",
                "أسترالي","كندي","روسي","أوكراني","بولندي","يوناني","أخرى"
            }),
            ("personTypes", "👤 أنواع الأشخاص", "👤", false, new[] { "مواطن","مقيم","زائر","دبلوماسي" }),
            ("idTypes", "🆔 أنواع الهويات", "🆔", false, new[] { "هوية وطنية","إقامة","جواز سفر","هوية خليجية" }),
            ("visitTypes", "📅 أنواع الزيارات", "📅", false, new[] { "زيارة عمل","صيانة","توريد","اجتماع","تدريب","زيارة رسمية","أخرى" }),
            ("devices", "📱 الأجهزة", "📱", false, new[] {
                "📱 جوال","💻 كمبيوتر محمول","🖥️ جهاز مكتبي","🖨️ طابعة","🎥 كاميرا",
                "📷 كاميرا احترافية","🔬 معدات فنية","🧰 حقيبة عمل","🔌 أجهزة كهربائية",
                "📁 ملفات","📦 طرود","🔧 قطع غيار"
            }),
            ("vehicleTypes", "🚗 تصنيفات المركبات", "🚗", false, new[] { "سيدان","دفع رباعي","شاحنة نقل","دراجة نارية","حافلة","معدات ثقيلة" }),
            ("vehicleColors", "🎨 الألوان", "🎨", false, new[] { "أبيض","أسود","فضي","رمادي","أحمر","أزرق","أخضر","أصفر","بني","بيج","برتقالي","ذهبي","كحلي","وردي","بنفسجي" }),
            ("gateLocations", "📍 مواقع البوابات", "📍", false, new[] { "المدخل الرئيسي","مدخل الخدمات","مدخل الموظفين","مدخل الطوارئ","مدخل الزوار" }),
            ("exitItemCategories", "📦 تصنيفات الخروج", "📦", false, new[] { "💻 إلكترونيات","📱 هواتف","🖨️ طابعات","🪑 أثاث","📄 مستندات","📦 أخرى" }),
            ("buildingTypes", "🏬 أنواع المباني", "🏬", false, new[] { "مبنى إداري","مبنى تشغيلي","مبنى تقني","مبنى خدمات","مستودع","مبنى أمني" })
        };

        foreach (var def in defs)
        {
            var cat = new LookupCategory { Key = def.Key, Title = def.Title, Icon = def.Icon, IsSystem = true };
            db.LookupCategories.Add(cat);
            await db.SaveChangesAsync();

            var sort = 0;
            foreach (var raw in def.Items)
            {
                db.LookupItems.Add(new LookupItem
                {
                    CategoryId = cat.Id,
                    Value = raw,
                    SortOrder = sort++,
                    IsActive = true
                });
            }
            await db.SaveChangesAsync();
        }
    }
}