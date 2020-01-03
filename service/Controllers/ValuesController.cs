using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace authService.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
//        Contexts.UsersDbContext _context { get; }

        public ValuesController() {
//            _context = context;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var vars = Environment.GetEnvironmentVariables();
            var res = new List<string>();
            foreach (DictionaryEntry de in vars)
            {
                res.Add($"{de.Key} --> {de.Value}");
            }
            return res;
//            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            try
            {
//                using (var ctx = _context)
//                {
//                    ctx.Database.EnsureCreated();
//                }
            }
            catch(Exception ex) {
                System.Console.Error.WriteLine(ex);
            }

            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
