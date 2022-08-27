using Convey.CQRS.Queries;
using InBranchNotification.Domain;
using InBranchNotification.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchNotification.Queries.ADLogin.queries
{
    public class LoginWithAdQuery : IQuery<ObjectResponse>
    {

        public LoginWithAdQuery(string userName, string password, string domanin)
        {
            UserName = userName;
            Password = password;
            Domanin = domanin;
        }

        public String UserName { get; set; }
        public String Password { get; set; }
        public string Domanin { get; set; }
    }


}
