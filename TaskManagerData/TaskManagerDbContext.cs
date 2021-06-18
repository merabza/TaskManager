using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManagerData.Inflection;
using TaskManagerData.Models;

namespace TaskManagerData
{
  public class TaskManagerDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
  {

    public TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options) : base(options)
    {

    }

    public virtual DbSet<Status> Statuses { get; set; }
    public virtual DbSet<Priority> Priorities { get; set; }
    public virtual DbSet<Task> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<Status>(entity =>
      {
        string tableName = nameof(Status).Pluralize();
        entity.HasKey(e => e.SttId);
        entity.ToTable(tableName);
        entity.Property(e => e.SttId).HasColumnName(nameof(Status.SttId).UnCapitalize());
        entity.Property(e => e.SttName).IsRequired().HasColumnName(nameof(Status.SttName).UnCapitalize()).HasMaxLength(50);
        entity.HasIndex(e => e.SttName).HasDatabaseName($"IX_{tableName}_{nameof(Status.SttName)}").IsUnique();

        //entity.HasData(
        //  new() {SttId = 1, SttName = "New"},
        //  new() {SttId = 2, SttName = "InProgress"},
        //  new() {SttId = 3, SttName = "Done"}
        //);

      });

      modelBuilder.Entity<Priority>(entity =>
      {
        string tableName = nameof(Priority).Pluralize();
        entity.HasKey(e => e.PrtId);
        entity.ToTable(tableName);
        entity.Property(e => e.PrtId).HasColumnName(nameof(Priority.PrtId).UnCapitalize());
        entity.Property(e => e.PrtName).IsRequired().HasColumnName(nameof(Priority.PrtName).UnCapitalize()).HasMaxLength(50);
        entity.HasIndex(e => e.PrtName).HasDatabaseName($"IX_{tableName}_{nameof(Priority.PrtName)}").IsUnique();

        //entity.HasData(
        //  new() {PrtId = 1, PrtName = "blocker"},
        //  new() {PrtId = 2, PrtName = "critical"},
        //  new() {PrtId = 3, PrtName = "high"},
        //  new() {PrtId = 4, PrtName = "low"},
        //  new() {PrtId = 5, PrtName = "trivial"}
        //);

      });

      modelBuilder.Entity<Task>(entity =>
      {
        string tableName = nameof(Task).Pluralize();
        entity.HasKey(e => e.TskId);
        entity.ToTable(tableName);
        entity.Property(e => e.TskId).HasColumnName(nameof(Task.TskId).UnCapitalize());
        entity.Property(e => e.TskTitle).IsRequired().HasColumnName(nameof(Task.TskTitle).UnCapitalize())
          .HasMaxLength(50);
        //entity.HasIndex(e => e.TskTitle).HasDatabaseName($"IX_{tableName}_{nameof(Task.TskTitle)}").IsUnique();
        entity.Property(e => e.TskDescription).IsRequired().HasColumnName(nameof(Task.TskDescription).UnCapitalize())
          .HasMaxLength(250);
        entity.Property(e => e.PriorityId).HasColumnName(nameof(Task.PriorityId).UnCapitalize());
        entity.Property(e => e.StatusId).HasColumnName(nameof(Task.StatusId).UnCapitalize());
        entity.HasOne(d => d.PriorityNavigation).WithMany(p => p.Tasks).HasForeignKey(d => d.PriorityId)
          .OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName($"FK_{tableName}_{nameof(Priority).Pluralize()}");
        entity.HasOne(d => d.StatusNavigation).WithMany(p => p.Tasks).HasForeignKey(d => d.StatusId)
          .OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName($"FK_{tableName}_{nameof(Status).Pluralize()}");

        //entity.HasData(
        //  new() {TskId = 1, TskTitle = "ამოცანა 1", TskDescription = "ამოცანა 1-ის აღწერა", PriorityId = 1, StatusId = 2},
        //  new() {TskId = 2, TskTitle = "ამოცანა 2", TskDescription = "ამოცანა 2-ის აღწერა", PriorityId = 3, StatusId = 1},
        //  new() {TskId = 3, TskTitle = "ამოცანა 3", TskDescription = "ამოცანა 3-ის აღწერა", PriorityId = 4, StatusId = 3}
        //);

      });

    }


  }
}
