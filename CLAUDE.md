# CSU Student Attendance Management System — project rules

Desktop app for Cagayan State University (BSAIS 3B system project). **Deadline: Tuesday.** Build module by module, keep it working after every step.

## Stack (fixed — do not change)
- **VB.NET WinForms, .NET Framework 4.8**, SDK-style project `src/CSUAttendance/CSUAttendance.vbproj`, solution `src/CSUAttendance.sln` (Visual Studio 2022).
- **XAMPP MySQL/MariaDB** database `csu_attendance` via **MySqlConnector** (NuGet, `Imports MySqlConnector`).
- No other NuGet packages, no paid UI kits (no Guna/Bunifu), no RDLC/Crystal Reports, no chart libraries.
- `Option Strict On`. Every file has explicit `Imports System.Drawing` / `System.Windows.Forms` when it needs them.

## Build & check
- Build: `dotnet build src/CSUAttendance.sln` — must end with **0 errors**. Fix every error before moving on.
- DB CLI (Windows/XAMPP): `C:\xampp\mysql\bin\mysql.exe -u root csu_attendance -e "SQL"` — use it to test queries before putting them in code.
- The user tests the UI by pressing F5 in Visual Studio. After each module, list exactly what they should click to verify it.

## Folder map
```
database/csu_attendance.sql        schema + demo data (import this)   | csu_attendance_clean.sql = empty system
src/CSUAttendance/Core/            Db, PasswordHasher, Session, AppSettings, AuthService, AttendanceRules, Audit, Notifier
src/CSUAttendance/UI/Theme.vb      colors, fonts, StylePrimary/Secondary/Danger/Ghost, StyleGrid, PaintBadgeCell, Icons
src/CSUAttendance/UI/Ui.vb         builders for code-built pages (Toolbar, Card, CardHeader, Grid, Combo, Action, EmptyState, Greeting)
src/CSUAttendance/UI/Chrome/       Notice (message dialog), SheetForm (dialog base), Backdrop, PillButton
src/CSUAttendance/UI/Controls/     CardPanel, TextField, StatusBadge, NavButton, StatCard, HoursBar, SimpleBarChart, Toast,
                                   Banner, TabStrip, CountChips, StandingLadder, ClassTimeline, SubjectTicket
src/CSUAttendance/Forms/           LoginForm, MainForm (shell), ChangePasswordForm
src/CSUAttendance/Pages/           PageBase, PageRegistry (menu + role guard + factory), SubjectsPage (REFERENCE list page) + all module pages
src/CSUAttendance/Dialogs/         SubjectDialog (REFERENCE add/edit dialog) + all module dialogs
src/CSUAttendance/Core/            also Lookups (term/section/class combos), ReportPrinter, CsvExporter, TextUtil, Attachments (proof files)
docs/                              WORK_ORDER.md, PROMPTS.md, BUSINESS_RULES.md, USER_GUIDE.md, DEFENSE_SCRIPT.md
```

## Patterns to follow (copy, don't reinvent)
- **New page** = `Partial Class XxxPage` inheriting `PageBase` with a `.Designer.vb` file, like `SubjectsPage`. Override `PageTitle`, load data in `OnPageShown()` (never in the constructor), override `CanLeave()` if it has unsaved edits. Register it in `PageRegistry.Create()` replacing the `PlaceholderPage` line.
- **Add/Edit** = modal dialog that inherits `SheetForm` (rose side rail) like `SubjectDialog` (id 0 = add). Designer controls start at X = 238 (right of the rail); call `SetRail(glyph, caption, note)` in Load. Validate with `ErrorProvider`. Return `DialogResult.OK`; the page reloads and shows `ShowToast("... saved")`.
- **List layout** = toolbar panel (Dock Top, 52px: search `TextField` left, buttons right) + `CardPanel` (Dock Fill) containing a `DataGridView` styled with `Theme.StyleGrid`, an empty-state label, and a count label.
- **Status columns** in grids: handle `CellPainting` and call `Theme.PaintBadgeCell(e)` for that column.
- **Complex dynamic screens** (Take Attendance, dashboards, student subject cards) may build controls in code, but still use Theme + the custom controls.
- Navigation between pages: `Host.Navigate("key", arg)`; read `NavigationArg` in the target page. New keys must also be allowed in `PageRegistry.IsAllowed` (menu items are allowed automatically).
- Designer files must stay designer-compatible: only property assignments and `Controls.Add` inside `InitializeComponent` — no loops, no Theme calls, no custom logic (apply Theme styles in `OnPageShown`/`Load`).

