-- ============================================================
-- eTermini — Test Data Script
-- Ekzekuto pas: Update-Database
-- PasswordHash = ASP.NET IPasswordHasher i "Test123!" per te gjitha llogarite
-- ============================================================

USE eTerminiDB;
GO

-- ============================================================
-- PASTRO (renditja e kundert e FK)
-- ============================================================
DELETE FROM AuditLogs;
DELETE FROM AppointmentStatusHistories;
DELETE FROM Appointments;
DELETE FROM TimeSlots;
DELETE FROM StaffSchedules;
DELETE FROM StaffMembers;
DELETE FROM ServiceRequirements;
DELETE FROM PublicServices;
DELETE FROM ServiceCategories;
DELETE FROM Departments;
DELETE FROM InstitutionBranches;
DELETE FROM Notifications;
DELETE FROM NotificationTemplates;
DELETE FROM RefreshTokens;
DELETE FROM Users;
DELETE FROM TenantSettings;
DELETE FROM Tenants;
GO

-- ============================================================
-- 1. TENANTS
-- ============================================================
INSERT INTO Tenants (Id, Name, Slug, LogoUrl, IsActive, CreatedAt, IsDeleted)
VALUES
  ('10000000-0000-0000-0000-000000000001', 'QKUK',                'qkuk',         NULL, 1, GETUTCDATE(), 0),
  ('10000000-0000-0000-0000-000000000002', 'Policia e Kosovës',   'policia',       NULL, 1, GETUTCDATE(), 0),
  ('10000000-0000-0000-0000-000000000003', 'Komuna e Prishtinës', 'komuna-prishte',NULL, 1, GETUTCDATE(), 0);
GO

-- ============================================================
-- 2. TENANT SETTINGS
-- ============================================================
INSERT INTO TenantSettings (Id, TenantId, [Key], [Value], CreatedAt, IsDeleted)
VALUES
  (NEWID(), '10000000-0000-0000-0000-000000000001', 'max_appointments_per_day', '50',    GETUTCDATE(), 0),
  (NEWID(), '10000000-0000-0000-0000-000000000001', 'slot_duration_minutes',    '30',    GETUTCDATE(), 0),
  (NEWID(), '10000000-0000-0000-0000-000000000002', 'max_appointments_per_day', '100',   GETUTCDATE(), 0),
  (NEWID(), '10000000-0000-0000-0000-000000000002', 'slot_duration_minutes',    '20',    GETUTCDATE(), 0),
  (NEWID(), '10000000-0000-0000-0000-000000000003', 'max_appointments_per_day', '80',    GETUTCDATE(), 0),
  (NEWID(), '10000000-0000-0000-0000-000000000003', 'slot_duration_minutes',    '15',    GETUTCDATE(), 0);
GO

