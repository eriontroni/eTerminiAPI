using eTerminiAPI.Domain.Entities;
using eTerminiAPI.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace eTerminiAPI.Infrastructure.Persistence;

public static class DbSeeder
{
    private static readonly Guid TenantId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public static async Task SeedAsync(AppDbContext db)
    {
        await SeedTenantsAsync(db);
        await SeedCategoriesAsync(db);
        await SeedCatalogAsync(db);
    }

    // ── Tenants ──────────────────────────────────────────────────────────────

    private static async Task SeedTenantsAsync(AppDbContext db)
    {
        if (await db.Tenants.AnyAsync()) return;

        db.Tenants.AddRange(
            new Tenant { Id = TenantId, Name = "Komuna e Prishtinës",  Slug = "prishtina", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Tenant { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Name = "Komuna e Prizrenit", Slug = "prizreni",  IsActive = true, CreatedAt = DateTime.UtcNow },
            new Tenant { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Name = "Komuna e Gjilanit",  Slug = "gjilani",   IsActive = true, CreatedAt = DateTime.UtcNow }
        );
        await db.SaveChangesAsync();
    }

    // ── Categories ───────────────────────────────────────────────────────────

    private static readonly Guid CatHealth     = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001");
    private static readonly Guid CatCivilDocs  = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000002");
    private static readonly Guid CatPolice     = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000003");
    private static readonly Guid CatTraffic    = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000004");
    private static readonly Guid CatMunicipal  = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000005");

    private static async Task SeedCategoriesAsync(AppDbContext db)
    {
        if (await db.ServiceCategories.AnyAsync()) return;

        var now = DateTime.UtcNow;
        db.ServiceCategories.AddRange(
            new ServiceCategory { Id = CatHealth,    TenantId = TenantId, Name = "Shëndetësi",                Description = "Konsulta dhe shërbime mjekësore",                              CreatedAt = now },
            new ServiceCategory { Id = CatCivilDocs, TenantId = TenantId, Name = "Dokumente Civile",          Description = "Pasaporta, letërnjoftim, certifikata",                          CreatedAt = now },
            new ServiceCategory { Id = CatPolice,    TenantId = TenantId, Name = "Policia",                   Description = "Raportime, kërkesa dhe shërbime policore",                      CreatedAt = now },
            new ServiceCategory { Id = CatTraffic,   TenantId = TenantId, Name = "Trafiku Rrugor",            Description = "Patentë shoferi, regjistrim automjeti, pagimi i gjobave",       CreatedAt = now },
            new ServiceCategory { Id = CatMunicipal, TenantId = TenantId, Name = "Shërbime Komunale",         Description = "Tatime, leje ndërtimi, dokumente komunale",                     CreatedAt = now }
        );
        await db.SaveChangesAsync();
    }

    // ── Institutions, departments, services, staff ───────────────────────────

    private static async Task SeedCatalogAsync(AppDbContext db)
    {
        if (await db.Institutions.AnyAsync()) return;

        var pack = new List<InstitutionPack>
        {
            // 1) Shëndetësi
            new(
                Id: Guid.Parse("bbbbbbbb-0001-0000-0000-000000000001"),
                CategoryId: CatHealth,
                Name: "QKMF Prishtinë",
                Description: "Qendra Kryesore e Mjekësisë Familjare",
                City: "Prishtinë",
                Address: "Rr. Bedri Pejani, Prishtinë",
                Phone: "+383 38 230 100",
                Departments: new()
                {
                    new("cccccccc-0001-0001-0000-000000000001", "Mjekësi Familjare", new()
                    {
                        new("dddddddd-0001-0001-0001-000000000001", "Konsultë e përgjithshme",    "Vizitë mjekësore standarde",      30),
                        new("dddddddd-0001-0001-0002-000000000001", "Vizitë pediatrike",          "Kontroll për fëmijë deri 14 vjeç", 30),
                    }),
                    new("cccccccc-0001-0002-0000-000000000001", "Specialistike", new()
                    {
                        new("dddddddd-0001-0002-0001-000000000001", "Kardiologji",                "Kontrolli i zemrës dhe enëve të gjakut", 45),
                        new("dddddddd-0001-0002-0002-000000000001", "Neurologji",                 "Diagnostikim i sistemit nervor",          45),
                        new("dddddddd-0001-0002-0003-000000000001", "Oftalmologji",               "Kontroll i syrit",                        30),
                    }),
                },
                Staff: new()
                {
                    new("ffffffff-0001-0001-0001-000000000001", "Arben",  "Krasniqi", "Mjek i përgjithshëm", "cccccccc-0001-0001-0000-000000000001"),
                    new("ffffffff-0001-0001-0002-000000000001", "Elira",  "Gashi",    "Pediatre",            "cccccccc-0001-0001-0000-000000000001"),
                    new("ffffffff-0001-0002-0001-000000000001", "Albana", "Berisha",  "Kardiologe",          "cccccccc-0001-0002-0000-000000000001"),
                    new("ffffffff-0001-0002-0002-000000000001", "Fatos",  "Hoxha",    "Neurolog",            "cccccccc-0001-0002-0000-000000000001"),
                    new("ffffffff-0001-0002-0003-000000000001", "Lirie",  "Shala",    "Oftalmologe",         "cccccccc-0001-0002-0000-000000000001"),
                }
            ),

            // 2) Dokumente Civile
            new(
                Id: Guid.Parse("bbbbbbbb-0002-0000-0000-000000000001"),
                CategoryId: CatCivilDocs,
                Name: "Drejtoria e Regjistrit Civil – Prishtinë",
                Description: "Lëshim dokumentesh personale dhe certifikata",
                City: "Prishtinë",
                Address: "Sheshi Nëna Terezë, Prishtinë",
                Phone: "+383 38 244 100",
                Departments: new()
                {
                    new("cccccccc-0002-0001-0000-000000000001", "Dokumente Personale", new()
                    {
                        new("dddddddd-0002-0001-0001-000000000001", "Aplikim për pasaportë",       "Lëshim i pasaportës biometrike",        30),
                        new("dddddddd-0002-0001-0002-000000000001", "Letërnjoftim (ID)",            "Lëshim ose ripërtëritje e ID-së",       30),
                        new("dddddddd-0002-0001-0003-000000000001", "Certifikatë lindjeje",         "Lëshim i certifikatës së lindjes",      15),
                        new("dddddddd-0002-0001-0004-000000000001", "Certifikatë martese",          "Lëshim i certifikatës së martesës",     15),
                        new("dddddddd-0002-0001-0005-000000000001", "Certifikatë vendbanimi",       "Vërtetim i vendbanimit",                15),
                    }),
                },
                Staff: new()
                {
                    new("ffffffff-0002-0001-0001-000000000001", "Donika",  "Maloku",   "Zyrtare e Pasaportave",  "cccccccc-0002-0001-0000-000000000001"),
                    new("ffffffff-0002-0001-0002-000000000001", "Burim",   "Selmani",  "Zyrtar i ID-ve",          "cccccccc-0002-0001-0000-000000000001"),
                    new("ffffffff-0002-0001-0003-000000000001", "Mirjeta", "Zogaj",    "Zyrtare e Certifikatave", "cccccccc-0002-0001-0000-000000000001"),
                }
            ),

            // 3) Policia
            new(
                Id: Guid.Parse("bbbbbbbb-0003-0000-0000-000000000001"),
                CategoryId: CatPolice,
                Name: "Stacioni Policor – Prishtinë",
                Description: "Shërbime dhe raportime policore",
                City: "Prishtinë",
                Address: "Rr. Luan Haradinaj, Prishtinë",
                Phone: "192",
                Departments: new()
                {
                    new("cccccccc-0003-0001-0000-000000000001", "Shërbime Qytetare", new()
                    {
                        new("dddddddd-0003-0001-0001-000000000001", "Raportim humbjeje dokumentesh", "Raportim i humbjes së dokumenteve personale", 20),
                        new("dddddddd-0003-0001-0002-000000000001", "Vërtetim i dënueshmërisë",      "Vërtetim që personi nuk është nën hetim",     20),
                        new("dddddddd-0003-0001-0003-000000000001", "Leje për tubim publik",         "Aplikim për leje tubimi",                     30),
                    }),
                },
                Staff: new()
                {
                    new("ffffffff-0003-0001-0001-000000000001", "Driton",  "Bajrami", "Inspektor i Policisë",     "cccccccc-0003-0001-0000-000000000001"),
                    new("ffffffff-0003-0001-0002-000000000001", "Vlora",   "Krasniqi","Zyrtare për Qytetarë",     "cccccccc-0003-0001-0000-000000000001"),
                }
            ),

            // 4) Trafiku Rrugor
            new(
                Id: Guid.Parse("bbbbbbbb-0004-0000-0000-000000000001"),
                CategoryId: CatTraffic,
                Name: "Drejtoria e Trafikut Rrugor – Prishtinë",
                Description: "Patentë shoferi, regjistrim automjeti, pagimi i gjobave",
                City: "Prishtinë",
                Address: "Rr. Vëllezërit Fazliu, Prishtinë",
                Phone: "+383 38 200 400",
                Departments: new()
                {
                    new("cccccccc-0004-0001-0000-000000000001", "Patentë Shoferi", new()
                    {
                        new("dddddddd-0004-0001-0001-000000000001", "Aplikim për patentë shoferi",   "Lëshim i patentës së shoferit", 30),
                        new("dddddddd-0004-0001-0002-000000000001", "Ripërtëritje e patentës",       "Zëvendësim i patentës ekzistuese", 20),
                        new("dddddddd-0004-0001-0003-000000000001", "Teste teorike",                 "Caktim termini për provim teorik", 60),
                    }),
                    new("cccccccc-0004-0002-0000-000000000001", "Automjete", new()
                    {
                        new("dddddddd-0004-0002-0001-000000000001", "Regjistrim automjeti",          "Regjistrim i parë i automjetit", 45),
                        new("dddddddd-0004-0002-0002-000000000001", "Pagimi i gjobave të trafikut",  "Pagesë e drejtpërdrejtë e gjobave", 15),
                    }),
                },
                Staff: new()
                {
                    new("ffffffff-0004-0001-0001-000000000001", "Agon",   "Hoti",    "Zyrtar Patentash",         "cccccccc-0004-0001-0000-000000000001"),
                    new("ffffffff-0004-0001-0002-000000000001", "Blerta", "Ramaj",   "Instruktore e Testeve",    "cccccccc-0004-0001-0000-000000000001"),
                    new("ffffffff-0004-0002-0001-000000000001", "Liridon","Beqiri",  "Zyrtar i Regjistrimeve",   "cccccccc-0004-0002-0000-000000000001"),
                    new("ffffffff-0004-0002-0002-000000000001", "Adelina","Krasniqi","Arkëtare e Gjobave",       "cccccccc-0004-0002-0000-000000000001"),
                }
            ),

            // 5) Shërbime Komunale
            new(
                Id: Guid.Parse("bbbbbbbb-0005-0000-0000-000000000001"),
                CategoryId: CatMunicipal,
                Name: "Komuna e Prishtinës",
                Description: "Shërbime administrative dhe komunale",
                City: "Prishtinë",
                Address: "Sheshi Adem Jashari, Prishtinë",
                Phone: "+383 38 230 200",
                Departments: new()
                {
                    new("cccccccc-0005-0001-0000-000000000001", "Tatimi në Pronë", new()
                    {
                        new("dddddddd-0005-0001-0001-000000000001", "Pagimi i tatimit në pronë",   "Pagesa e tatimit vjetor",         15),
                        new("dddddddd-0005-0001-0002-000000000001", "Vërtetim tatimor",            "Lëshim i vërtetimit për tatime",  15),
                    }),
                    new("cccccccc-0005-0002-0000-000000000001", "Leje Ndërtimi", new()
                    {
                        new("dddddddd-0005-0002-0001-000000000001", "Aplikim për leje ndërtimi",   "Aplikim i ri për leje ndërtimi",  60),
                        new("dddddddd-0005-0002-0002-000000000001", "Konsultim urbanistik",        "Takim me planifikuesit urban",    45),
                    }),
                    new("cccccccc-0005-0003-0000-000000000001", "Shërbime Administrative", new()
                    {
                        new("dddddddd-0005-0003-0001-000000000001", "Vërtetim banimi",             "Vërtetim që banoni në komunë",    15),
                        new("dddddddd-0005-0003-0002-000000000001", "Ankesë qytetare",             "Paraqitje e ankesës formale",     30),
                    }),
                },
                Staff: new()
                {
                    new("ffffffff-0005-0001-0001-000000000001", "Besarta","Mehmeti", "Arkëtare Tatimore",       "cccccccc-0005-0001-0000-000000000001"),
                    new("ffffffff-0005-0002-0001-000000000001", "Edmond", "Berisha", "Inxhinier Urbanist",      "cccccccc-0005-0002-0000-000000000001"),
                    new("ffffffff-0005-0003-0001-000000000001", "Arlinda","Zeqiri",  "Zyrtare Administrative",  "cccccccc-0005-0003-0000-000000000001"),
                }
            ),
        };

        var now = DateTime.UtcNow;

        foreach (var p in pack)
        {
            db.Institutions.Add(new Institution
            {
                Id = p.Id,
                TenantId = TenantId,
                CategoryId = p.CategoryId,
                Name = p.Name,
                Description = p.Description,
                City = p.City,
                Address = p.Address,
                PhoneNumber = p.Phone,
                IsActive = true,
                CreatedAt = now,
            });

            foreach (var dept in p.Departments)
            {
                db.Departments.Add(new Department
                {
                    Id = Guid.Parse(dept.Id),
                    TenantId = TenantId,
                    InstitutionId = p.Id,
                    Name = dept.Name,
                    IsActive = true,
                    CreatedAt = now,
                });

                foreach (var svc in dept.Services)
                {
                    db.PublicServices.Add(new PublicService
                    {
                        Id = Guid.Parse(svc.Id),
                        TenantId = TenantId,
                        InstitutionId = p.Id,
                        Name = svc.Name,
                        Description = svc.Description,
                        DurationMinutes = svc.DurationMinutes,
                        IsActive = true,
                        CreatedAt = now,
                    });
                }
            }

            foreach (var staff in p.Staff)
            {
                var userId = Guid.Parse("ee" + staff.StaffId.Substring(2));
                db.Users.Add(new User
                {
                    Id = userId,
                    TenantId = TenantId,
                    FirstName = staff.FirstName,
                    LastName = staff.LastName,
                    Email = $"{staff.FirstName}.{staff.LastName}@etermini.rks-gov.net".ToLowerInvariant(),
                    PasswordHash = "SEEDED_NO_LOGIN",
                    Role = UserRole.Staff,
                    IsActive = true,
                    CreatedAt = now,
                });

                var staffMemberId = Guid.Parse(staff.StaffId);
                db.StaffMembers.Add(new StaffMember
                {
                    Id = staffMemberId,
                    TenantId = TenantId,
                    UserId = userId,
                    DepartmentId = Guid.Parse(staff.DepartmentId),
                    Title = staff.Title,
                    IsActive = true,
                    CreatedAt = now,
                });

                // Mon-Fri 09:00-17:00 schedule
                foreach (var day in new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday })
                {
                    db.StaffSchedules.Add(new StaffSchedule
                    {
                        Id = Guid.NewGuid(),
                        TenantId = TenantId,
                        StaffMemberId = staffMemberId,
                        DayOfWeek = day,
                        StartTime = new TimeOnly(9, 0),
                        EndTime = new TimeOnly(17, 0),
                        IsActive = true,
                        CreatedAt = now,
                    });
                }
            }
        }

        await db.SaveChangesAsync();
    }

    // ── DTOs internal to the seeder ──────────────────────────────────────────

    private record InstitutionPack(
        Guid Id,
        Guid CategoryId,
        string Name,
        string Description,
        string City,
        string Address,
        string Phone,
        List<DeptPack> Departments,
        List<StaffPack> Staff);

    private record DeptPack(string Id, string Name, List<ServicePack> Services);
    private record ServicePack(string Id, string Name, string Description, int DurationMinutes);
    private record StaffPack(string StaffId, string FirstName, string LastName, string Title, string DepartmentId);
}
