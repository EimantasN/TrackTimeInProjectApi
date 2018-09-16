﻿// <auto-generated />
using System;
using Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Library.Migrations
{
    [DbContext(typeof(DbContextApi))]
    [Migration("20180915202817_initial")]
    partial class initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Library.Models.Error", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Count");

                    b.Property<DateTime>("Created");

                    b.Property<string>("InnerMessage");

                    b.Property<DateTime>("LastAccrued");

                    b.Property<string>("Message");

                    b.Property<string>("MethodName");

                    b.Property<string>("Parameters");

                    b.HasKey("Id");

                    b.ToTable("Errors");
                });

            modelBuilder.Entity("Library.Models.Members", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active");

                    b.Property<long>("CurrentDayTime");

                    b.Property<string>("Email");

                    b.Property<string>("ImgUrl");

                    b.Property<string>("LastName");

                    b.Property<string>("Name");

                    b.Property<string>("Password");

                    b.Property<long>("TotalTime");

                    b.HasKey("Id");

                    b.ToTable("Members");
                });

            modelBuilder.Entity("Library.Models.Time", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("Diff");

                    b.Property<DateTime>("End");

                    b.Property<int?>("MembersId");

                    b.Property<DateTime>("Start");

                    b.HasKey("Id");

                    b.HasIndex("MembersId");

                    b.ToTable("Times");
                });

            modelBuilder.Entity("Library.Models.WorkTask", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("MembersId");

                    b.Property<string>("Name");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.HasIndex("MembersId");

                    b.ToTable("Task");
                });

            modelBuilder.Entity("Library.Models.Time", b =>
                {
                    b.HasOne("Library.Models.Members")
                        .WithMany("Times")
                        .HasForeignKey("MembersId");
                });

            modelBuilder.Entity("Library.Models.WorkTask", b =>
                {
                    b.HasOne("Library.Models.Members")
                        .WithMany("Member_Tasks")
                        .HasForeignKey("MembersId");
                });
#pragma warning restore 612, 618
        }
    }
}