-- ============================================================
-- 3. USERS
-- Role: 0=Citizen, 1=Staff, 2=InstitutionAdmin, 3=SuperAdmin
-- PasswordHash = IPasswordHasher("Test123!")
-- ============================================================
INSERT INTO Users (Id, TenantId, FirstName, LastName, Email, PasswordHash, PhoneNumber, [Role], IsActive, CreatedAt, IsDeleted)
VALUES
  -- SuperAdmin (Tenant=QKUK si placeholder)
  ('20000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000001',
   'Admin', 'Super', 'superadmin@etermini.com',
   'AQAAAAIAAYagAAAAEMN8wAys5RkLiu+wu11/E0BoFqJWqD+zTQhBMb5KsP86MVeg1+SqnYs6lBWtzlM5/Q==',
   '+38344100000', 3, 1, GETUTCDATE(), 0),

  -- InstitutionAdmins
  ('20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000001',
   'Besnik', 'Krasniqi', 'admin@qkuk.com',
   'AQAAAAIAAYagAAAAEMN8wAys5RkLiu+wu11/E0BoFqJWqD+zTQhBMb5KsP86MVeg1+SqnYs6lBWtzlM5/Q==',
   '+38344100001', 2, 1, GETUTCDATE(), 0),

  ('20000000-0000-0000-0000-000000000003', '10000000-0000-0000-0000-000000000002',
   'Valdrin', 'Gashi', 'admin@policia.com',
   'AQAAAAIAAYagAAAAEMN8wAys5RkLiu+wu11/E0BoFqJWqD+zTQhBMb5KsP86MVeg1+SqnYs6lBWtzlM5/Q==',
   '+38344100002', 2, 1, GETUTCDATE(), 0),

  ('20000000-0000-0000-0000-000000000004', '10000000-0000-0000-0000-000000000003',
   'Shpresa', 'Berisha', 'admin@komuna-prishte.com',
   'AQAAAAIAAYagAAAAEMN8wAys5RkLiu+wu11/E0BoFqJWqD+zTQhBMb5KsP86MVeg1+SqnYs6lBWtzlM5/Q==',
   '+38344100003', 2, 1, GETUTCDATE(), 0),

  -- Staff - QKUK
  ('20000000-0000-0000-0000-000000000005', '10000000-0000-0000-0000-000000000001',
   'Ardita', 'Hoxha', 'ardita.hoxha@qkuk.com',
   'AQAAAAIAAYagAAAAEMN8wAys5RkLiu+wu11/E0BoFqJWqD+zTQhBMb5KsP86MVeg1+SqnYs6lBWtzlM5/Q==',
   '+38344100010', 1, 1, GETUTCDATE(), 0),

  ('20000000-0000-0000-0000-000000000006', '10000000-0000-0000-0000-000000000001',
   'Driton', 'Morina', 'driton.morina@qkuk.com',
   'AQAAAAIAAYagAAAAEMN8wAys5RkLiu+wu11/E0BoFqJWqD+zTQhBMb5KsP86MVeg1+SqnYs6lBWtzlM5/Q==',
   '+38344100011', 1, 1, GETUTCDATE(), 0),

  -- Staff - Policia
  ('20000000-0000-0000-0000-000000000007', '10000000-0000-0000-0000-000000000002',
   'Fjolla', 'Rama', 'fjolla.rama@policia.com',
   'AQAAAAIAAYagAAAAEMN8wAys5RkLiu+wu11/E0BoFqJWqD+zTQhBMb5KsP86MVeg1+SqnYs6lBWtzlM5/Q==',
   '+38344100012', 1, 1, GETUTCDATE(), 0),

  -- Staff - Komuna
  ('20000000-0000-0000-0000-000000000008', '10000000-0000-0000-0000-000000000003',
   'Kujtim', 'Aliu', 'kujtim.aliu@komuna-prishte.com',
   'AQAAAAIAAYagAAAAEMN8wAys5RkLiu+wu11/E0BoFqJWqD+zTQhBMb5KsP86MVeg1+SqnYs6lBWtzlM5/Q==',
   '+38344100013', 1, 1, GETUTCDATE(), 0),

  -- Citizens
  ('20000000-0000-0000-0000-000000000010', '10000000-0000-0000-0000-000000000001',
   'Arjeta', 'Syla', 'arjeta.syla@gmail.com',
   'AQAAAAIAAYagAAAAEMN8wAys5RkLiu+wu11/E0BoFqJWqD+zTQhBMb5KsP86MVeg1+SqnYs6lBWtzlM5/Q==',
   '+38344200001', 0, 1, GETUTCDATE(), 0),

  ('20000000-0000-0000-0000-000000000011', '10000000-0000-0000-0000-000000000001',
   'Liridon', 'Hoti', 'liridon.hoti@gmail.com',
   'AQAAAAIAAYagAAAAEMN8wAys5RkLiu+wu11/E0BoFqJWqD+zTQhBMb5KsP86MVeg1+SqnYs6lBWtzlM5/Q==',
   '+38344200002', 0, 1, GETUTCDATE(), 0),

  ('20000000-0000-0000-0000-000000000012', '10000000-0000-0000-0000-000000000001',
   'Mimoza', 'Zhitia', 'mimoza.zhitia@gmail.com',
   'AQAAAAIAAYagAAAAEMN8wAys5RkLiu+wu11/E0BoFqJWqD+zTQhBMb5KsP86MVeg1+SqnYs6lBWtzlM5/Q==',
   '+38344200003', 0, 1, GETUTCDATE(), 0),

  ('20000000-0000-0000-0000-000000000013', '10000000-0000-0000-0000-000000000002',
   'Bujar', 'Sadiku', 'bujar.sadiku@gmail.com',
   'AQAAAAIAAYagAAAAEMN8wAys5RkLiu+wu11/E0BoFqJWqD+zTQhBMb5KsP86MVeg1+SqnYs6lBWtzlM5/Q==',
   '+38344200004', 0, 1, GETUTCDATE(), 0),

  ('20000000-0000-0000-0000-000000000014', '10000000-0000-0000-0000-000000000003',
   'Teuta', 'Kastrati', 'teuta.kastrati@gmail.com',
   'AQAAAAIAAYagAAAAEMN8wAys5RkLiu+wu11/E0BoFqJWqD+zTQhBMb5KsP86MVeg1+SqnYs6lBWtzlM5/Q==',
   '+38344200005', 0, 1, GETUTCDATE(), 0);
GO

-- ============================================================
-- 4. INSTITUTIONS
-- ============================================================
INSERT INTO Institutions (Id, TenantId, Name, Description, City, [Address], PhoneNumber, Email, LogoUrl, IsActive, CreatedAt, IsDeleted)
VALUES
  ('30000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000001',
   'QKUK', 'Qendra Klinike Universitare e Kosovës',
   'Prishtinë', 'Rr. Bulevardi i Dëshmorëve', '+38338500600', 'info@qkuk.com', NULL, 1, GETUTCDATE(), 0),

  ('30000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002',
   'Policia e Kosovës', 'Shërbimi Policor i Kosovës',
   'Prishtinë', 'Rr. Luan Haradinaj', '+38338511111', 'info@policia.ks', NULL, 1, GETUTCDATE(), 0),

  ('30000000-0000-0000-0000-000000000003', '10000000-0000-0000-0000-000000000003',
   'Komuna e Prishtinës', 'Administrata komunale e Prishtinës',
   'Prishtinë', 'Rr. UCK 61', '+38338230000', 'info@komuna-prishte.net', NULL, 1, GETUTCDATE(), 0);
GO

