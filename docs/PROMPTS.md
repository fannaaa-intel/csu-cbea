# Claude Code prompts — one per module

How to use:
1. Open the **CSU-AttendanceSystem** folder in Claude Code (it reads `CLAUDE.md` automatically).
2. Paste **one** prompt, let it finish, then run the app in Visual Studio (F5) and do the click-test it gives you.
3. If something is wrong, describe exactly what you saw ("clicking Save on Students shows error X"). Fix before the next module.
4. Start a **new Claude Code session (or `/clear`) between modules** so each one starts with a clean context.

Module 1 (foundation, database, login, shell, Subjects page) is **already done**.

---

## Module 0: Check the foundation (5 min)

```
Read CLAUDE.md, docs/BUSINESS_RULES.md and docs/WORK_ORDER.md. Then:
1. Run `dotnet build src/CSUAttendance.sln` and fix any errors (the foundation was compile-checked, but my machine may differ).
2. Check the database exists: C:\xampp\mysql\bin\mysql.exe -u root csu_attendance -e "SELECT COUNT(*) FROM v_attendance_status"
   If it fails, tell me to import database/csu_attendance.sql in phpMyAdmin first.
3. Do not build any new feature yet. Summarize the architecture back to me in 10 lines and list anything that looks wrong.
```

---

## Module 2A: Admin master data (Terms, Sections, Teachers, Students)

```
Build Module 2A following CLAUDE.md. Copy the SubjectsPage + SubjectDialog pattern exactly (designer files, toolbar, CardPanel grid, empty state, count, toast, audit).

1. TermsPage + TermDialog (school_terms): school year (e.g. 2026-2027), semester combo (1st Semester/2nd Semester/Summer), start/end date pickers (end > start).
   Grid shows an "Active" badge column. Toolbar button "Set as active" sets is_active=1 for the selected term and 0 for all others in ONE transaction.
   Block delete if the term has classes (Db.IsInUse).

2. SectionsPage + SectionDialog (sections): section name (unique, e.g. BSAIS 3B), program, year level (1-5). Grid shows student count.

3. TeachersPage + TeacherDialog (teachers + users): employee no., first/middle/last name, department, contact no., username.
   On ADD: in one Db.InTransaction create the users row (role Teacher, full_name "First M. Last", password = employee no. hashed with PasswordHasher.HashPassword, must_change_password = 1) then the teachers row.
   Show the default password in a MessageBox after creating ("Give this to the teacher; they must change it at first login").
   On EDIT: update teachers + users.full_name/username. Duplicate username / employee no. → ErrorProvider message.
   Block delete if the teacher has classes.

4. StudentsPage + StudentDialog (students + users): student no. (format check like 24-1234, unique), first/middle/last name, sex combo, section combo, contact no., guardian name, guardian contact, status (Active/Inactive).
   Filters in toolbar: search box + section combo ("All sections"). Grid: student no, name (Last, First M.), section, sex, status badge.
   On ADD: one transaction creating users (role Student, username = student no., password = student no., must_change_password = 1) then students with user_id.
   On EDIT: keep users.username and users.full_name in sync with student no. and name.
   Delete: if the student has attendance records, refuse and suggest setting status to Inactive.

Register keys "terms", "sections", "teachers", "students" in PageRegistry.Create. Build with 0 errors.
Finish with a click-test list for me (add, edit, search, delete-blocked cases, duplicate cases, the new student can log in and is forced to change password).
```

---

## Module 2B: User accounts, Classes, Enrollment