## Data access rules
- Only through `Db.Query / Execute / Scalar / Insert` with `Db.P("@name", value)` parameters. **Never concatenate user input into SQL.**
- Multi-step writes (e.g. create student + user account, save attendance) use `Db.InTransaction(Sub(cn, tx) ... )` with the transaction overloads.
- Read DataRow values with `Db.ToInt/ToDec/ToStr/ToBool/ToDate/ToTime` (handle DBNull). MySqlConnector types: TIME→TimeSpan, DATE→Date, TINYINT(1)→Boolean, COUNT→Long.
- Catch duplicates with `Catch ex As Exception When Db.IsDuplicate(ex)` and FK "in use" with `Db.IsInUse(ex)`.
- Call `Audit.Log(action, entity, id, details)` after every create/update/delete/attendance save.
- **The schema is fixed.** Do not add/rename tables or columns without asking the user first. Use the views.

## Attendance rules (CSU Student Manual, Revised 2019 — see docs/BUSINESS_RULES.md)
- Counted in **class hours**. Absent (and Excused while `excuse_status='Pending'`) adds the session's hours; Excused + `Complied` does not count.
- **Warning 1 at 10%, Warning 2 at 15%, Dropped at ≥20%** of the class's `total_hours` (values in `settings`, read via `AppSettings`).
- Late = arrived more than `late_minutes` (15) after class start → `AttendanceRules.StatusFromTimeIn`. `tardy_alert_count` (3) lates → Tardiness alert. Lates do NOT convert to absences unless `lates_per_absence` > 0.
- **Compare or sort status words (`attendance_status`) in VB, never in SQL** — the view's text can have a different collation than query literals and MySQL then throws "Illegal mix of collations".
- **Never recompute percentages in VB.** Read `v_attendance_status` (columns: class_id, student_id, student_no, student_name, term_id, teacher_id, subject_code, subject_description, section_name, total_hours, session_hours, sessions_recorded, present_count, late_count, absent_count, excused_count, hours_absent, allowed_hours, percent_absent, attendance_status).
- After saving attendance or changing an excuse status, call `AttendanceRules.EvaluateClass(classId)`; it issues warnings + notifications (no duplicates) and returns the newly issued ones for a toast.
- Teachers may edit a session only within `edit_window_days` (`AttendanceRules.CanEditSession`); Admin always. Teachers only see their own classes (`classes.teacher_id = Session.User.TeacherId`). Students only see their own data (`Session.User.StudentId`).
- Only one `school_terms.is_active = 1` term; pages default to the active term.

## UI rules (minimal, consistent)
- **Display scaling:** the client PC runs at 150%. Every pixel size set in code goes through `Theme.Dpi(n)`; designer sizes are scaled automatically. Combos use `Theme.StyleCombo` (36 px owner-drawn). Grids hide scrollbars (mouse wheel still scrolls).
- **No `MessageBox`.** Use `Notice.Confirm / Info / Warn / Fail / Success` (or `Confirm`/`ShowError` in pages). Destructive confirms pass `danger:=True`.
- **Design is CSU-specific** — do not copy the look of other projects (no solid colored sidebar, no gradient welcome banner, no soft-circle decorations; use the diagonal `Theme.DrawStripes`).
- Main window and sign-in open maximized; layouts must fit 1920×1080 at 150% without page scrollbars.
- Colors/fonts only from `Theme` (rose accent `#D94F70`, soft pink `#FCE8EC`, bg `#F7F7F8`, cards white, border `#E6E6EA`, text `#1F2328` / muted `#6B7280`). Segoe UI 10; titles 18 semibold. 8px spacing grid; controls 36px tall; grid rows 34px.
- One primary (rose) button per screen. Secondary = outline, destructive = `StyleDanger`.
- Success feedback = `Toast` (non-blocking). `MessageBox` only for confirmations and errors.
- Every list has a search box, an empty-state message, and a count. Double-click a row = edit/open.
- Status is always color **and** text (StatusBadge / PaintBadgeCell). Status words: Present, Late, Absent, Excused, Good, Warning 1, Warning 2, Dropped, Tardiness, Pending, Complied, Active, Inactive.
- Icons: `Icons.*` constants (Segoe MDL2 Assets). Use `Theme.StylePrimary(btn, Icons.Add)` to put an icon on a button.
- Tab order must be logical; Enter = save (AcceptButton), Esc = cancel (CancelButton).

## Demo accounts (password)
admin (admin123) · mbautista / jvillanueva / kdomingo (teacher123) · students 24-1001 … 24-1040 (student123).
Interesting demo students: 24-1024 (Dropped in AIS 311 + Warning 1 in AIS 314), 24-1028 (Warning 2), 24-1030 (Tardiness).

## Working style
- Do one module per session (docs/PROMPTS.md). Plan briefly, implement, `dotnet build`, fix, then give the user a short click-test list.
- Don't rewrite working foundation files (Core, Theme, Controls, MainForm) unless a module truly needs a small addition; say what you changed.
- Keep code readable for students who must explain it at the defense: short methods, clear names, a one-line `''' <summary>` on each class.
