-- =====================================================================
-- Revision: Late / Absent / Excused need a reason, with an optional attachment.
-- Run once on an existing csu_attendance database (fresh imports of
-- csu_attendance.sql / csu_attendance_clean.sql already include this).
--   C:\xampp\mysql\bin\mysql.exe -u root csu_attendance < database\migrate_reasons_attachments.sql
-- =====================================================================
USE csu_attendance;

-- File name of the proof (excuse letter, medical certificate...) stored in the app's "attachments" folder
ALTER TABLE attendance_records
    ADD COLUMN IF NOT EXISTS attachment_path VARCHAR(255) NULL AFTER remarks;

-- Demo data: give every existing Late / Absent / Excused mark a reason
UPDATE attendance_records
SET remarks = ELT(1 + record_id % 5, 'Heavy traffic on the way to campus', 'Tricycle/jeepney delay',
                  'Came from a class across campus', 'Heavy rain', 'Overslept')
WHERE status = 'Late' AND (remarks IS NULL OR remarks = '');

UPDATE attendance_records
SET remarks = ELT(1 + record_id % 5, 'Sick (fever)', 'Family emergency', 'Flooded road, no transportation',
                  'Medical check-up', 'Did not inform the instructor')
WHERE status = 'Absent' AND (remarks IS NULL OR remarks = '');

UPDATE attendance_records
SET remarks = ELT(1 + record_id % 3, 'Represented CSU in a provincial sports meet', 'Attended a university seminar',
                  'Hospitalized (medical certificate submitted)')
WHERE status = 'Excused' AND (remarks IS NULL OR remarks = '');
