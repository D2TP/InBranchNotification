using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Convey.Test.Accounts.Domain
{
    public class JwtConfig
    {
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public TimeSpan TokenLifeTime { get; set; }
        public int RefreshTokenExpiryInDays { get; set; }
    }
}
