Build started...
Build succeeded.
The Entity Framework tools version '6.0.7' is older than that of the runtime '6.0.8'. Update the tools for the latest features and bug fixes. See https://aka.ms/AAc1fbw for more information.
info: Microsoft.EntityFrameworkCore.Infrastructure[10403]
      Entity Framework Core 6.0.8 initialized 'MyCourseDbContext' using provider 'Microsoft.EntityFrameworkCore.Sqlite:6.0.7' with options: MaxPoolSize=1024 
BEGIN TRANSACTION;

CREATE UNIQUE INDEX "IX_Courses_Title" ON "Courses" ("Title");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20220823102958_UniqueCourseTitle', '6.0.8');

COMMIT;

BEGIN TRANSACTION;

CREATE TRIGGER CoursesSetRowVersionOnInsert
                                   AFTER INSERT ON Courses
                                   BEGIN
                                   UPDATE Courses SET RowVersion = CURRENT_TIMESTAMP WHERE Id=NEW.Id;
                                   END;

CREATE TRIGGER CoursesSetRowVersionOnUpdate
                                   AFTER UPDATE ON Courses WHEN NEW.RowVersion <= OLD.RowVersion
                                   BEGIN
                                   UPDATE Courses SET RowVersion = CURRENT_TIMESTAMP WHERE Id=NEW.Id;
                                   END;

UPDATE Courses SET RowVersion = CURRENT_TIMESTAMP;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20220823103152_TriggersCourseVersion', '6.0.8');

COMMIT;


