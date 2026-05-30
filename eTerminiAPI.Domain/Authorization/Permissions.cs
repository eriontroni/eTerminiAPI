namespace eTerminiAPI.Domain.Authorization;

/// <summary>
/// Katalogu qendror i lejeve për Admin Panel. Kodet ndahen mes backend-it
/// (autorizim + JWT claims) dhe frontend-it (gating i menusë/rrugëve).
/// </summary>
public static class Permissions
{
    public static class Dashboard
    {
        public const string View = "dashboard.view";
    }

    public static class Tenants
    {
        public const string View         = "tenants.view";
        public const string CreateUpdate = "tenants.create_update";
        public const string Delete       = "tenants.delete";
    }

    public static class Institutions
    {
        public const string View         = "institutions.view";
        public const string CreateUpdate = "institutions.create_update";
        public const string Delete       = "institutions.delete";
    }

    public static class Workers
    {
        public const string View         = "workers.view";
        public const string CreateUpdate = "workers.create_update";
        public const string Delete       = "workers.delete";
    }

    public static class System
    {
        public const string View = "system.view";
    }

    public static class Departments
    {
        public const string View         = "departments.view";
        public const string CreateUpdate = "departments.create_update";
        public const string Delete       = "departments.delete";
    }

    public static class Administrators
    {
        public const string View         = "administrators.view";
        public const string CreateUpdate = "administrators.create_update";
        public const string Delete       = "administrators.delete";
    }

    /// <summary>Përkufizimi i një veprimi brenda një moduli.</summary>
    public sealed record PermissionAction(string Code, string Label);

    /// <summary>Përkufizimi i një moduli me veprimet e tij.</summary>
    public sealed record PermissionModule(string Key, string Label, IReadOnlyList<PermissionAction> Actions);

    /// <summary>Katalogu i plotë i moduleve/veprimeve për të renderuar matricën e lejeve.</summary>
    public static readonly IReadOnlyList<PermissionModule> Catalog = new List<PermissionModule>
    {
        new("dashboard", "Dashboard", new List<PermissionAction>
        {
            new(Dashboard.View, "Shiko"),
        }),
        new("tenants", "Qytetet", new List<PermissionAction>
        {
            new(Tenants.View,         "Shiko qytetet"),
            new(Tenants.CreateUpdate, "Krijo/Përditëso"),
            new(Tenants.Delete,       "Fshi"),
        }),
        new("institutions", "Institucionet", new List<PermissionAction>
        {
            new(Institutions.View,         "Shiko institucionet"),
            new(Institutions.CreateUpdate, "Krijo/Përditëso"),
            new(Institutions.Delete,       "Fshi"),
        }),
        new("workers", "Punëtorët", new List<PermissionAction>
        {
            new(Workers.View,         "Shiko punëtorët"),
            new(Workers.CreateUpdate, "Krijo/Përditëso"),
            new(Workers.Delete,       "Fshi"),
        }),
        new("system", "Sistemi", new List<PermissionAction>
        {
            new(System.View, "Shiko"),
        }),
        new("departments", "Departamentet", new List<PermissionAction>
        {
            new(Departments.View,         "Shiko departamentet"),
            new(Departments.CreateUpdate, "Krijo/Përditëso"),
            new(Departments.Delete,       "Fshi"),
        }),
        new("administrators", "Shto Administrator", new List<PermissionAction>
        {
            new(Administrators.View,         "Shiko rolet & administratorët"),
            new(Administrators.CreateUpdate, "Krijo/Përditëso"),
            new(Administrators.Delete,       "Fshi"),
        }),
    };

    /// <summary>Të gjitha kodet e lejeve (përdoret për SuperAdmin → qasje e plotë).</summary>
    public static readonly IReadOnlyList<string> All =
        Catalog.SelectMany(m => m.Actions.Select(a => a.Code)).ToList();

    /// <summary>Verifikon nëse një kod leje është valid.</summary>
    public static bool IsValid(string code) => All.Contains(code);
}
