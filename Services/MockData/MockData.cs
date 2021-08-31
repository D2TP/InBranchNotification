using InBranchDashboard.Commands.AdUser;
using InBranchDashboard.Domain;
using InBranchDashboard.DTOs;
using InBranchMgt.Commands.AdUser;
using InBranchMgt.Commands.AdUser.Handlers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Services.MockData
{
    public class MockData
    {



        public static List<ADCreateCommandDTO> GetADUserItems()
        {
            var _ADUsers = new List<ADCreateCommandDTO>()
            {





                new ADCreateCommandDTO()
                   {
                         user_name= "markt",
                         first_name= "Mark",
                         last_name= "Twain",
                         role_name= "Support Officer",
                         id= "acc3f96e-d294-4700-952c-07aff64c77be",
                         active= true,
                         entry_date= Convert.ToDateTime("2021-08-23T06:54:08.787"),
                         branch_name= "Branch 1",
                         email= "mark@pragimtech.com"
                   },
                new ADCreateCommandDTO()
                   {
                       user_name= "markt",
                         first_name= "Mark",
                         last_name= "Twain",
                         role_name= "Support Officer",
                         id= "81ba906a-2b02-42cd-9af8-b5ece9e06cbb",
                         active= true,
                         entry_date= Convert.ToDateTime("2021-08-23T06:54:08.787"),
                         branch_name= "Branch 1",
                         email= "mark@pragimtech.com"
                },
                new ADCreateCommandDTO()
                   {
                      user_name= "markt",
                         first_name= "Mark",
                         last_name= "Twain",
                         role_name= "Support Officer",
                         id= "a1b87813-a3ab-487c-9e17-cfafd9eaec99",
                         active= true,
                         entry_date= Convert.ToDateTime("2021-08-23T06:54:08.787"),
                         branch_name= "Branch 1",
                         email= "mark@pragimtech.com"
                },
               new ADCreateCommandDTO()
                   {
                      user_name= "markt",
                         first_name= "Mark",
                         last_name= "Twain",
                         role_name= "Support Officer",
                         id="95c059a0-a8d4-417e-8cce-7d452b8f42b9",
                         active= true,
                         entry_date= Convert.ToDateTime("2021-08-23T06:54:08.787"),
                         branch_name= "Branch 1",
                         email= "mark@pragimtech.com"
                },
                new ADCreateCommandDTO()
                   {
                        user_name= "markt",
                         first_name= "Mark",
                         last_name= "Twain",
                         role_name= "Support Officer",
                         id= "e22e8f2c-295f-4ad3-b5c9-a3dcb0ef4424",
                         active= true,
                         entry_date= Convert.ToDateTime("2021-08-23T06:54:08.787"),
                         branch_name= "Branch 1",
                         email= "mark@pragimtech.com"
                }
            };

            return _ADUsers;
        }
        public static IEnumerable<CreateADUserCommand> GetADUserCommand()
        {

            var _CreateADUserCommand = new List<CreateADUserCommand>()
            {
                new CreateADUserCommand()
                   {
                      Active=true,
                       FirstName = "Tayo",
                       LastName = "Lawal",
                       UserName =  "lawalt",
                       BranchId="07a766ef-99c0-427d-b59e-fc373d12c028",
                       Email="lawat@babaco.com",
                       RoleId="f08c93f6-8ef1-4d7c-b612-8e98de77ab3b"

                   },
                new CreateADUserCommand()
                   {

                    FirstName = "Moses",
                    LastName = "Olaiya",
                    UserName =  "lawalt",
                    BranchId="3504237e-2c20-464d-a02f-c6f0f5a6c0cb",
                     Email="Moses@babaco.com",
                       RoleId="7a315c34-6bfe-4529-beae-73bfed0fc96f"
                },
                new CreateADUserCommand()
                   {

                    FirstName = "Segun",
                    LastName = "Ayo",
                    UserName =  "Ayos",
                     BranchId="07a766ef-99c0-427d-b59e-fc373d12c028",
                       Email="Segun@babaco.com",
                       RoleId="f08c93f6-8ef1-4d7c-b612-8e98de77ab3b"
                },
               new CreateADUserCommand()
                   {

                    FirstName = "Morenike",
                    LastName = "Olukoya",
                    UserName =  "Olukoyam",
                     BranchId="07a766ef-99c0-427d-b59e-fc373d12c028",
                       Email="Morenike@babaco.com",
                       RoleId="f08c93f6-8ef1-4d7c-b612-8e98de77ab3b"
                },
                 new CreateADUserCommand()
                   {

                    FirstName = "Wale",
                    LastName = "Dada",
                   UserName =  "Dadaw"  ,
                     BranchId="3504237e-2c20-464d-a02f-c6f0f5a6c0cb",
                     Email="Wale@babaco.com",
                       RoleId="7a315c34-6bfe-4529-beae-73bfed0fc96f"
                }
            };

            return _CreateADUserCommand;

        }




    }
}
