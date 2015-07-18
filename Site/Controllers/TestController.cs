using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Site.Controllers
{
    [AllowAnonymous]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TestController
    {
        public async Task<IEnumerable<int>> Get()
        {
            return await Task.Factory.StartNew(() => Enumerable.Range(0,4).ToList());
        }
    }
}