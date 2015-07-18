using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Repository.DataServices;
using Repository.Models;

namespace Site.Controllers
{
    [AllowAnonymous]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TestController : ApiController
    {
        private readonly IUserService service;

        public TestController(IUserService service)
        {
            this.service = service;
        }

        public async Task<User> Get()
        {
            return await Task.Factory.StartNew(() => service.GetUserByLogin("foo"));
        }
    }
}