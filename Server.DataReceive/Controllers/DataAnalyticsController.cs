using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.Core.RunTime;

namespace Server.DataReceive.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataAnalyticsController : ControllerBase
    {
        IMonitorOnlineModule m_monitorOnlineModule;
        public DataAnalyticsController(IMonitorOnlineModule monitorOnlineModule)
        {
            m_monitorOnlineModule = monitorOnlineModule;
        }
        [HttpGet]
        public ActionResult<string> Get()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(new object());
        }
    }
}