```
Build Module 2B following CLAUDE.md and the SubjectsPage pattern.

1. UsersPage (users): grid of username, full name, role badge, Active/Inactive badge, last login. Filters: search + role combo.
   Toolbar: "Add admin" (dialog: username, full name, temporary password; creates role Admin with must_change_password=1),
   "Reset password" (resets to the username for students, employee no. for teachers, or a typed temporary password for admins; uses AuthService.ResetPassword; confirm first),
   "Activate/Deactivate" (toggle is_active; you cannot deactivate yourself or the last active admin).
   Teacher/student accounts are NOT created here (they come from the Teachers/Students pages); say so in a muted hint label.

2. ClassesPage + ClassDialog (classes): term combo (default active term), subject combo, section combo, teacher combo,
   day checkboxes Mon–Sat (stored as the SET string, e.g. "Mon,Wed"), start and end time (DateTimePicker Format=Time, ShowUpDown=True), room,
   total hours = AttendanceRules.ComputeTotalHours(term start, term end, days, start, end) shown live as a label, with an "Override" checkbox + NumericUpDown to type a different value.
   Validate: at least one day, end > start. Warn (Yes/No to continue) if the same teacher OR the same section already has a class in the same term on an overlapping day and time.
   Grid: subject code, description, section, teacher, schedule text (e.g. "Mon, Wed 8:00–9:30 AM"), room, total hours, enrolled count. Filter by term + section + search.
   Block delete if attendance sessions exist.

3. EnrollmentPage: class combo at the top (active term, text like "AIS 311 · BSAIS 3B · Bautista").
   Two cards side by side: left "Enrolled" grid, right "Available students" grid (active students NOT enrolled in this class; filter by section, default = the class's section, plus search).
   Buttons: "Add selected →", "Add whole section", "← Remove selected" (confirm; if the student has attendance in this class, warn that their records will stop showing).
   Multi-select allowed in both grids for this page only. Use INSERT IGNORE for adding. Audit every change.

Register "users", "classes", "enrollment". Build with 0 errors. Give me a click-test list.
```

---

## Module 3: Take Attendance (the most important screen), Records, My Students

```
Build Module 3 following CLAUDE.md. This screen is what the panel will watch, so it must be fast, clear and keyboard-friendly.

1. TakeAttendancePage (key "take"). Build it in code (no designer needed) using Theme and the custom controls.
   - Top bar: Class combo (teacher: own classes in the active term; admin: all), DateTimePicker (default today, cannot be in the future), "Load" happens automatically on change.
     If the class does not meet on that weekday (AttendanceRules.MeetsOn), show an amber info banner but still allow it.
   - If NavigationArg is a Tuple(Of Integer, Date) (class_id, date), preselect them (used by the Records page and the teacher dashboard).
   - Summary strip: StatusBadge-like counters "Present 32 · Late 3 · Absent 2 · Excused 1" updating live, plus the session length ("1.5 hrs").
   - Quick check-in box: TextField "Type or scan student no. + Enter" → marks that student Present or Late using AttendanceRules.StatusFromTimeIn(class start, current time) when the date is today, sets time_in = now, highlights the row briefly. Unknown number → small red hint.
   - Roster: a scrollable panel of AttendanceRow controls (create Controls/AttendanceRow.vb): row number, student no., name, current attendance_status badge from v_attendance_status (so the teacher sees who is at Warning 1/2/Dropped), four toggle chips P / L / A / E (selected chip = solid status color, others outline), time-in label, remarks TextField.
     Choosing E opens a small dialog asking for the excuse requirement (required text) → excuse_status = Pending.
   - Keyboard: Up/Down moves the focused row, P/L/A/E sets status, Ctrl+S saves.
   - New session: everyone defaults to Present. Existing session: load saved records (edit mode). "Mark all Present" button.
   - If the session exists and AttendanceRules.CanEditSession(date) is False → read-only with a banner "Locked: sessions can be edited for N days. Ask the Admin.".
   - Save (primary button): Db.InTransaction → INSERT attendance_sessions ... ON DUPLICATE KEY UPDATE (hours = AttendanceRules.SessionHours(class start,end), recorded_by), get session_id,
     then for each student INSERT attendance_records ... ON DUPLICATE KEY UPDATE status, time_in, hours_absent (AttendanceRules.HoursAbsentFor), remarks, excuse_requirement, excuse_status, updated_by.
     After commit: AttendanceRules.EvaluateClass(classId); Audit.Log("SAVE","Attendance",session_id,"class/date/counts"); Host.RefreshBell();
     Toast "Attendance saved" and, if warnings were issued, a second toast like "2 students reached Warning 1" (kind Warning).
   - CanLeave(): if there are unsaved changes ask "Discard unsaved attendance?".

2. RecordsPage (key "records"): filters class combo, date range (default this month). Grid of sessions: date, day, class, P/L/A/E counts, recorded by, last updated. Double-click → Host.Navigate("take", Tuple.Create(classId, date)).
   Second tab or toggle "Pending excuses": list Excused records with excuse_status = Pending (student, class, date, requirement) → button "Mark complied" → update + EvaluateClass + audit.

3. MyStudentsPage (key "mystudents"; admins can reach it from Records too): class combo → grid from v_attendance_status: student no, name, P, L, A, E, hours absent, allowed hours, % absent, status badge (PaintBadgeCell).
   Sort so Dropped/Warning students are on top. Double-click → dialog with that student's dated history in this class.

Register the keys. Build with 0 errors. Click-test list must include: take today's attendance for a class, a student crossing Warning 2 (log in as jvillanueva, class AIS 312 · BSAIS 3B, mark 24-1026 Absent on 2 new dates: 6 h → 9 h = 16.7%), the bell count increases, editing an old (locked) session as a teacher vs as admin, and the quick check-in box.
```

