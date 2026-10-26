# CSU Student Attendance Management System — User Guide

## 1. Install (once per PC)

1. Install **XAMPP**, open the XAMPP Control Panel and click **Start** beside **MySQL** (Apache is not needed).
2. Open **phpMyAdmin** (http://localhost/phpmyadmin) → **Import** → choose `database/csu_attendance.sql` → **Go**.
   - `csu_attendance.sql` = full demo data (40 students, 10 classes, 8 weeks of attendance).
   - `csu_attendance_clean.sql` = empty system (only the `admin` account; you must set a new password at first sign-in).
3. Run the app: open `src/CSUAttendance.sln` in Visual Studio 2022 and press **F5**, or run `CSUAttendance.exe` from the Release folder.
4. The sign-in screen shows **● Database connected** in green. If it shows red, MySQL is not running.

> Using another PC as the server: change `Server=localhost` in `CSUAttendance.exe.config` to that PC's IP address.

## 2. Demo accounts

| Role | Username | Password |
|---|---|---|
| Admin | `admin` | `admin123` |
| Teacher | `mbautista`, `jvillanueva`, `kdomingo` | `teacher123` |
| Student | `24-1001` … `24-1040` | `student123` |

Interesting students: **24-1024** (Dropped in AIS 311, Warning 1 in AIS 314), **24-1028** (Warning 2), **24-1030** (Tardiness).

New accounts start with a temporary password and must set a new one at first sign-in (at least 8 characters with letters and numbers).

## 3. Admin

| To do this | Go to | Steps |
|---|---|---|
| Start a semester | **School Terms** | **Add term** → school year, semester, dates → Save. Select it → **Set as active**. |
| Add sections / subjects | **Sections**, **Subjects** | **Add** → fill in → Save. Typing `BSAIS 4A` fills in the program and year. |
| Add a teacher | **Teachers** | **Add teacher**. The login account is created automatically (password = employee no.). |
| Add a student | **Students** | **Add student**. Username and first password = student no. (e.g. 24-1041). |
| Schedule a class | **Classes** | **Add class** → term, subject, section, teacher, meeting days, times. Total class hours are computed automatically (e.g. 36 meetings × 1.5 h = 54 h). |
| Enroll students | **Enrollment** | Pick the class → select students on the left → **Add selected**, or **Add whole section**. |
| Reset a password / deactivate | **User Accounts** | Select the account → **Reset password** or **Deactivate**. |
| Change the rules | **Settings** | Change a value → **Save settings**. **Re-check all warnings** issues warnings students have newly reached. |
| See who did what | **Audit Log** | Filter by date, action or search text. |

Admins can also take or edit attendance for any class, at any time (open a session from **Attendance Records**).

## 4. Teacher

1. **Dashboard** → *Today's classes* shows each class meeting today. Click **Take attendance**.
2. On **Take Attendance**, everyone starts as **Present**. Click **P / L / A / E** for each student, or use the keyboard:
   - **↑ / ↓** move between students, **P L A E** set the status, **F2** edits the reason, **Ctrl+S** saves.
   - **Quick check-in**: type or scan a student no. and press **Enter**. The student is marked Present, or Late if more than 15 minutes after the start.
   - **L, A and E always ask for a reason** (required) and let you **attach a file** (photo or PDF of the excuse letter / medical certificate). Click the reason or the paperclip later to change it.
   - **E (Excused)** also asks what the student must do instead (e.g. a reflection paper). The hours count as absent until the requirement is **Complied**.
   - Save is blocked while any Late / Absent / Excused student has no reason (quick check-in Lates show *Reason required* in red).
3. Click **Save attendance**. A message tells you if any student reached Warning 1 / Warning 2 / Dropped; they are notified automatically.
4. **Attendance Records** → *Sessions*: double-click a session to edit it (allowed for 3 days). *Pending excuses*: select → **Mark complied**.
5. **Section Attendance**: pick a **section**, then one subject or **All subjects**. Every student is listed with P / L / A / E counts and status, at-risk students first. Click the **V** button (or double-click) to open the student's meetings right under the row: date, status, time in, reason and **View attachment**. Hover a name to see the latest five meetings. **Hours & history** shows the absence-hours bar for one subject.
6. **Reports**: choose a report → **Print preview** → printer icon → **Microsoft Print to PDF** to save a PDF, or **Export CSV** to open in Excel.

## 5. Student

- **My Attendance**: one card per subject with the absence-hours bar (ticks at the Warning 1 and Warning 2 levels) and your status. A banner explains any warning.
- Click a subject card, or open **Attendance History**, to see every class meeting.
- **My Profile** → **Change password**.
- The **bell** (top right) shows new notifications (warnings, drops, tardiness).

## 6. The rules (CSU Student Manual, Revised 2019)

- Absences are counted in **class hours** (a 1.5-hour meeting missed = 1.5 hours).
- **Warning 1 at 10%**, **Warning 2 at 15%**, **Dropped at 20%** of the class's total hours (s.II.B.4, s.II.C.2.1).
- **Late** = more than 15 minutes after the start. **3 lates** = tardiness alert (guidance referral, s.III.B.6.5).
- **Excused** absences count until the faculty requirement is marked **Complied** (s.II.B.4).
- All of these numbers can be changed by the Admin in **Settings**.

## 7. Troubleshooting

| Problem | Fix |
|---|---|
| "Cannot connect to the database" | Start MySQL in the XAMPP Control Panel, then sign in again. |
| "Unknown database csu_attendance" | Import `database/csu_attendance.sql` in phpMyAdmin. |
| A teacher cannot edit an old session | Sessions lock after the edit window (3 days). The Admin can edit, or raise the window in Settings. |
| A student is missing from Take Attendance | Enroll them in the class on the **Enrollment** page. |