-- ============================================================
-- 5. INSTITUTION BRANCHES
-- ============================================================
INSERT INTO InstitutionBranches (Id, TenantId, InstitutionId, Name, City, [Address], PhoneNumber, IsActive, CreatedAt, IsDeleted)
VALUES
  ('40000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000001',
   '30000000-0000-0000-0000-000000000001', 'QKUK - Ndërtesa Kryesore', 'Prishtinë', 'Rr. Bulevardi i Dëshmorëve 1', '+38338500601', 1, GETUTCDATE(), 0),

  ('40000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000001',
   '30000000-0000-0000-0000-000000000001', 'QKUK - Klinika Pediatrike', 'Prishtinë', 'Rr. Bulevardi i Dëshmorëve 3', '+38338500602', 1, GETUTCDATE(), 0),

  ('40000000-0000-0000-0000-000000000003', '10000000-0000-0000-0000-000000000002',
   '30000000-0000-0000-0000-000000000002', 'Policia - Stacioni Qendror', 'Prishtinë', 'Rr. Luan Haradinaj 10', '+38338511112', 1, GETUTCDATE(), 0),

  ('40000000-0000-0000-0000-000000000004', '10000000-0000-0000-0000-000000000002',
   '30000000-0000-0000-0000-000000000002', 'Policia - Stacioni Fushë Kosovë', 'Fushë Kosovë', 'Rr. Liria 5', '+38338522222', 1, GETUTCDATE(), 0),

  ('40000000-0000-0000-0000-000000000005', '10000000-0000-0000-0000-000000000003',
   '30000000-0000-0000-0000-000000000003', 'Komuna - Qendra Administrative', 'Prishtinë', 'Rr. UCK 61', '+38338230001', 1, GETUTCDATE(), 0);
GO

-- ============================================================
-- 6. DEPARTMENTS
-- ============================================================
INSERT INTO Departments (Id, TenantId, InstitutionId, BranchId, Name, Description, IsActive, CreatedAt, IsDeleted)
VALUES
  -- QKUK Departments
  ('50000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000001',
   '30000000-0000-0000-0000-000000000001', '40000000-0000-0000-0000-000000000001',
   'Kardiologjia', 'Shërbime kardiologjike dhe zemrës', 1, GETUTCDATE(), 0),

  ('50000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000001',
   '30000000-0000-0000-0000-000000000001', '40000000-0000-0000-0000-000000000001',
   'Ortopedia', 'Shërbime ortopedike dhe kirurgjia kockave', 1, GETUTCDATE(), 0),

  ('50000000-0000-0000-0000-000000000003', '10000000-0000-0000-0000-000000000001',
   '30000000-0000-0000-0000-000000000001', '40000000-0000-0000-0000-000000000002',
   'Pediatria', 'Kujdesi shëndetësor për fëmijë', 1, GETUTCDATE(), 0),

  -- Policia Departments
  ('50000000-0000-0000-0000-000000000004', '10000000-0000-0000-0000-000000000002',
   '30000000-0000-0000-0000-000000000002', '40000000-0000-0000-0000-000000000003',
   'Dokumentet e Identitetit', 'Lëshim pasaporta dhe letra ID', 1, GETUTCDATE(), 0),

  ('50000000-0000-0000-0000-000000000005', '10000000-0000-0000-0000-000000000002',
   '30000000-0000-0000-0000-000000000002', '40000000-0000-0000-0000-000000000003',
   'Patenta Shoferit', 'Lëshim dhe rinovim i patentave', 1, GETUTCDATE(), 0),

  -- Komuna Departments
  ('50000000-0000-0000-0000-000000000006', '10000000-0000-0000-0000-000000000003',
   '30000000-0000-0000-0000-000000000003', '40000000-0000-0000-0000-000000000005',
   'Gjendja Civile', 'Certifikata lindjeje, martese dhe vdekjeje', 1, GETUTCDATE(), 0),

  ('50000000-0000-0000-0000-000000000007', '10000000-0000-0000-0000-000000000003',
   '30000000-0000-0000-0000-000000000003', '40000000-0000-0000-0000-000000000005',
   'Urbanizmi', 'Lejet e ndërtimit dhe urbanistike', 1, GETUTCDATE(), 0);
GO

-- ============================================================
-- 7. SERVICE CATEGORIES
-- ============================================================
INSERT INTO ServiceCategories (Id, TenantId, Name, Description, CreatedAt, IsDeleted)
VALUES
  ('60000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000001', 'Kontroll Mjekësor',   'Kontrolle rutinë dhe parandaluese', GETUTCDATE(), 0),
  ('60000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000001', 'Konsultë Specialiste','Konsulta me specialist', GETUTCDATE(), 0),
  ('60000000-0000-0000-0000-000000000003', '10000000-0000-0000-0000-000000000002', 'Dokumentet Zyrtare',  'Lëshim dhe rinovim dokumentesh', GETUTCDATE(), 0),
  ('60000000-0000-0000-0000-000000000004', '10000000-0000-0000-0000-000000000003', 'Çertifikata Civile',  'Certifikata zyrtare civile', GETUTCDATE(), 0),
  ('60000000-0000-0000-0000-000000000005', '10000000-0000-0000-0000-000000000003', 'Leje dhe Aprovime',   'Leje ndërtimi dhe aprovime', GETUTCDATE(), 0);
GO

