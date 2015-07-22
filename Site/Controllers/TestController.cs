using System.Web.Http;
using System.Web.Http.Cors;

namespace Site.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TestController : ApiController
    {
        public int Get()
        {
            return 42;
        }
    }
}