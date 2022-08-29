using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCourse.Migrations
{
    public partial class CourseAuthor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"PRAGMA foreign_keys = 0;

            CREATE TABLE sqlitestudio_temp_table AS SELECT *
                                                      FROM Courses;

            DROP TABLE Courses;

            CREATE TABLE Courses (
                Id                    INTEGER  NOT NULL
                                               CONSTRAINT PK_Courses PRIMARY KEY,
                Author                TEXT     NOT NULL,
                CurrentPrice_Amount   REAL     NOT NULL,
                CurrentPrice_Currency TEXT     NOT NULL,
                Description           TEXT     NOT NULL,
                Email                 TEXT     NOT NULL,
                FullPrice_Amount      REAL     NOT NULL,
                FullPrice_Currency    TEXT     NOT NULL,
                ImagePath             TEXT     NOT NULL,
                Rating                REAL     NOT NULL,
                Title                 TEXT     NOT NULL,
                RowVersion            DATETIME,
                Status                TEXT     NOT NULL
                                               DEFAULT 'Draft',
                AuthorId              TEXT     REFERENCES AspNetUsers (Id) 
            );

            INSERT INTO Courses (
                                    Id,
                                    Author,
                                    CurrentPrice_Amount,
                                    CurrentPrice_Currency,
                                    Description,
                                    Email,
                                    FullPrice_Amount,
                                    FullPrice_Currency,
                                    ImagePath,
                                    Rating,
                                    Title,
                                    RowVersion,
                                    Status
                                )
                                SELECT Id,
                                       Author,
                                       CurrentPrice_Amount,
                                       CurrentPrice_Currency,
                                       Description,
                                       Email,
                                       FullPrice_Amount,
                                       FullPrice_Currency,
                                       ImagePath,
                                       Rating,
                                       Title,
                                       RowVersion,
                                       Status
                                  FROM sqlitestudio_temp_table;

            DROP TABLE sqlitestudio_temp_table;

            CREATE UNIQUE INDEX IX_Courses_Title ON Courses (
                'Title'
            );

            CREATE UNIQUE INDEX ux_title ON Courses (
                Title COLLATE NOCASE
            );

            CREATE TRIGGER CoursesSetRowVersionOnInsert
                     AFTER INSERT
                        ON Courses
            BEGIN
                UPDATE Courses
                   SET RowVersion = CURRENT_TIMESTAMP
                 WHERE Id = NEW.Id;
            END;

            CREATE TRIGGER CoursesSetRowVersionOnUpdate
                     AFTER UPDATE
                        ON Courses
                      WHEN NEW.RowVersion <= OLD.RowVersion
            BEGIN
                UPDATE Courses
                   SET RowVersion = CURRENT_TIMESTAMP
                 WHERE Id = NEW.Id;
            END;

            PRAGMA foreign_keys = 1;",
            suppressTransaction: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"PRAGMA foreign_keys = 0;
            CREATE TABLE sqlitestudio_temp_table AS SELECT *
                                                      FROM Courses;
            DROP TABLE Courses;
            CREATE TABLE Courses (
                Id                    INTEGER NOT NULL
                                              CONSTRAINT PK_Courses PRIMARY KEY AUTOINCREMENT,
                Title                 TEXT,
                Description           TEXT,
                ImagePath             TEXT,
                Author                TEXT,
                Email                 TEXT,
                Rating                REAL    NOT NULL,
                FullPrice_Amount      REAL,
                FullPrice_Currency    TEXT,
                CurrentPrice_Amount   REAL,
                CurrentPrice_Currency TEXT,
                RowVersion            TEXT,
                Status                TEXT    NOT NULL
                                              DEFAULT 'Deleted'
            );
            INSERT INTO Courses (
                                    Id,
                                    Title,
                                    Description,
                                    ImagePath,
                                    Author,
                                    Email,
                                    Rating,
                                    FullPrice_Amount,
                                    FullPrice_Currency,
                                    CurrentPrice_Amount,
                                    CurrentPrice_Currency,
                                    RowVersion,
                                    Status
                                )
                                SELECT Id,
                                       Title,
                                       Description,
                                       ImagePath,
                                       Author,
                                       Email,
                                       Rating,
                                       FullPrice_Amount,
                                       FullPrice_Currency,
                                       CurrentPrice_Amount,
                                       CurrentPrice_Currency,
                                       RowVersion,
                                       Status
                                  FROM sqlitestudio_temp_table;
            DROP TABLE sqlitestudio_temp_table;
            CREATE UNIQUE INDEX IX_Courses_Title ON Courses (
                'Title'
            );
            CREATE TRIGGER CoursesSetRowVersionOnInsert
                     AFTER INSERT
                        ON Courses
            BEGIN
                UPDATE Courses
                   SET RowVersion = CURRENT_TIMESTAMP
                 WHERE Id = NEW.Id;
            END;
            CREATE TRIGGER CoursesSetRowVersionOnUpdate
                     AFTER UPDATE
                        ON Courses
                      WHEN NEW.RowVersion <= OLD.RowVersion
            BEGIN
                UPDATE Courses
                   SET RowVersion = CURRENT_TIMESTAMP
                 WHERE Id = NEW.Id;
            END;
            PRAGMA foreign_keys = 1;
            ", 
            suppressTransaction: true);
        }
    }
}