-- ============================================================
-- 8. PUBLIC SERVICES
-- ============================================================
INSERT INTO PublicServices (Id, TenantId, DepartmentId, CategoryId, Name, Description, DurationMinutes, IsActive, CreatedAt, IsDeleted)
VALUES
  -- Kardiologjia
  ('70000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000001',
   '50000000-0000-0000-0000-000000000001', '60000000-0000-0000-0000-000000000001',
   'Kontroll Kardiologjik Rutinë', 'EKG dhe kontroll i presionit', 30, 1, GETUTCDATE(), 0),

  ('70000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000001',
   '50000000-0000-0000-0000-000000000001', '60000000-0000-0000-0000-000000000002',
   'Ekografi Kardiale',            'Ekokardiografi dydimensionale', 45, 1, GETUTCDATE(), 0),

  -- Ortopedia
  ('70000000-0000-0000-0000-000000000003', '10000000-0000-0000-0000-000000000001',
   '50000000-0000-0000-0000-000000000002', '60000000-0000-0000-0000-000000000002',
   'Konsultë Ortopedike',          'Vlerësim ortopedik dhe plan trajtimi', 30, 1, GETUTCDATE(), 0),

  -- Pediatria
  ('70000000-0000-0000-0000-000000000004', '10000000-0000-0000-0000-000000000001',
   '50000000-0000-0000-0000-000000000003', '60000000-0000-0000-0000-000000000001',
   'Kontroll Pediatrik',           'Kontroll rutinë i fëmijës', 20, 1, GETUTCDATE(), 0),

  -- Dokumentet e Identitetit
  ('70000000-0000-0000-0000-000000000005', '10000000-0000-0000-0000-000000000002',
   '50000000-0000-0000-0000-000000000004', '60000000-0000-0000-0000-000000000003',
   'Lëshim Pasaporte',             'Kërkesë dhe marrje e pasaportës', 20, 1, GETUTCDATE(), 0),

  ('70000000-0000-0000-0000-000000000006', '10000000-0000-0000-0000-000000000002',
   '50000000-0000-0000-0000-000000000004', '60000000-0000-0000-0000-000000000003',
   'Lëshim Letër Identiteti',      'Kërkesë dhe marrje e Letër ID', 15, 1, GETUTCDATE(), 0),

  -- Patenta
  ('70000000-0000-0000-0000-000000000007', '10000000-0000-0000-0000-000000000002',
   '50000000-0000-0000-0000-000000000005', '60000000-0000-0000-0000-000000000003',
   'Lëshim Patente Shoferi',       'Marrja e patentës së re', 30, 1, GETUTCDATE(), 0),

  ('70000000-0000-0000-0000-000000000008', '10000000-0000-0000-0000-000000000002',
   '50000000-0000-0000-0000-000000000005', '60000000-0000-0000-0000-000000000003',
   'Rinovim Patente Shoferi',      'Rinovimi i patentës ekzistuese', 15, 1, GETUTCDATE(), 0),

  -- Gjendja Civile
  ('70000000-0000-0000-0000-000000000009', '10000000-0000-0000-0000-000000000003',
   '50000000-0000-0000-0000-000000000006', '60000000-0000-0000-0000-000000000004',
   'Certifikatë Lindjeje',         'Lëshim certifikatë lindjeje', 15, 1, GETUTCDATE(), 0),

  ('70000000-0000-0000-0000-000000000010', '10000000-0000-0000-0000-000000000003',
   '50000000-0000-0000-0000-000000000006', '60000000-0000-0000-0000-000000000004',
   'Certifikatë Martese',          'Lëshim certifikatë martese', 15, 1, GETUTCDATE(), 0),

  -- Urbanizmi
  ('70000000-0000-0000-0000-000000000011', '10000000-0000-0000-0000-000000000003',
   '50000000-0000-0000-0000-000000000007', '60000000-0000-0000-0000-000000000005',
   'Leje Ndërtimi',                'Aplikim për leje ndërtimi', 45, 1, GETUTCDATE(), 0);
GO

-- ============================================================
-- 9. SERVICE REQUIREMENTS
-- ============================================================
INSERT INTO ServiceRequirements (Id, ServiceId, Description, CreatedAt, IsDeleted)
VALUES
  (NEWID(), '70000000-0000-0000-0000-000000000005', 'Certifikatë lindjeje ose dokument identifikimi', GETUTCDATE(), 0),
  (NEWID(), '70000000-0000-0000-0000-000000000005', 'Fotografi 3.5x4.5 cm (2 copë)', GETUTCDATE(), 0),
  (NEWID(), '70000000-0000-0000-0000-000000000005', '5 euro tarifë administrative', GETUTCDATE(), 0),
  (NEWID(), '70000000-0000-0000-0000-000000000006', 'Certifikatë lindjeje ose pasaportë', GETUTCDATE(), 0),
  (NEWID(), '70000000-0000-0000-0000-000000000006', 'Fotografi 3.5x4.5 cm (2 copë)', GETUTCDATE(), 0),
  (NEWID(), '70000000-0000-0000-0000-000000000007', 'Certifikatë mjekësore (aktuale)', GETUTCDATE(), 0),
  (NEWID(), '70000000-0000-0000-0000-000000000007', 'Letër identiteti ose pasaportë', GETUTCDATE(), 0),
  (NEWID(), '70000000-0000-0000-0000-000000000009', 'Libri amëzor ose dokument familjar', GETUTCDATE(), 0),
  (NEWID(), '70000000-0000-0000-0000-000000000011', 'Situacion urban i parcelës', GETUTCDATE(), 0),
  (NEWID(), '70000000-0000-0000-0000-000000000011', 'Projekt arkitektonik i noterizuar', GETUTCDATE(), 0);
