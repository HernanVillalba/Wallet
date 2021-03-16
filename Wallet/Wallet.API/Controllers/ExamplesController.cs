using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;

namespace Wallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamplesController : ControllerBase
    {
        private readonly WALLETContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ExamplesController(WALLETContext context, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // JUST AN EXAMPLE !! DELETE IT LATER
        [HttpGet]
        public IEnumerable<Transactions> Get()
        {
            return _context.Transactions.ToList();
        }
        // JUST AN EXAMPLE !! DELETE IT LATER

        //Unit of work example
        //[HttpGet]
        //public async Task<IActionResult> GetUsersAsync()
        //{
        //    var users = await _unitOfWork.Users.GetAllAsync();
        //    return Ok(users);
        //}

        //MAPPER EXAMPLE
        //public void Index()
        //{
        //    Examples example = new example();
        //    ExampleViewModel exampleViewModel = _mapper.Map<ExampleViewModel>(example);
        //}
    }
}
