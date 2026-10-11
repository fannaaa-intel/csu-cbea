# Business rules — attendance policy

All rules follow the **Cagayan State University Student Manual (Revised 2019)**. Where the manual says nothing, the system uses a default that the Admin can change in **Settings**.

## What the CSU manual says

| Source | Rule |
|---|---|
| **s.II.B.4 Class Attendance** | "Class attendance shall be required in all courses; provided, absences may be allowed for not more than 20% of the total class hours required of the course or be dropped after two (2) warnings." |
| **s.II.B.4 (excused absences)** | "For excused absences, however, the faculty may prescribe other requirements in lieu of physical attendance depending on the specific requirements of the course." |
| **s.II.C.2.1 Dropping of Students** | "A student shall be dropped from the course if he/she has an accumulated absence of at least twenty percent (20%) of the total number of class hours in a semester." |
| **s.III.B.6.5 Guidance referral** | Lists "absenteeism, tardiness" as at-risk behaviors for guidance referral (the manual sets no tardiness limit). |
| **CHED MORPHE 2008, s.101** (national basis) | A student absent for more than 20% of the prescribed class periods fails and earns no credit, unless exempted for a just and reasonable ground. |

## How the system applies them

| # | Rule | Default | Setting key |
|---|---|---|---|
| R1 | Absences are counted in **class hours** (an absence = that meeting's length, e.g. 1.5 h). | — | — |
| R2 | **Total class hours** of a class = number of meeting days between term start and end × hours per meeting. Auto-computed, editable per class. | — | `classes.total_hours` |
| R3 | **Warning 1** when absent hours reach this % of total hours. | 10% | `warning1_pct` |
| R4 | **Warning 2** when absent hours reach this %. | 15% | `warning2_pct` |
| R5 | **Dropped** when absent hours reach **at least** this %. (CSU s.II.C.2.1) | 20% | `drop_pct` |
| R6 | **Late** = arrived more than N minutes after class start. CSU has no rule, so this is a school default. | 15 min | `late_minutes` |
| R7 | **Frequent tardiness** alert (guidance referral, CSU s.III.B.6.5) after N lates in one class. | 3 | `tardy_alert_count` |
| R8 | Optional: convert N lates into 1 absence. **Off by default** because the CSU manual has no such rule. | 0 (off) | `lates_per_absence` |
| R9 | **Excused** absences need a faculty requirement (e.g. make-up activity). While **Pending** they count as absent; once **Complied** they no longer count. (CSU s.II.B.4) | — | `attendance_records.excuse_status` |
| R10 | A teacher may edit a saved attendance session for N days; Admin anytime. Every edit is in the audit log. | 3 days | `edit_window_days` |
| R11 | Warnings are issued once per level per student per class and are kept as a record even if attendance is later corrected. The **current** status always comes from `v_attendance_status`. | — | `attendance_warnings` |
| R12 | "Dropped" in the system means the student **reached the drop limit**. The official dropping is processed by the Registrar; the system flags it and notifies the student, teacher and Admin. | — | — |

### Worked example
3-unit lecture meeting Mon/Wed, 1.5 h per meeting, Aug 17 – Dec 18 → 36 meetings × 1.5 h = **54 h**.
- Warning 1 at 5.4 h (4th absence), Warning 2 at 8.1 h (6th absence), **Dropped at 10.8 h (8th absence)**.

## Who can do what

| Action | Admin | Teacher | Student |
|---|---|---|---|
| Manage users, students, teachers, terms, subjects, sections, classes, enrollment | ✓ | | |
| Take attendance | ✓ (any class) | ✓ (own classes) | |
| Edit saved attendance | ✓ anytime | ✓ within edit window | |
| Mark excuse requirement complied | ✓ | ✓ (own classes) | |
| View attendance & status | all | own classes | own records |
| Reports (print / export) | all | own classes | own report |
| Settings, audit log | ✓ | | |

## Sources
- Cagayan State University, *Student Manual (Revised 2019)* — https://lallo.csu.edu.ph/uploads/downloads/Student%20Manual%20(2019).pdf
- CHED Memorandum Order No. 40, s. 2008 (MORPHE), Section 101 — https://legaldex.com/laws/manual-of-regulations-for-private-higher-education-of-2008