GO

-- ============================================================
-- 10. STAFF MEMBERS
-- ============================================================
INSERT INTO StaffMembers (Id, TenantId, UserId, DepartmentId, Title, IsActive, CreatedAt, IsDeleted)
VALUES
  ('80000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000001',
   '20000000-0000-0000-0000-000000000005', '50000000-0000-0000-0000-000000000001',
   'Dr. Kardiolog', 1, GETUTCDATE(), 0),

  ('80000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000001',
   '20000000-0000-0000-0000-000000000006', '50000000-0000-0000-0000-000000000002',
   'Dr. Ortoped', 1, GETUTCDATE(), 0),

  ('80000000-0000-0000-0000-000000000003', '10000000-0000-0000-0000-000000000002',
   '20000000-0000-0000-0000-000000000007', '50000000-0000-0000-0000-000000000004',
   'Zyrtar Dokumentesh', 1, GETUTCDATE(), 0),

  ('80000000-0000-0000-0000-000000000004', '10000000-0000-0000-0000-000000000003',
   '20000000-0000-0000-0000-000000000008', '50000000-0000-0000-0000-000000000006',
   'Nëpunës Civil', 1, GETUTCDATE(), 0);
GO

-- ============================================================
-- 11. STAFF SCHEDULES
-- DayOfWeek: 0=Sunday,1=Monday,2=Tuesday,3=Wednesday,4=Thursday,5=Friday,6=Saturday
-- ============================================================
INSERT INTO StaffSchedules (Id, TenantId, StaffMemberId, DayOfWeek, StartTime, EndTime, IsActive, CreatedAt, IsDeleted)
VALUES
  -- Dr. Kardiolog - Hënë deri Premte 08:00-16:00
  (NEWID(), '10000000-0000-0000-0000-000000000001', '80000000-0000-0000-0000-000000000001', 1, '08:00:00', '16:00:00', 1, GETUTCDATE(), 0),
  (NEWID(), '10000000-0000-0000-0000-000000000001', '80000000-0000-0000-0000-000000000001', 2, '08:00:00', '16:00:00', 1, GETUTCDATE(), 0),
  (NEWID(), '10000000-0000-0000-0000-000000000001', '80000000-0000-0000-0000-000000000001', 3, '08:00:00', '16:00:00', 1, GETUTCDATE(), 0),
  (NEWID(), '10000000-0000-0000-0000-000000000001', '80000000-0000-0000-0000-000000000001', 4, '08:00:00', '16:00:00', 1, GETUTCDATE(), 0),
  (NEWID(), '10000000-0000-0000-0000-000000000001', '80000000-0000-0000-0000-000000000001', 5, '08:00:00', '14:00:00', 1, GETUTCDATE(), 0),

  -- Dr. Ortoped - Hënë deri Premte 09:00-17:00
  (NEWID(), '10000000-0000-0000-0000-000000000001', '80000000-0000-0000-0000-000000000002', 1, '09:00:00', '17:00:00', 1, GETUTCDATE(), 0),
  (NEWID(), '10000000-0000-0000-0000-000000000001', '80000000-0000-0000-0000-000000000002', 2, '09:00:00', '17:00:00', 1, GETUTCDATE(), 0),
  (NEWID(), '10000000-0000-0000-0000-000000000001', '80000000-0000-0000-0000-000000000002', 3, '09:00:00', '17:00:00', 1, GETUTCDATE(), 0),
  (NEWID(), '10000000-0000-0000-0000-000000000001', '80000000-0000-0000-0000-000000000002', 4, '09:00:00', '17:00:00', 1, GETUTCDATE(), 0),
  (NEWID(), '10000000-0000-0000-0000-000000000001', '80000000-0000-0000-0000-000000000002', 5, '09:00:00', '13:00:00', 1, GETUTCDATE(), 0),

  -- Zyrtar Policia - Hënë deri Premte 07:30-15:30
  (NEWID(), '10000000-0000-0000-0000-000000000002', '80000000-0000-0000-0000-000000000003', 1, '07:30:00', '15:30:00', 1, GETUTCDATE(), 0),
  (NEWID(), '10000000-0000-0000-0000-000000000002', '80000000-0000-0000-0000-000000000003', 2, '07:30:00', '15:30:00', 1, GETUTCDATE(), 0),
  (NEWID(), '10000000-0000-0000-0000-000000000002', '80000000-0000-0000-0000-000000000003', 3, '07:30:00', '15:30:00', 1, GETUTCDATE(), 0),
  (NEWID(), '10000000-0000-0000-0000-000000000002', '80000000-0000-0000-0000-000000000003', 4, '07:30:00', '15:30:00', 1, GETUTCDATE(), 0),
  (NEWID(), '10000000-0000-0000-0000-000000000002', '80000000-0000-0000-0000-000000000003', 5, '07:30:00', '15:30:00', 1, GETUTCDATE(), 0),

  -- Nëpunës Komuna - Hënë deri Premte 08:00-16:00
  (NEWID(), '10000000-0000-0000-0000-000000000003', '80000000-0000-0000-0000-000000000004', 1, '08:00:00', '16:00:00', 1, GETUTCDATE(), 0),
  (NEWID(), '10000000-0000-0000-0000-000000000003', '80000000-0000-0000-0000-000000000004', 2, '08:00:00', '16:00:00', 1, GETUTCDATE(), 0),
  (NEWID(), '10000000-0000-0000-0000-000000000003', '80000000-0000-0000-0000-000000000004', 3, '08:00:00', '16:00:00', 1, GETUTCDATE(), 0),
  (NEWID(), '10000000-0000-0000-0000-000000000003', '80000000-0000-0000-0000-000000000004', 4, '08:00:00', '16:00:00', 1, GETUTCDATE(), 0),
  (NEWID(), '10000000-0000-0000-0000-000000000003', '80000000-0000-0000-0000-000000000004', 5, '08:00:00', '14:00:00', 1, GETUTCDATE(), 0);
