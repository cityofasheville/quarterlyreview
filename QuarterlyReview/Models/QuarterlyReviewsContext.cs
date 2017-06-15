using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace QuarterlyReview.Models
{
    public partial class QuarterlyReviewsContext : DbContext
    {
        public virtual DbSet<DeptToDivMapping> DeptToDivMapping { get; set; }
        public virtual DbSet<Employees> Employees { get; set; }
        public virtual DbSet<QuestionTemplate> QuestionTemplate { get; set; }
        public virtual DbSet<QuestionTypeList> QuestionTypeList { get; set; }
        public virtual DbSet<Questions> Questions { get; set; }
        public virtual DbSet<Responses> Responses { get; set; }
        public virtual DbSet<ReviewStatusList> ReviewStatusList { get; set; }
        public virtual DbSet<ReviewTemplate> ReviewTemplate { get; set; }
        public virtual DbSet<Reviews> Reviews { get; set; }

        public QuarterlyReviewsContext(DbContextOptions<QuarterlyReviewsContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DeptToDivMapping>(entity =>
            {
                entity.HasKey(e => e.DivId)
                    .HasName("PK_DeptToDivMapping");

                entity.Property(e => e.DivId)
                    .HasColumnName("DivID")
                    .HasColumnType("varchar(4)");

                entity.Property(e => e.Department).HasColumnType("varchar(50)");

                entity.Property(e => e.DeptId)
                    .HasColumnName("DeptID")
                    .HasColumnType("varchar(2)");

                entity.Property(e => e.Division).HasColumnType("varchar(50)");
            });

            modelBuilder.Entity<Employees>(entity =>
            {
                entity.HasKey(e => e.EmpId)
                    .HasName("PK_Employees");

                entity.Property(e => e.EmpId)
                    .HasColumnName("EmpID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasColumnType("varchar(1)");

                entity.Property(e => e.Department).HasColumnType("varchar(50)");

                entity.Property(e => e.DeptId)
                    .HasColumnName("DeptID")
                    .HasColumnType("varchar(2)");

                entity.Property(e => e.DivId)
                    .HasColumnName("DivID")
                    .HasColumnType("varchar(4)");

                entity.Property(e => e.Division).HasColumnType("varchar(50)");

                entity.Property(e => e.EmpEmail)
                    .HasColumnName("Emp_Email")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Employee).HasColumnType("varchar(67)");

                entity.Property(e => e.Position).HasColumnType("varchar(20)");

                entity.Property(e => e.SupEmail)
                    .HasColumnName("Sup_Email")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.SupId).HasColumnName("SupID");

                entity.Property(e => e.Supervisor).HasColumnType("varchar(67)");

                entity.HasOne(d => d.Div)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.DivId)
                    .HasConstraintName("FK_Employees_DeptToDivMapping");
            });

            modelBuilder.Entity<QuestionTemplate>(entity =>
            {
                entity.HasKey(e => e.QtId)
                    .HasName("PK_QuestionTemplate");

                entity.HasIndex(e => e.QType)
                    .HasName("IX_QuestionType");

                entity.HasIndex(e => e.RtId)
                    .HasName("IX_QuestionTemplate");

                entity.Property(e => e.QtId).HasColumnName("QT_ID");

                entity.Property(e => e.QOrder).HasColumnName("Q_Order");

                entity.Property(e => e.QType)
                    .IsRequired()
                    .HasColumnName("Q_Type")
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.QuestionText)
                    .IsRequired()
                    .HasColumnName("Question Text")
                    .HasColumnType("varchar(max)");

                entity.Property(e => e.RtId).HasColumnName("RT_ID");

                entity.HasOne(d => d.QTypeNavigation)
                    .WithMany(p => p.QuestionTemplate)
                    .HasForeignKey(d => d.QType)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_QuestionTemplate_QuestionTypeList");

                entity.HasOne(d => d.Rt)
                    .WithMany(p => p.QuestionTemplate)
                    .HasForeignKey(d => d.RtId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_QuestionTemplate_ReviewTemplate");
            });

            modelBuilder.Entity<QuestionTypeList>(entity =>
            {
                entity.HasKey(e => e.QuestionType)
                    .HasName("IX_QuestionTypes");

                entity.HasIndex(e => new { e.QtypeOrder, e.QuestionType })
                    .HasName("IX_Order_Type")
                    .IsUnique();

                entity.Property(e => e.QuestionType)
                    .HasColumnName("Question Type")
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.QtypeOrder)
                    .HasColumnName("QType_Order")
                    .HasDefaultValueSql("99");
            });

            modelBuilder.Entity<Questions>(entity =>
            {
                entity.HasKey(e => e.QId)
                    .HasName("PK_Questions");

                entity.HasIndex(e => e.QtId)
                    .HasName("IX_QuestionQTID");

                entity.HasIndex(e => e.QtType)
                    .HasName("IX_QuestionQType");

                entity.HasIndex(e => e.RId)
                    .HasName("IX_QuestionRID");

                entity.HasIndex(e => e.RtId)
                    .HasName("IX_QuestionRTID");

                entity.Property(e => e.QId).HasColumnName("Q_ID");

                entity.Property(e => e.Answer).HasColumnType("varchar(max)");

                entity.Property(e => e.QtId).HasColumnName("QT_ID");

                entity.Property(e => e.QtOrder).HasColumnName("QT_Order");

                entity.Property(e => e.QtQuestion)
                    .IsRequired()
                    .HasColumnName("QT_Question")
                    .HasColumnType("varchar(max)");

                entity.Property(e => e.QtType)
                    .IsRequired()
                    .HasColumnName("QT_Type")
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.RId).HasColumnName("R_ID");

                entity.Property(e => e.RtId).HasColumnName("RT_ID");

                entity.HasOne(d => d.Qt)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.QtId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Questions_QuestionTemplate");

                entity.HasOne(d => d.QtTypeNavigation)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.QtType)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Questions_QuestionTypeList");

                entity.HasOne(d => d.R)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.RId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Questions_Reviews");
            });

            modelBuilder.Entity<Responses>(entity =>
            {
                entity.HasKey(e => e.Respid)
                    .HasName("PK_Responses");

                entity.Property(e => e.Respid).HasColumnName("RESPID");

                entity.Property(e => e.QId).HasColumnName("Q_ID");

                entity.Property(e => e.RId).HasColumnName("R_ID");

                entity.Property(e => e.Response).HasColumnType("varchar(max)");

                entity.Property(e => e.ResponseDate)
                    .HasColumnName("Response Date")
                    .HasColumnType("date")
                    .HasDefaultValueSql("getdate()");

                entity.HasOne(d => d.R)
                    .WithMany(p => p.Responses)
                    .HasForeignKey(d => d.RId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Responses_Reviews");
            });

            modelBuilder.Entity<ReviewStatusList>(entity =>
            {
                entity.HasKey(e => e.ReviewStatus)
                    .HasName("IX_ReviewStatusList");

                entity.HasIndex(e => new { e.RstatusOrder, e.ReviewStatus })
                    .HasName("IX_Order_Status");

                entity.Property(e => e.ReviewStatus)
                    .HasColumnName("Review Status")
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.RstatusOrder)
                    .HasColumnName("RStatus_Order")
                    .HasDefaultValueSql("99");
            });

            modelBuilder.Entity<ReviewTemplate>(entity =>
            {
                entity.HasKey(e => e.RtId)
                    .HasName("PK_ReviewTemplate");

                entity.Property(e => e.RtId).HasColumnName("RT_ID");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("varchar(max)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.ValidFrom)
                    .HasColumnName("Valid From")
                    .HasColumnType("date")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.ValidTo)
                    .HasColumnName("Valid To")
                    .HasColumnType("date")
                    .HasDefaultValueSql("'9999-12-31'");
            });

            modelBuilder.Entity<Reviews>(entity =>
            {
                entity.HasKey(e => e.RId)
                    .HasName("PK_Reviews");

                entity.HasIndex(e => e.DivId)
                    .HasName("IX_ReviewDiv");

                entity.HasIndex(e => e.EmpId)
                    .HasName("IX_ReviewEmp");

                entity.HasIndex(e => e.RtId)
                    .HasName("IX_ReviewTemplate");

                entity.HasIndex(e => e.Status)
                    .HasName("IX_ReviewStatus");

                entity.HasIndex(e => e.SupId)
                    .HasName("IX_ReviewSup");

                entity.Property(e => e.RId).HasColumnName("R_ID");

                entity.Property(e => e.DivId)
                    .IsRequired()
                    .HasColumnName("DivID")
                    .HasColumnType("varchar(4)");

                entity.Property(e => e.EmpId).HasColumnName("EmpID");

                entity.Property(e => e.PeriodEnd)
                    .HasColumnName("Period_End")
                    .HasColumnType("date");

                entity.Property(e => e.PeriodStart)
                    .HasColumnName("Period_Start")
                    .HasColumnType("date");

                entity.Property(e => e.Position)
                    .IsRequired()
                    .HasColumnType("varchar(25)");

                entity.Property(e => e.RtDesc)
                    .IsRequired()
                    .HasColumnName("RT_Desc")
                    .HasColumnType("varchar(max)");

                entity.Property(e => e.RtId).HasColumnName("RT_ID");

                entity.Property(e => e.RtName)
                    .IsRequired()
                    .HasColumnName("RT_Name")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnType("varchar(25)")
                    .HasDefaultValueSql("'Ready'");

                entity.Property(e => e.StatusDate)
                    .HasColumnName("Status_Date")
                    .HasColumnType("date")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.SupId).HasColumnName("SupID");

                entity.HasOne(d => d.Div)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.DivId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Reviews_DeptToDivMapping");

                entity.HasOne(d => d.Emp)
                    .WithMany(p => p.ReviewsEmp)
                    .HasForeignKey(d => d.EmpId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Reviews_Employees");

                entity.HasOne(d => d.Rt)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.RtId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Reviews_ReviewTemplate");

                entity.HasOne(d => d.StatusNavigation)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.Status)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Reviews_ReviewStatusList");

                entity.HasOne(d => d.Sup)
                    .WithMany(p => p.ReviewsSup)
                    .HasForeignKey(d => d.SupId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Reviews_Supervisors");
            });
        }
    }
}