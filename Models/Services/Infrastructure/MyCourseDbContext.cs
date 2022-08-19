using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MyCourse.Models.Entities;

namespace MyCourse.Models.Services.Infrastructure
{
    public partial class MyCourseDbContext : DbContext
    {
        public MyCourseDbContext(DbContextOptions<MyCourseDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Course> Courses { get; set; } = null!;
        public virtual DbSet<Lesson> Lessons { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("Courses");
                entity.HasKey(course => course.Id); //Superfluo se la proprietà si chiama Id oppure CourseId
                //entity.HasKey(course => new {course.Id, course.Author }); caso in cui si ha più di una primary key


                //Mapping per gli owned type
                entity.OwnsOne(course => course.CurrentyPrice, builder =>{
                    builder.Property(money => money.Currency)
                    .HasConversion<string>()
                    .HasColumnName("CurrentPrice_Currency"); //questo è superfluo perchè le nostre colonne seguono già la convenzione dei nomi
                    
                    builder.Property(money => money.Amount)
                     .HasConversion<float>()
                     .HasColumnName("CurrentPrice_Amount"); //questo è superfluo perchè le nostre colonne seguono già la convenzione dei nomi
                });

                entity.OwnsOne(course => course.FullPrice, builder =>{
                    builder.Property(money => money.Currency).HasConversion<string>();
                });

                //Mapping per le relazioni
                entity.HasMany(course => course.Lessons)
                      .WithOne(lesson => lesson.Course)
                      .HasForeignKey(lesson => lesson.CourseId); //Superflua se la proprietà si chiama CourseId

                #region Mapping generato automaticamente dal tool di reverse engineering
                /*
                entity.Property(e => e.Author).HasColumnType("TEXT (100)");

                entity.Property(e => e.CurrentyPriceAmount)
                    .HasColumnType("NUMERIC")
                    .HasColumnName("CurrentyPrice_Amount")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.CurrentyPriceCurrency)
                    .HasColumnType("TEXT (3)")
                    .HasColumnName("CurrentyPrice_Currency")
                    .HasDefaultValueSql("'EUR'");

                entity.Property(e => e.Description).HasColumnType("TEXT (10000)");

                entity.Property(e => e.Email).HasColumnType("TEXT (100)");

                entity.Property(e => e.FullPriceAmount)
                    .HasColumnType("NUMERIC")
                    .HasColumnName("FullPrice_Amount")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.FullPriceCurrency)
                    .HasColumnType("TEXT (3)")
                    .HasColumnName("FullPrice_Currency")
                    .HasDefaultValueSql("'EUR'");

                entity.Property(e => e.ImagePath).HasColumnType("TEXT (100)");

                entity.Property(e => e.Title).HasColumnType("TEXT (100)");
                */
                #endregion
            });

            modelBuilder.Entity<Lesson>(entity =>
            {
                entity.ToTable("Lessons");
                
                entity.HasOne(lesson => lesson.Course)
                      .WithMany(course => course.Lessons);

                #region Mapping generato automaticamente dal tool di reverse engineering
                /*
                entity.Property(e => e.Description).HasColumnType("TEXT (1000)");

                entity.Property(e => e.Duration)
                    .HasColumnType("TEXT (8)")
                    .HasDefaultValueSql("'00:00:00:'");

                entity.Property(e => e.Title).HasColumnType("TEXT (100)");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Lessons)
                    .HasForeignKey(d => d.CourseId);
                */
                #endregion
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