GO

-- ============================================================
-- 12. TIME SLOTS (Java e ardhshme — e lirë dhe e zënë)
-- ============================================================
INSERT INTO TimeSlots (Id, TenantId, ServiceId, StaffMemberId, StartTime, EndTime, IsAvailable, CreatedAt, IsDeleted)
VALUES
  -- Kardiologjia - Kontroll Rutinë
  ('90000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000001',
   '70000000-0000-0000-0000-000000000001', '80000000-0000-0000-0000-000000000001',
   '2026-05-12 08:00:00', '2026-05-12 08:30:00', 0, GETUTCDATE(), 0),  -- E zënë

  ('90000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000001',
   '70000000-0000-0000-0000-000000000001', '80000000-0000-0000-0000-000000000001',
   '2026-05-12 08:30:00', '2026-05-12 09:00:00', 1, GETUTCDATE(), 0),  -- E lirë

  ('90000000-0000-0000-0000-000000000003', '10000000-0000-0000-0000-000000000001',
   '70000000-0000-0000-0000-000000000001', '80000000-0000-0000-0000-000000000001',
   '2026-05-12 09:00:00', '2026-05-12 09:30:00', 1, GETUTCDATE(), 0),

  ('90000000-0000-0000-0000-000000000004', '10000000-0000-0000-0000-000000000001',
   '70000000-0000-0000-0000-000000000001', '80000000-0000-0000-0000-000000000001',
   '2026-05-13 08:00:00', '2026-05-13 08:30:00', 0, GETUTCDATE(), 0),  -- E zënë

  ('90000000-0000-0000-0000-000000000005', '10000000-0000-0000-0000-000000000001',
   '70000000-0000-0000-0000-000000000001', '80000000-0000-0000-0000-000000000001',
   '2026-05-13 08:30:00', '2026-05-13 09:00:00', 1, GETUTCDATE(), 0),

  -- Ortopedia
  ('90000000-0000-0000-0000-000000000006', '10000000-0000-0000-0000-000000000001',
   '70000000-0000-0000-0000-000000000003', '80000000-0000-0000-0000-000000000002',
   '2026-05-12 09:00:00', '2026-05-12 09:30:00', 1, GETUTCDATE(), 0),

  ('90000000-0000-0000-0000-000000000007', '10000000-0000-0000-0000-000000000001',
   '70000000-0000-0000-0000-000000000003', '80000000-0000-0000-0000-000000000002',
   '2026-05-12 09:30:00', '2026-05-12 10:00:00', 0, GETUTCDATE(), 0),  -- E zënë

  -- Pasaporta
  ('90000000-0000-0000-0000-000000000008', '10000000-0000-0000-0000-000000000002',
   '70000000-0000-0000-0000-000000000005', '80000000-0000-0000-0000-000000000003',
   '2026-05-12 07:30:00', '2026-05-12 07:50:00', 0, GETUTCDATE(), 0),  -- E zënë

  ('90000000-0000-0000-0000-000000000009', '10000000-0000-0000-0000-000000000002',
   '70000000-0000-0000-0000-000000000005', '80000000-0000-0000-0000-000000000003',
   '2026-05-12 07:50:00', '2026-05-12 08:10:00', 1, GETUTCDATE(), 0),

  ('90000000-0000-0000-0000-000000000010', '10000000-0000-0000-0000-000000000002',
   '70000000-0000-0000-0000-000000000005', '80000000-0000-0000-0000-000000000003',
   '2026-05-12 08:10:00', '2026-05-12 08:30:00', 1, GETUTCDATE(), 0),

  -- Certifikatë Lindjeje
  ('90000000-0000-0000-0000-000000000011', '10000000-0000-0000-0000-000000000003',
   '70000000-0000-0000-0000-000000000009', '80000000-0000-0000-0000-000000000004',
   '2026-05-12 08:00:00', '2026-05-12 08:15:00', 0, GETUTCDATE(), 0),  -- E zënë

  ('90000000-0000-0000-0000-000000000012', '10000000-0000-0000-0000-000000000003',
   '70000000-0000-0000-0000-000000000009', '80000000-0000-0000-0000-000000000004',
   '2026-05-12 08:15:00', '2026-05-12 08:30:00', 1, GETUTCDATE(), 0);
