using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbFactory
{
    public class SettingModel
    {
        public string settingId { get; set; } //{\"settingId\":\"CHANNEL_AUTH_CMD_TIME_OUT\",\"fieldValue\":\"60\"
        public string fieldValue { get; set; }
    }
}