---

## Module 4: Student side

```
Build Module 4 following CLAUDE.md (Session.User.StudentId).

1. StudentDashboardPage (key "dashboard" when role = Student; update PageRegistry so the key picks the right dashboard per role):
   - Greeting "Hi, {first name}" + term label (e.g. "1st Semester 2026-2027").
   - If any subject is Warning 1/2/Dropped or Tardiness: a soft banner at the top in the matching status color explaining it in one sentence and citing "CSU Student Manual s.II.B.4".
   - One CardPanel per subject (2 columns, FlowLayoutPanel): subject code + description, teacher, schedule, HoursBar (PercentAbsent, thresholds from AppSettings), text "6.0 of 10.8 allowed absence hours", StatusBadge, and a small counts row P / L / A / E.
   - Clicking a card → Host.Navigate("history", subjectCode).
2. HistoryPage (key "history"): subject filter combo (All + each subject; preselect from NavigationArg), grid of date, day, subject, status badge, time in, remarks, excuse requirement/status. Newest first. Count label.
3. ProfilePage (key "profile"): read-only card with student no., name, section, sex, contact, guardian, username, last login, and a "Change password" button opening ChangePasswordForm.

Build with 0 errors. Click-test: log in as 24-1024 (Dropped + Warning 1), 24-1030 (Tardiness), 24-1001 (all Good).
```

---

## Module 5: Admin & Teacher dashboards, Notifications

```
Build Module 5 following CLAUDE.md. Dashboards are built in code with StatCard, CardPanel, SimpleBarChart, StatusBadge.

1. AdminDashboardPage (key "dashboard" for Admin):
   Row of 4 StatCards: Active students · Classes this term · Today's attendance rate ((Present+Late)/all records today, "—" if none) · Students needing attention (distinct students with status <> 'Good' in the active term).
   Card "Absence hours by subject" → SimpleBarChart (SUM(hours_absent) per subject_code from v_attendance_status, active term).
   Card "Recent warnings" → small grid of the latest 8 attendance_warnings (date, student, subject, level badge).
   Card "Attendance not yet taken today" → classes meeting today (AttendanceRules.MeetsOn) without a session for today.
2. TeacherDashboardPage (key "dashboard" for Teacher):
   StatCards: My classes · My students · This week's attendance rate · Needs attention.
   "Today's classes" cards (classes meeting today, sorted by start time): subject, section, time, room, and either a primary "Take attendance" button → Host.Navigate("take", Tuple.Create(classId, Date.Today)) or a green "Taken ✓ · 32 P · 2 A" state with an "Edit" link.
   "Needs attention" list: my students with Warning 1/2/Dropped/Tardiness (name, subject, status badge, % absent).
3. NotificationsPage (key "notifications", opened from the bell): list of notifications as rows (colored dot by type, bold title if unread, message, relative time like "2 hours ago"), "Mark all as read" button, empty state "You're all caught up". After showing, mark all read and call Host.RefreshBell().

Register keys. Build with 0 errors. Click-test with admin, mbautista, and a student.
```

