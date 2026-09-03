using MicroERP.Domin.Entities.Policies;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Persistence.Seed;

public static class LeavePolicySeeder
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LeavePolicy>().HasData(

             new LeavePolicy
             {
                 Id = -1,

                 LeaveType = LeaveType.Annual,

                 MaximumDaysPerYear = 30,

                 DaysPerMonth = 2.5m,

                 AllowNegativeBalance = true,

                 MaxNegativeDays = 10,

                 RequiresBalance = true
             },


             new LeavePolicy
             {
                 Id = -2,
             
                 LeaveType = LeaveType.Sick,
             
                 MaximumDaysPerYear = 22,
             
                 DaysPerMonth = 2,
             
                 AllowNegativeBalance = false,
             
                 MaxNegativeDays = 0,
             
                 RequiresBalance = true
             },
             
             
             new LeavePolicy
             {
                 Id = -3,
             
                 LeaveType = LeaveType.Emergency,
             
                 MaximumDaysPerYear = 10,
             
                 DaysPerMonth = 0,
             
                 AllowNegativeBalance = false,
             
                 MaxNegativeDays = 0,
             
                 RequiresBalance = true
             },
             
             
             new LeavePolicy
             {
                 Id = -4,
             
                 LeaveType = LeaveType.Unpaid,
             
                 MaximumDaysPerYear = 0,
             
                 DaysPerMonth = 0,
             
                 AllowNegativeBalance = false,
             
                 MaxNegativeDays = 0,
             
                 RequiresBalance = false
             },
             
             
             new LeavePolicy
             {
                 Id = -5,
             
                 LeaveType = LeaveType.Maternity,
             
                 MaximumDaysPerYear = 60,
             
                 DaysPerMonth = 0,
             
                 AllowNegativeBalance = false,
             
                 MaxNegativeDays = 0,
             
                 RequiresBalance = false
             },
             
             
             new LeavePolicy
             {
                 Id = -6,
             
                 LeaveType = LeaveType.Paternity,
             
                 MaximumDaysPerYear = 5,
             
                 DaysPerMonth = 0,
             
                 AllowNegativeBalance = false,
             
                 MaxNegativeDays = 0,
             
                 RequiresBalance = false
             },


             new LeavePolicy
             {
                 Id = -7,

                 LeaveType = LeaveType.BereavementFirstDegree,

                 MaximumDaysPerYear = 7,

                 DaysPerMonth = 0,

                 AllowNegativeBalance = false,

                 MaxNegativeDays = 0,

                 RequiresBalance = false
             },


             new LeavePolicy
             {
                Id = -8,
            
                LeaveType = LeaveType.BereavementSecondDegree,
            
                MaximumDaysPerYear = 3,
            
                DaysPerMonth = 0,
            
                AllowNegativeBalance = false,
            
                MaxNegativeDays = 0,
            
                RequiresBalance = false
            },
            
            
            new LeavePolicy
            {
                Id = -9,
            
                LeaveType = LeaveType.Marriage,
            
                MaximumDaysPerYear = 30,
            
                DaysPerMonth = 0,
            
                AllowNegativeBalance = false,
            
                MaxNegativeDays = 0,
            
                RequiresBalance = false
            }

        );
    }
}