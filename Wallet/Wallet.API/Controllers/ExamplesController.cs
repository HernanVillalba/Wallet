using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wallet.Data.Models;

namespace Wallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamplesController : ControllerBase
    {
        private readonly WALLETContext _context;

        public ExamplesController(WALLETContext context)
        {
            _context = context;
        }

        // JUST AN EXAMPLE !! DELETE IT LATER
        [HttpGet]
        public IEnumerable<Transactions> Get()
        {
            return _context.Transactions.ToList();
        }
        // JUST AN EXAMPLE !! DELETE IT LATER

    }
}
