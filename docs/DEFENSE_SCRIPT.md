# Defense script — CSU Student Attendance Management System

About 10 minutes. Import the demo SQL fresh the morning of the defense, so the data matches this script.
On a **Tuesday**, mbautista's classes AIS 314 · BSAIS 3A / 3B meet, so "Today's classes" has something to show.

## Demo flow

| # | Who | Click | Say |
|---|---|---|---|
| 1 | — | Start the app | "The sign-in checks the database connection first — green means MySQL is running." |
| 2 | `admin` / `admin123` | Dashboard | "200 enrollments across 10 classes. The standing bar shows how many students are Good, Warning 1, Warning 2 or Dropped. The chart shows absence hours per subject." |
| 3 | admin | **Settings** | "These are the CSU Student Manual rules: 10% and 15% warnings, dropped at 20% (s.II.B.4, s.II.C.2.1). The admin can change them; nothing is hard-coded." |
| 4 | admin | **Classes** → double-click AIS 311 · BSAIS 3B | "Total class hours are computed from the term dates and meeting days: 36 meetings × 1.5 h = 54 hours. 20% of 54 is 10.8 hours." Close. |
| 5 | admin | Log out → `mbautista` / `teacher123` | "The teacher only sees her own classes." |
| 6 | teacher | Dashboard → **Take attendance** on today's class | Mark 2 **A**, 1 **L**, 1 **E** — each asks for a **reason** (attach a photo for one); E also asks for a requirement. Show the counters change. Type a student no. in quick check-in + Enter. **Ctrl+S**. "One transaction saves the session and every record. Then the warnings engine checks every student and notifies them." |
| 7 | teacher | **Section Attendance** → BSAIS 3B → AIS 311 | "24-1024 is Dropped: 12 of 10.8 allowed hours. 24-1028 is at Warning 2." Click **V** on 24-1024: every meeting with its reason and attachment. Switch to *All subjects* to see the whole section. |
| 8 | teacher | **Reports** → *Warning & Drop List* → **Print preview** → Microsoft Print to PDF | "Every report prints to PDF or exports to Excel. This list is for the Registrar." |
| 9 | teacher | Log out → `24-1024` / `student123` | "The student sees a red banner, a red bar on AIS 311 and an amber one on AIS 314. The bell shows the notifications that explain why." |
| 10 | — | Close | "Rules are configurable, passwords are hashed, and every change is in the audit log." |

## Likely panel questions (with short answers)

1. **Why count absences in hours instead of days?**
   The CSU Student Manual says absences may not exceed 20% of the *total class hours* (s.II.B.4, s.II.C.2.1). A 3-hour lab absence is worth more than a 1-hour lecture absence, so counting hours is fair and matches the manual.

2. **Where does the 20% come from?**
   CSU Student Manual (Revised 2019) s.II.C.2.1: a student is dropped with accumulated absences of at least 20% of total class hours. It also follows CHED MORPHE s.101. The 10% and 15% warning levels are the "two (2) warnings" in s.II.B.4.

3. **How are passwords stored?**
   Never as plain text. Each password is hashed with PBKDF2 (HMAC-SHA256, 10,000 iterations) and a random 16-byte salt. We store `PBKDF2$iterations$salt$hash`. At sign-in we hash what was typed with the same salt and compare.

4. **How do you prevent SQL injection?**
   Every query passes user input as parameters (`Db.P("@name", value)`), never by joining strings. The database treats parameters as values, not as SQL code.

5. **What is a transaction and where do you use it?**
   A group of writes that all succeed or all fail. Saving attendance writes one session plus a record per student. Adding a student creates the user account and the student row. If any step fails, everything is rolled back, so there is never half-saved data.

6. **Why are the percentages computed in database views?**
   `v_attendance_status` is the single source of truth: every screen and report reads the same numbers, so they can never disagree. When the admin changes a threshold in Settings, every status updates immediately.

7. **How do roles work?**
   Each user has a role (Admin, Teacher, Student). The menu is built per role, the page factory refuses pages a role may not open, teachers' queries are filtered to their own classes, and students only see their own records.

8. **What happens if a teacher makes a mistake?**
   They can edit a saved session for 3 days (configurable). After that only the Admin can edit. Every save, edit and delete is written to the audit log with who and when.

9. **How are warnings issued without duplicates?**
   After each save, the system reads the status of every student in the class. Warnings are stored with a unique key on class + student + level, so the same warning is never issued or notified twice.

10. **What is "Excused" and how does it affect the count?**
    Per s.II.B.4, faculty may require other work in lieu of attendance. An excused absence counts while the requirement is *Pending* and stops counting once the teacher marks it *Complied*.

## If something goes wrong during the demo
- **"Cannot connect"**: start MySQL in XAMPP and sign in again. The app does not crash.
- Data looks different: re-import `database/csu_attendance.sql` (it resets the demo data).
