using recruitlab.server.Data;
using Server.Model.Entities;

namespace Server.Data
{
    public class DataSeedHelper
    {
        private readonly AppDbContext dbContext;
        public DataSeedHelper(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void InsertData()
        {
            if (!dbContext.Roles.Any())
            {
                dbContext.Roles.Add(new Role { Name = "Admin" });
                dbContext.Roles.Add(new Role { Name = "HR" });
                dbContext.Roles.Add(new Role { Name = "Reviewer" });
                dbContext.Roles.Add(new Role { Name = "Interviewer" });
                dbContext.Roles.Add(new Role { Name = "Candidate" });
                dbContext.Roles.Add(new Role { Name = "Viewer" });
            }
            dbContext.SaveChanges();

            if (!dbContext.Users.Any())
            {
                dbContext.Users.Add(new User
                {
                    Email = "admin@123.com",
                    RoleId = 1,
                    Password = "1234"
                });
            }
            dbContext.SaveChanges();
        }
    }
}
