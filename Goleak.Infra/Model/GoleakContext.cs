using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Goleak.Models
{
    public class GoleakContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, add the following
        // code to the Application_Start method in your Global.asax file.
        // Note: this will destroy and re-create your database with every model change.
        // 
        // System.Data.Entity.Database.SetInitializer(new System.Data.Entity.DropCreateDatabaseIfModelChanges<Goleak.Models.GoleakContext>());

        public DbSet<Goleak.Models.User> User { get; set; }

        public DbSet<Goleak.Models.Leak> Leak { get; set; }

        public DbSet<Goleak.Models.LeakOpinion> LeakOpinion { get; set; }

        public GoleakContext()
            : base("GoleakDBContext")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>()
                .HasMany(p => p.Friends)
                .WithMany()
                .Map(m =>
                {
                    m.MapLeftKey("Id");
                    m.MapRightKey("FriendID");
                    m.ToTable("user_friend");
                });

            modelBuilder.Entity<Leak>()
                        .HasRequired(m => m.UserLeaked)
                        .WithMany(t => t.Leaks)
                        .HasForeignKey(m => m.UserLeakedId)
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<Leak>()
                        .HasRequired(m => m.UserWrote)
                        .WithMany(t => t.LeaksWrote)
                        .HasForeignKey(m => m.UserWroteId)
                        .WillCascadeOnDelete(false);
        }
    }
}