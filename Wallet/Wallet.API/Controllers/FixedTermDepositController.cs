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
    public class FixedTermDepositController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public FixedTermDepositController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                // Get UserId (you can hardcode it to be 1, 2 or whatever to test the function)
                var userId = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);

                // Load and execute the respective stored procedure to retrieve all the
                // fixed term deposits of the two user's accounts
                string storedProcedure = "SP_GetUserFixedTermDeposit " + userId;
                var fixedDeposits = _unitOfWork.FixedTermDeposits.ExecuteStoredProcedure(storedProcedure).ToList();

                // Check if the response was valid, otherwise throw Error 400
                if(fixedDeposits == null)
                {
                    return BadRequest();
                }
                return Ok(fixedDeposits);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
