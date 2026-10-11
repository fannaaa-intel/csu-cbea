-- =====================================================================
--  CSU Student Attendance Management System
--  Database schema  (XAMPP MySQL / MariaDB 10.4+  and  MySQL 8.0+)
--
--  Attendance rules follow the Cagayan State University Student Manual
--  (Revised 2019):
--    s.II.B.4   absences allowed up to 20% of total class hours,
--               student dropped after two (2) warnings
--    s.II.C.2.1 dropped when accumulated absence reaches at least 20%
--               of the total number of class hours in a semester
--  All thresholds are stored in the `settings` table (editable by Admin).
-- =====================================================================

DROP DATABASE IF EXISTS csu_attendance;
CREATE DATABASE csu_attendance CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
USE csu_attendance;

-- ---------------------------------------------------------------------
-- Accounts
-- ---------------------------------------------------------------------
CREATE TABLE users (
    user_id               INT AUTO_INCREMENT PRIMARY KEY,
    username              VARCHAR(50)  NOT NULL UNIQUE,
    password_hash         VARCHAR(255) NOT NULL,          -- PBKDF2$iterations$salt$hash
    role                  ENUM('Admin','Teacher','Student') NOT NULL,
    full_name             VARCHAR(150) NOT NULL,
    email                 VARCHAR(100) NULL,
    is_active             TINYINT(1)   NOT NULL DEFAULT 1,
    must_change_password  TINYINT(1)   NOT NULL DEFAULT 1,
    last_login            DATETIME     NULL,
    created_at            DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE TABLE teachers (
    teacher_id    INT AUTO_INCREMENT PRIMARY KEY,
    user_id       INT NOT NULL UNIQUE,
    employee_no   VARCHAR(20)  NOT NULL UNIQUE,
    first_name    VARCHAR(50)  NOT NULL,
    middle_name   VARCHAR(50)  NULL,
    last_name     VARCHAR(50)  NOT NULL,
    department    VARCHAR(100) NULL,
    contact_no    VARCHAR(20)  NULL,
    created_at    DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_teachers_user FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- Academic structure
-- ---------------------------------------------------------------------
CREATE TABLE sections (
    section_id    INT AUTO_INCREMENT PRIMARY KEY,
    section_name  VARCHAR(30) NOT NULL UNIQUE,            -- e.g. BSAIS 3B
    program       VARCHAR(20) NOT NULL,                   -- e.g. BSAIS
    year_level    TINYINT     NOT NULL
) ENGINE=InnoDB;

CREATE TABLE students (
    student_id        INT AUTO_INCREMENT PRIMARY KEY,
    user_id           INT NULL UNIQUE,
    student_no        VARCHAR(20)  NOT NULL UNIQUE,
    first_name        VARCHAR(50)  NOT NULL,
    middle_name       VARCHAR(50)  NULL,
    last_name         VARCHAR(50)  NOT NULL,
    sex               ENUM('Male','Female') NOT NULL,
    section_id        INT NULL,
    contact_no        VARCHAR(20)  NULL,
    guardian_name     VARCHAR(100) NULL,
    guardian_contact  VARCHAR(20)  NULL,
    status            ENUM('Active','Inactive') NOT NULL DEFAULT 'Active',
    created_at        DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_students_user    FOREIGN KEY (user_id)    REFERENCES users(user_id)       ON DELETE SET NULL,
    CONSTRAINT fk_students_section FOREIGN KEY (section_id) REFERENCES sections(section_id) ON DELETE SET NULL
) ENGINE=InnoDB;

CREATE TABLE school_terms (
    term_id      INT AUTO_INCREMENT PRIMARY KEY,
    school_year  VARCHAR(9)  NOT NULL,                    -- e.g. 2026-2027
    semester     ENUM('1st Semester','2nd Semester','Summer') NOT NULL,
    start_date   DATE NOT NULL,
    end_date     DATE NOT NULL,
    is_active    TINYINT(1) NOT NULL DEFAULT 0,           -- only one active term at a time
    UNIQUE KEY uq_term (school_year, semester)
) ENGINE=InnoDB;

CREATE TABLE subjects (
    subject_id    INT AUTO_INCREMENT PRIMARY KEY,
    subject_code  VARCHAR(20)  NOT NULL UNIQUE,
    description   VARCHAR(150) NOT NULL,
    units         DECIMAL(3,1) NOT NULL DEFAULT 3.0
) ENGINE=InnoDB;

-- A class = one subject, taught to one section, by one teacher, in one term
CREATE TABLE classes (
    class_id     INT AUTO_INCREMENT PRIMARY KEY,
    term_id      INT NOT NULL,
    subject_id   INT NOT NULL,
    section_id   INT NOT NULL,
    teacher_id   INT NOT NULL,
    days         SET('Mon','Tue','Wed','Thu','Fri','Sat') NOT NULL,
    start_time   TIME NOT NULL,
    end_time     TIME NOT NULL,
    room         VARCHAR(30) NULL,
    total_hours  DECIMAL(6,2) NOT NULL,                   -- total class hours in the semester (auto-computed, editable)
    is_active    TINYINT(1) NOT NULL DEFAULT 1,
    UNIQUE KEY uq_class (term_id, subject_id, section_id),
    CONSTRAINT fk_classes_term    FOREIGN KEY (term_id)    REFERENCES school_terms(term_id),
    CONSTRAINT fk_classes_subject FOREIGN KEY (subject_id) REFERENCES subjects(subject_id),
    CONSTRAINT fk_classes_section FOREIGN KEY (section_id) REFERENCES sections(section_id),
    CONSTRAINT fk_classes_teacher FOREIGN KEY (teacher_id) REFERENCES teachers(teacher_id)
) ENGINE=InnoDB;

CREATE TABLE class_enrollments (
    enrollment_id  INT AUTO_INCREMENT PRIMARY KEY,
    class_id       INT NOT NULL,
    student_id     INT NOT NULL,
    enrolled_at    DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UNIQUE KEY uq_enrollment (class_id, student_id),
    CONSTRAINT fk_enr_class   FOREIGN KEY (class_id)   REFERENCES classes(class_id)   ON DELETE CASCADE,
    CONSTRAINT fk_enr_student FOREIGN KEY (student_id) REFERENCES students(student_id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- Attendance
-- ---------------------------------------------------------------------
-- One row per class meeting (a class can only have one session per date)
CREATE TABLE attendance_sessions (
    session_id    INT AUTO_INCREMENT PRIMARY KEY,
    class_id      INT  NOT NULL,
    session_date  DATE NOT NULL,
    hours         DECIMAL(4,2) NOT NULL,                  -- length of this meeting in hours
    recorded_by   INT  NULL,                              -- users.user_id
    created_at    DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at    DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE KEY uq_session (class_id, session_date),
    CONSTRAINT fk_sess_class FOREIGN KEY (class_id)    REFERENCES classes(class_id) ON DELETE CASCADE,
    CONSTRAINT fk_sess_user  FOREIGN KEY (recorded_by) REFERENCES users(user_id)    ON DELETE SET NULL
) ENGINE=InnoDB;

-- One row per student per session
CREATE TABLE attendance_records (
    record_id           INT AUTO_INCREMENT PRIMARY KEY,
    session_id          INT NOT NULL,
    student_id          INT NOT NULL,
    status              ENUM('Present','Late','Absent','Excused') NOT NULL DEFAULT 'Present',
    time_in             TIME NULL,
    hours_absent        DECIMAL(4,2) NOT NULL DEFAULT 0,  -- = session hours when Absent/Excused, else 0
    remarks             VARCHAR(255) NULL,
    excuse_requirement  VARCHAR(255) NULL,                -- what the faculty requires in lieu of attendance
    excuse_status       ENUM('Pending','Complied') NULL,  -- Excused + Pending counts as absent; Complied does not
    updated_by          INT NULL,
    updated_at          DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE KEY uq_record (session_id, student_id),
    CONSTRAINT fk_rec_session FOREIGN KEY (session_id) REFERENCES attendance_sessions(session_id) ON DELETE CASCADE,
    CONSTRAINT fk_rec_student FOREIGN KEY (student_id) REFERENCES students(student_id)            ON DELETE CASCADE,
    CONSTRAINT fk_rec_user    FOREIGN KEY (updated_by) REFERENCES users(user_id)                  ON DELETE SET NULL
) ENGINE=InnoDB;

-- The "two (2) warnings" before dropping, plus frequent-tardiness alerts
CREATE TABLE attendance_warnings (
    warning_id    INT AUTO_INCREMENT PRIMARY KEY,
    class_id      INT NOT NULL,
    student_id    INT NOT NULL,
    level         ENUM('Warning 1','Warning 2','Dropped','Tardiness') NOT NULL,
    hours_absent  DECIMAL(6,2) NOT NULL DEFAULT 0,
    percent       DECIMAL(5,2) NOT NULL DEFAULT 0,
    late_count    INT NOT NULL DEFAULT 0,
    issued_at     DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    issued_by     INT NULL,                               -- users.user_id (NULL = issued by system)
    UNIQUE KEY uq_warning (class_id, student_id, level),
    CONSTRAINT fk_warn_class   FOREIGN KEY (class_id)   REFERENCES classes(class_id)   ON DELETE CASCADE,
    CONSTRAINT fk_warn_student FOREIGN KEY (student_id) REFERENCES students(student_id) ON DELETE CASCADE,
    CONSTRAINT fk_warn_user    FOREIGN KEY (issued_by)  REFERENCES users(user_id)       ON DELETE SET NULL
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- System
-- ---------------------------------------------------------------------
CREATE TABLE notifications (
    notification_id  INT AUTO_INCREMENT PRIMARY KEY,
    user_id          INT NOT NULL,
    title            VARCHAR(100) NOT NULL,
    message          VARCHAR(500) NOT NULL,
    type             ENUM('Info','Success','Warning','Danger') NOT NULL DEFAULT 'Info',
    is_read          TINYINT(1) NOT NULL DEFAULT 0,
    created_at       DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    KEY ix_notif_user (user_id, is_read),
    CONSTRAINT fk_notif_user FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE audit_log (
    log_id      INT AUTO_INCREMENT PRIMARY KEY,
    user_id     INT NULL,
    action      VARCHAR(50)  NOT NULL,                    -- e.g. LOGIN, CREATE, UPDATE, DELETE
    entity      VARCHAR(50)  NOT NULL,                    -- e.g. Student, Attendance
    entity_id   INT NULL,
    details     VARCHAR(1000) NULL,
    created_at  DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    KEY ix_audit_date (created_at),
    CONSTRAINT fk_audit_user FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE SET NULL
) ENGINE=InnoDB;

CREATE TABLE settings (
    setting_key    VARCHAR(50)  PRIMARY KEY,
    setting_value  VARCHAR(100) NOT NULL,
    description    VARCHAR(255) NULL
) ENGINE=InnoDB;

INSERT INTO settings (setting_key, setting_value, description) VALUES
 ('school_name',        'Cagayan State University',  'Shown on login screen and report headers'),
 ('warning1_pct',       '10',  'Warning 1 is issued when absences reach this % of total class hours'),
 ('warning2_pct',       '15',  'Warning 2 is issued when absences reach this % of total class hours'),
 ('drop_pct',           '20',  'Dropped when absences reach at least this % of total class hours (CSU Student Manual s.II.C.2.1)'),
 ('late_minutes',       '15',  'Minutes after class start before a student is marked Late'),
 ('tardy_alert_count',  '3',   'Number of lates that triggers a frequent-tardiness (guidance referral) alert'),
 ('lates_per_absence',  '0',   'Convert this many lates into one absence (0 = off; CSU manual has no conversion rule)'),
 ('edit_window_days',   '3',   'Days a teacher may still edit a saved attendance session (Admin: unlimited)');

-- ---------------------------------------------------------------------
-- Views: the single source of truth for attendance status
-- ---------------------------------------------------------------------
-- 1) raw counts and hours per enrolled student per class
CREATE OR REPLACE VIEW v_attendance_counts AS
SELECT
    ce.class_id,
    ce.student_id,
    c.total_hours,
    ROUND(TIME_TO_SEC(TIMEDIFF(c.end_time, c.start_time)) / 3600, 2)                AS session_hours,
    COUNT(ar.record_id)                                                            AS sessions_recorded,
    COALESCE(SUM(ar.status = 'Present'), 0)                                        AS present_count,
    COALESCE(SUM(ar.status = 'Late'), 0)                                           AS late_count,
    COALESCE(SUM(ar.status = 'Absent'), 0)                                         AS absent_count,
    COALESCE(SUM(ar.status = 'Excused'), 0)                                        AS excused_count,
    COALESCE(SUM(CASE
                    WHEN ar.status = 'Absent' THEN ar.hours_absent
                    WHEN ar.status = 'Excused' AND ar.excuse_status = 'Pending' THEN ar.hours_absent
                    ELSE 0 END), 0)                                                AS hours_absent_raw
FROM class_enrollments ce
JOIN classes c                  ON c.class_id = ce.class_id
LEFT JOIN attendance_sessions s ON s.class_id = ce.class_id
LEFT JOIN attendance_records ar ON ar.session_id = s.session_id AND ar.student_id = ce.student_id
GROUP BY ce.class_id, ce.student_id, c.total_hours, c.start_time, c.end_time;

-- 2) apply optional lates-to-absence conversion
CREATE OR REPLACE VIEW v_attendance_hours AS
SELECT
    v.*,
    ROUND(v.hours_absent_raw +
          CASE WHEN CAST((SELECT setting_value FROM settings WHERE setting_key = 'lates_per_absence') AS UNSIGNED) > 0
               THEN FLOOR(v.late_count / CAST((SELECT setting_value FROM settings WHERE setting_key = 'lates_per_absence') AS UNSIGNED)) * v.session_hours
               ELSE 0 END, 2)                                                       AS hours_absent,
    CAST((SELECT setting_value FROM settings WHERE setting_key = 'warning1_pct') AS DECIMAL(5,2)) AS warning1_pct,
    CAST((SELECT setting_value FROM settings WHERE setting_key = 'warning2_pct') AS DECIMAL(5,2)) AS warning2_pct,
    CAST((SELECT setting_value FROM settings WHERE setting_key = 'drop_pct')     AS DECIMAL(5,2)) AS drop_pct
FROM v_attendance_counts v;

-- 3) percent + status + display columns  (use THIS view in the app)
CREATE OR REPLACE VIEW v_attendance_status AS
SELECT
    h.class_id,
    h.student_id,
    st.student_no,
    CONCAT(st.last_name, ', ', st.first_name)                                      AS student_name,
    c.term_id,
    c.teacher_id,
    sub.subject_code,
    sub.description                                                                AS subject_description,
    sec.section_name,
    h.total_hours,
    h.session_hours,
    h.sessions_recorded,
    h.present_count,
    h.late_count,
    h.absent_count,
    h.excused_count,
    h.hours_absent,
    ROUND(h.total_hours * h.drop_pct / 100, 2)                                     AS allowed_hours,
    ROUND(COALESCE(h.hours_absent / NULLIF(h.total_hours, 0) * 100, 0), 2)         AS percent_absent,
    CASE
        WHEN COALESCE(h.hours_absent / NULLIF(h.total_hours, 0) * 100, 0) >= h.drop_pct     THEN 'Dropped'
        WHEN COALESCE(h.hours_absent / NULLIF(h.total_hours, 0) * 100, 0) >= h.warning2_pct THEN 'Warning 2'
        WHEN COALESCE(h.hours_absent / NULLIF(h.total_hours, 0) * 100, 0) >= h.warning1_pct THEN 'Warning 1'
        ELSE 'Good'
    END                                                                            AS attendance_status
FROM v_attendance_hours h
JOIN students st  ON st.student_id  = h.student_id
JOIN classes c    ON c.class_id     = h.class_id
JOIN subjects sub ON sub.subject_id = c.subject_id
JOIN sections sec ON sec.section_id = c.section_id;

-- Default administrator (change the password after first login)
INSERT INTO users (user_id, username, password_hash, role, full_name, is_active, must_change_password) VALUES (1, 'admin', 'PBKDF2$10000$AvzpfYKgu1jJuOl+7KGHVg==$501XhAa9y+0Os6Bm5SS2ebJxdyUUjgO5aoWp7lV5+dE=', 'Admin', 'System Administrator', 1, 1);