GO

-- ============================================================
-- 13. APPOINTMENTS
-- Status: 0=Pending, 1=Confirmed, 2=Cancelled, 3=Completed, 4=NoShow
-- ============================================================
INSERT INTO Appointments (Id, TenantId, UserId, TimeSlotId, [Status], Notes, CreatedAt, IsDeleted)
VALUES
  ('A0000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000001',
   '20000000-0000-0000-0000-000000000010', '90000000-0000-0000-0000-000000000001',
   1, 'Kontroll rutinë kardiologjik', GETUTCDATE(), 0),   -- Confirmed

  ('A0000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000001',
   '20000000-0000-0000-0000-000000000011', '90000000-0000-0000-0000-000000000004',
   0, NULL, GETUTCDATE(), 0),                              -- Pending

  ('A0000000-0000-0000-0000-000000000003', '10000000-0000-0000-0000-000000000001',
   '20000000-0000-0000-0000-000000000012', '90000000-0000-0000-0000-000000000007',
   3, 'Dhimbje gjuri', GETUTCDATE(), 0),                  -- Completed

  ('A0000000-0000-0000-0000-000000000004', '10000000-0000-0000-0000-000000000002',
   '20000000-0000-0000-0000-000000000013', '90000000-0000-0000-0000-000000000008',
   1, 'Pasaportë e re', GETUTCDATE(), 0),                 -- Confirmed

  ('A0000000-0000-0000-0000-000000000005', '10000000-0000-0000-0000-000000000003',
   '20000000-0000-0000-0000-000000000014', '90000000-0000-0000-0000-000000000011',
   2, NULL, GETUTCDATE(), 0);                             -- Cancelled
GO

-- ============================================================
-- 14. APPOINTMENT STATUS HISTORY
-- ============================================================
INSERT INTO AppointmentStatusHistories (Id, AppointmentId, OldStatus, NewStatus, Reason, ChangedByUserId, CreatedAt, IsDeleted)
VALUES
  (NEWID(), 'A0000000-0000-0000-0000-000000000001', 0, 1,
   'Konfirmuar nga stafi', '20000000-0000-0000-0000-000000000005', GETUTCDATE(), 0),

  (NEWID(), 'A0000000-0000-0000-0000-000000000003', 0, 1,
   'Konfirmuar nga stafi', '20000000-0000-0000-0000-000000000005', GETUTCDATE(), 0),

  (NEWID(), 'A0000000-0000-0000-0000-000000000003', 1, 3,
   'Termin i përfunduar', '20000000-0000-0000-0000-000000000005', GETUTCDATE(), 0),

  (NEWID(), 'A0000000-0000-0000-0000-000000000004', 0, 1,
   'Konfirmuar nga stafi', '20000000-0000-0000-0000-000000000007', GETUTCDATE(), 0),

  (NEWID(), 'A0000000-0000-0000-0000-000000000005', 0, 2,
   'Anuluar nga qytetari', '20000000-0000-0000-0000-000000000014', GETUTCDATE(), 0);
GO

-- ============================================================
-- 15. NOTIFICATION TEMPLATES
-- ============================================================
INSERT INTO NotificationTemplates (Id, TenantId, Name, Subject, Body, [Type], IsActive, CreatedAt, IsDeleted)
VALUES
  (NEWID(), '10000000-0000-0000-0000-000000000001',
   'Konfirmim Termini', 'Termini juaj u konfirmua',
   'Pershendetje {{FirstName}}, termini juaj me {{ServiceName}} eshte konfirmuar per {{Date}} ne oren {{Time}}.',
   'Email', 1, GETUTCDATE(), 0),

  (NEWID(), '10000000-0000-0000-0000-000000000001',
   'Kujtues Termini', 'Kujtues: Termini juaj nesër',
   'Pershendetje {{FirstName}}, ju kujtojme se keni termin neser, {{Date}} ne oren {{Time}} per {{ServiceName}}.',
   'Email', 1, GETUTCDATE(), 0),

  (NEWID(), '10000000-0000-0000-0000-000000000001',
   'Anulim Termini', 'Termini juaj u anulua',
   'Pershendetje {{FirstName}}, termini juaj per {{ServiceName}} ne {{Date}} u anulua. Mund te rezervoni nje tjeter ne platforme.',
   'Email', 1, GETUTCDATE(), 0),

  (NEWID(), '10000000-0000-0000-0000-000000000002',
   'Konfirmim Dokumenti', 'Dokumenti juaj eshte gati',
   'Pershendetje {{FirstName}}, dokumenti juaj {{DocumentType}} eshte gati per marrje.',
   'SMS', 1, GETUTCDATE(), 0),

  (NEWID(), '10000000-0000-0000-0000-000000000003',
   'Njoftim Certifikate', 'Certifikata juaj eshte procesuar',
   'Pershendetje {{FirstName}}, certifikata {{CertType}} eshte procesuar dhe gati per marrje.',
   'SMS', 1, GETUTCDATE(), 0);
GO

