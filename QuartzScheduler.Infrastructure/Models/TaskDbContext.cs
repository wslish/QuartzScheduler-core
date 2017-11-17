using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using QuartzScheduler.Model;

namespace QuartzScheduler.Infrastructure.Models
{
    public class TaskDbContext : DbContext
    {
        public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options)
        {
        }


        public DbSet<Task> Task { get; set; }
        public DbSet<TaskLog> TaskLog { get; set; }
    }
}
