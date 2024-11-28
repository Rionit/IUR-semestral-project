using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IUR_Semestral_Work.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;


namespace IUR_Semestral_Work.Support
{
    public class PushpinContext : DbContext
    {
        public DbSet<PushpinData> PushpinData { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = "pushpin.db"
            };

            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);

            optionsBuilder.UseSqlite(connection);
        }

    }
}
