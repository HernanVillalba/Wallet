using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class TransactionsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public TransactionsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            //tengo que saber el id de la account del user, por ahora lista todas las transacciones 
            //int id = account.id
            //var ListDB = await _unitOfWork.Transactions.FromSqlRaw($"EXEC TRANSACTIONS_USER {1}").ToListAsync();
            var ListDB = await _unitOfWork.Transactions.GetAllAsync();
            if (ListDB != null) { return Ok(ListDB); }
            else { return BadRequest(); }
        }

    }
}
