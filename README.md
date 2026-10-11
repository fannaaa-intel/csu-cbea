# CSU Student Attendance Management System

Desktop attendance system for **Cagayan State University**: records Present / Late / Absent / Excused per class meeting, tracks absences in **class hours**, and warns students at 10% and 15% before the **20% drop limit** in the CSU Student Manual (s.II.B.4, s.II.C.2.1).

**Stack:** VB.NET WinForms (.NET Framework 4.8) · XAMPP MySQL/MariaDB · MySqlConnector

## Quick start
1. **XAMPP**: start **MySQL** in the XAMPP Control Panel.
2. **Database**: open http://localhost/phpmyadmin → **Import** → choose `database/csu_attendance.sql` → Go.
   (`csu_attendance_clean.sql` = empty system with only `admin / admin123`, forced password change.)
3. **Visual Studio 2022** (workload ".NET desktop development"): open `src/CSUAttendance.sln` → wait for NuGet restore → **F5**.
4. Sign in:

| Role | Username | Password |
|---|---|---|
| Admin | `admin` | `admin123` |
| Teacher | `mbautista`, `jvillanueva`, `kdomingo` | `teacher123` |
| Student | `24-1001` … `24-1040` | `student123` |

Demo students with alerts: **24-1024** (Dropped in AIS 311, Warning 1 in AIS 314), **24-1028** (Warning 2), **24-1026** (Warning 1), **24-1030** (frequent tardiness).

Database settings live in `src/CSUAttendance/App.config` (default XAMPP: `root`, no password, `localhost:3306`).

## Folder map
```
CLAUDE.md                  rules Claude Code follows while building
docs/WORK_ORDER.md         scope, schedule, acceptance checklist, demo script
docs/PROMPTS.md            the Claude Code prompt for each module (run in order)
docs/BUSINESS_RULES.md     CSU attendance rules and how the system applies them
database/                  csu_attendance.sql (with demo data), csu_attendance_clean.sql
src/CSUAttendance.sln      Visual Studio solution
src/CSUAttendance/
  Core/                    database helper, password hashing, session, settings, rules engine, audit, notifications
  UI/                      Theme (colors, fonts, styles, icons) and custom controls
  Forms/                   Login, Main shell, Change password
  Pages/                   one UserControl per screen (SubjectsPage = reference)
  Dialogs/                 add/edit dialogs (SubjectDialog = reference)
```

## Database at a glance
`users` · `teachers` · `students` · `sections` · `school_terms` · `subjects` · `classes` · `class_enrollments` · `attendance_sessions` · `attendance_records` · `attendance_warnings` · `notifications` · `audit_log` · `settings`
Views: `v_attendance_counts` → `v_attendance_hours` → **`v_attendance_status`** (hours absent, allowed hours, % absent, Good / Warning 1 / Warning 2 / Dropped).

## Troubleshooting
| Problem | Fix |
|---|---|
| "Cannot connect to the database" on login | Start MySQL in XAMPP; check port 3306 and `App.config`. |
| Import fails in phpMyAdmin | Make sure you imported the whole file; it drops and recreates `csu_attendance`. |
| Icons show as squares | Needs Windows 10/11 (uses the built-in Segoe MDL2 Assets font). |
| NuGet restore fails | Check internet, then in VS: Tools → NuGet Package Manager → Restore. |
| Blurry text | Already DPI-aware; if still blurry, right-click the exe → Properties → Compatibility → Change high DPI settings → "Application". |