-- ============================================================
-- 16. NOTIFICATIONS
-- ============================================================
INSERT INTO Notifications (Id, TenantId, UserId, Title, [Message], IsRead, [Type], CreatedAt, IsDeleted)
VALUES
  (NEWID(), '10000000-0000-0000-0000-000000000001',
   '20000000-0000-0000-0000-000000000010',
   'Termini u konfirmua',
   'Termini juaj kardiologjik për 12 Maj 2026, ora 08:00 u konfirmua.',
   1, 'Appointment', GETUTCDATE(), 0),

  (NEWID(), '10000000-0000-0000-0000-000000000001',
   '20000000-0000-0000-0000-000000000011',
   'Termin i ri rezervuar',
   'Keni rezervuar termin kardiologjik për 13 Maj 2026, ora 08:00.',
   0, 'Appointment', GETUTCDATE(), 0),

  (NEWID(), '10000000-0000-0000-0000-000000000001',
   '20000000-0000-0000-0000-000000000012',
   'Termini u përfundua',
   'Termini juaj ortopedik u shënua si i përfunduar. Faleminderit!',
   1, 'Appointment', GETUTCDATE(), 0),

  (NEWID(), '10000000-0000-0000-0000-000000000002',
   '20000000-0000-0000-0000-000000000013',
   'Termini u konfirmua',
   'Termini juaj për lëshim pasaporte, 12 Maj 2026 ora 07:30 u konfirmua.',
   0, 'Appointment', GETUTCDATE(), 0),

  (NEWID(), '10000000-0000-0000-0000-000000000003',
   '20000000-0000-0000-0000-000000000014',
   'Termini u anulua',
   'Termini juaj për certifikatë lindjeje u anulua. Mund të rezervoni termin të ri.',
   0, 'Appointment', GETUTCDATE(), 0);
GO

-- ============================================================
-- 17. AUDIT LOGS
-- ============================================================
INSERT INTO AuditLogs (Id, TenantId, UserId, Action, EntityName, EntityId, OldValues, NewValues, IpAddress, CreatedAt, IsDeleted)
VALUES
  (NEWID(), '10000000-0000-0000-0000-000000000001',
   '20000000-0000-0000-0000-000000000010',
   'CREATE', 'Appointment', 'A0000000-0000-0000-0000-000000000001',
   NULL, '{"Status":"Pending"}', '192.168.1.101', GETUTCDATE(), 0),

  (NEWID(), '10000000-0000-0000-0000-000000000001',
   '20000000-0000-0000-0000-000000000005',
   'UPDATE', 'Appointment', 'A0000000-0000-0000-0000-000000000001',
   '{"Status":"Pending"}', '{"Status":"Confirmed"}', '192.168.1.10', GETUTCDATE(), 0),

  (NEWID(), '10000000-0000-0000-0000-000000000002',
   '20000000-0000-0000-0000-000000000013',
   'CREATE', 'Appointment', 'A0000000-0000-0000-0000-000000000004',
   NULL, '{"Status":"Pending"}', '10.0.0.55', GETUTCDATE(), 0),

  (NEWID(), '10000000-0000-0000-0000-000000000003',
   '20000000-0000-0000-0000-000000000014',
   'UPDATE', 'Appointment', 'A0000000-0000-0000-0000-000000000005',
   '{"Status":"Pending"}', '{"Status":"Cancelled"}', '172.16.0.22', GETUTCDATE(), 0),

  (NEWID(), '10000000-0000-0000-0000-000000000001',
   '20000000-0000-0000-0000-000000000001',
   'CREATE', 'Institution', '30000000-0000-0000-0000-000000000001',
   NULL, '{"Name":"QKUK"}', '10.0.0.1', GETUTCDATE(), 0);
GO

-- ============================================================
-- VERIFIKIM
-- ============================================================
SELECT 'Tenants'                   AS Tabela, COUNT(*) AS Rekorde FROM Tenants                    UNION ALL
SELECT 'TenantSettings',                      COUNT(*)            FROM TenantSettings              UNION ALL
SELECT 'Users',                               COUNT(*)            FROM Users                       UNION ALL
SELECT 'Institutions',                        COUNT(*)            FROM Institutions                UNION ALL
SELECT 'InstitutionBranches',                 COUNT(*)            FROM InstitutionBranches         UNION ALL
SELECT 'Departments',                         COUNT(*)            FROM Departments                 UNION ALL
SELECT 'ServiceCategories',                   COUNT(*)            FROM ServiceCategories           UNION ALL
SELECT 'PublicServices',                      COUNT(*)            FROM PublicServices              UNION ALL
SELECT 'ServiceRequirements',                 COUNT(*)            FROM ServiceRequirements         UNION ALL
SELECT 'StaffMembers',                        COUNT(*)            FROM StaffMembers                UNION ALL
SELECT 'StaffSchedules',                      COUNT(*)            FROM StaffSchedules              UNION ALL
SELECT 'TimeSlots',                           COUNT(*)            FROM TimeSlots                   UNION ALL
SELECT 'Appointments',                        COUNT(*)            FROM Appointments                UNION ALL
SELECT 'AppointmentStatusHistories',          COUNT(*)            FROM AppointmentStatusHistories  UNION ALL
SELECT 'Notifications',                       COUNT(*)            FROM Notifications               UNION ALL
SELECT 'NotificationTemplates',               COUNT(*)            FROM NotificationTemplates       UNION ALL
SELECT 'AuditLogs',                           COUNT(*)            FROM AuditLogs;
GO
