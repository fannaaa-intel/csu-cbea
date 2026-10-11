# Work order: CSU Student Attendance Management System

| | |
|---|---|
| **Client** | BSAIS 3B proponents (Pallago, Dela Cruz, Arellano), Cagayan State University |
| **Deliverable** | Working desktop system (no documentation revisions) |
| **Stack** | VB.NET WinForms (.NET Framework 4.8, Visual Studio 2022) + XAMPP MySQL |
| **Deadline** | Tuesday (defense) |
| **Rules basis** | CSU Student Manual (Revised 2019) s.II.B.4, s.II.C.2.1, and CHED MORPHE s.101 (see BUSINESS_RULES.md) |

## 1. Scope

### Must (the system fails without these)
- [x] Database: 14 tables + 3 views, demo data (Module 1)
- [x] Login, role-based shell, change password, forced change on first login (Module 1)
- [x] Subjects CRUD (Module 1, reference pattern)
- [ ] Terms, Sections, Teachers, Students CRUD with automatic accounts (2A)
- [ ] User accounts, Classes (auto total hours), Enrollment (2B)
- [ ] **Take Attendance** P/L/A/E, edit window, warnings engine (3)
- [ ] Attendance Records + pending excuses, My Students (3)
- [ ] Student dashboard with hours bar + status, history, profile (4)
- [ ] Reports with print/PDF + CSV export (6)

### Should (makes it look finished)
- [ ] Admin + Teacher dashboards (5)
- [ ] Notifications page + bell (5)
- [ ] Settings page, Audit log page (6)
- [ ] Quick check-in box (type/scan student no.) (3)

### Won't (out of scope; matches their proposal boundaries)
Grades, enrollment/admission processing, payroll, biometrics/GPS/RFID hardware, SMS/email, LAN multi-PC setup (connection string ready for it), mobile app.

## 2. Schedule

| When | Module | Done when |
|---|---|---|
| Sun night | **0** check build + DB · **2A** Terms, Sections, Teachers, Students | New student can log in and is forced to change password |
| Mon morning | **2B** Users, Classes, Enrollment | A new class auto-computes 54 h; students can be enrolled |
| Mon midday | **3** Take Attendance, Records, My Students | Saving attendance issues Warning 1 + bell notification |
| Mon afternoon | **4** Student side · **5** Dashboards + Notifications | All three roles land on a real dashboard |
| Mon night | **6** Reports, Settings, Audit | Warning & Drop List prints to PDF |
| Tue morning | **7** Polish + QA, demo rehearsal, handover walkthrough | Demo script runs start to finish with no errors |

Buffer: if you fall behind, skip Should items in this order: Audit page → Quick check-in → Settings page (keep the defaults) → dashboard chart.

## 3. Setup on the build PC (once)
1. Install **XAMPP** (Apache not needed) → start **MySQL**.
2. phpMyAdmin → Import → `database/csu_attendance.sql` (creates DB `csu_attendance` with demo data).
3. Visual Studio 2022 with the **.NET desktop development** workload (includes .NET Framework 4.8 targeting pack).
4. Open `src/CSUAttendance.sln` → it restores **MySqlConnector** from NuGet automatically → F5.
5. Log in as `admin / admin123`.
6. Open the `CSU-AttendanceSystem` folder in Claude Code and run the prompts in `docs/PROMPTS.md` in order.

## 4. Acceptance checklist (test before handover)
- [ ] Each role logs in and sees only its own menu; wrong password shows an inline error; deactivated user cannot log in.
- [ ] Admin creates term → section → subject → teacher → student → class → enrollment without touching phpMyAdmin.
- [ ] Teacher takes attendance for today; counters update; saving twice doesn't duplicate (edit mode).
- [ ] Marking a student absent enough times raises Warning 1 → Warning 2 → Dropped at 10/15/20% of total hours, with notifications to the student, teacher and admin (Dropped only).
- [ ] Excused + Pending counts; marking Complied removes those hours from the student's total.
- [ ] Teacher cannot edit a session older than 3 days; Admin can.
- [ ] Student sees the correct hours bar and status; matches the Class Summary report.
- [ ] Every report previews, prints to PDF, and exports CSV that opens in Excel.
- [ ] Closing XAMPP MySQL → the login shows a friendly "Cannot connect" message (no crash).
- [ ] Release build runs on a second PC with XAMPP + the SQL imported.

## 5. Demo script (about 10 minutes)
1. **Login as admin**: dashboard stats, absence-hours chart, recent warnings → show Settings (10/15/20% from the CSU Student Manual).
2. **Classes**: open AIS 311 · BSAIS 3B → total hours 54 computed from the schedule.
3. **Log out → login as `mbautista`**: Today's classes → Take Attendance → mark 2 absent, 1 late, 1 excused (with requirement) → Save → toast + bell.
4. **My Students**: 24-1024 shows **Dropped** (12 of 10.8 h), 24-1028 **Warning 2**.
5. **Reports**: Warning & Drop List → Print preview → Save as PDF.
6. **Log out → login as `24-1024`** (student123): banner + red bar on AIS 311, amber on AIS 314; notifications explain why.
7. Close with: rules are configurable; passwords are hashed; every change is audit-logged.

## 6. Handover
- Zip the whole folder (without `bin/` and `obj/`) + a fresh `csu_attendance.sql` export.
- 30–45 min walkthrough with the three proponents: database diagram, the views, one CRUD page, Take Attendance save flow, the rules engine.
- They should be able to answer: why hours instead of sessions, where the 20% comes from, how passwords are stored, what a transaction is used for.
