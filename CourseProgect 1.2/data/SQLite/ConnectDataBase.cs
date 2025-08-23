using CourseProgect_1._2.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseProgect_1._2.data.SQLite
{
    internal class ConnectDataBase : DbContext
    {
        public DbSet<Progect> Progects { get; set; } = null!;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=BasisProgect.db");
        }
    }
}