---

## Module 6: Reports, Settings, Audit log

```
Build Module 6 following CLAUDE.md. No report libraries: use PrintDocument + PrintPreviewDialog (users can choose "Microsoft Print to PDF") and CSV export.

1. Create Core/ReportPrinter.vb: prints any DataTable with a header (school name from AppSettings, report title, filter line, "Generated {date} by {user}"), column headers, zebra rows, automatic column widths, landscape option, multi-page with "Page X of Y", and a footer with "Prepared by: ____________". Also Core/CsvExporter.vb (SaveFileDialog, UTF-8 with BOM so Excel opens it, quote fields).

2. ReportsPage (key "reports"): left list of report types, right filter panel + preview grid + buttons "Print preview" (primary), "Export CSV".
   Reports (teachers are limited to their own classes; students don't see this page):
   a. Daily Attendance: class + date → student no, name, status, time in, remarks.
   b. Class Attendance Summary: class → v_attendance_status columns (P/L/A/E, hours absent, allowed, %, status).
   c. Student Attendance Report: student picker → per subject summary + dated history.
   d. Warning & Drop List: term + optional section → students with Warning 1/2/Dropped/Tardiness, highest level, % absent, date issued. (Most useful for the Registrar.)
   e. Attendance by Date Range: class + from/to dates (weekly/monthly) → per student counts in that range.

3. SettingsPage (key "settings", Admin): one card per rule with a NumericUpDown, its description from the settings table, and the CSU manual citation as muted text. Save → AppSettings.SetValue for each + Audit. Button "Re-check all warnings" runs EvaluateClass for every class in the active term and toasts how many were issued. Also edit school_name.

4. AuditLogPage (key "audit", Admin): filters date range + action combo + search; grid date/time, user, action, entity, id, details. Newest first, LIMIT 500.

Register keys. Build with 0 errors. Click-test: print preview each report, save one as PDF, export one CSV and open it in Excel, change late_minutes and see it applied.
```

---

## Module 7: Polish, QA and handover

```
Final pass following CLAUDE.md. Do not add new features.
1. Remove PlaceholderPage usages from PageRegistry (every key now has a real page). Make sure every menu item opens without error for Admin, Teacher, Student.
2. Go through every page: tab order, AcceptButton/CancelButton on dialogs, empty states, counts, toasts, long names don't overflow, grids have sensible column widths, no hard-coded colors outside Theme.
3. Check every SQL uses parameters, every write is audited, and every page catches exceptions with ShowError.
4. Run `dotnet build -c Release` with 0 errors and 0 warnings if possible.
5. Write docs/USER_GUIDE.md: install steps (XAMPP, import SQL, run), demo accounts, and a step-by-step for each role with what to click.
6. Write docs/DEFENSE_SCRIPT.md: a 10-minute demo flow (see WORK_ORDER.md "Demo script") and the 10 questions a panel is most likely to ask about this system with short answers the students can give (database design, why hours not sessions, CSU manual rules, password hashing, SQL injection, views, transactions, roles).
Then list anything you think is still risky for the demo.
```

---

## When something breaks (paste this)

```
Bug: <what I clicked> → <what happened / exact error text>. Expected: <what should happen>.
Find the cause, fix it with the smallest change that follows CLAUDE.md, rebuild with 0 errors, and tell me how to re-test.
```
