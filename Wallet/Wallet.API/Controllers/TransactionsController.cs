using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wallet.API.Models;
using Wallet.Data.Repositories.Interfaces;
using Wallet.Data.Models;
using System;
using System.Threading.Tasks;

namespace Wallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TransactionsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpPost]
        public IActionResult GetAllTransactions()
        {
            //tengo que saber el id del user traido desde el login, por ahora lista todas las transacciones
            //asignando su id a mano.
            var user_id = 1;
            string SP = "TRANSACTIONS_USER " + user_id;
            var transactions = _unitOfWork.Transactions.SP_ListTransactions(SP, user_id);
            if (transactions != null) { return Ok(transactions); }
            else { return BadRequest(); }
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] TransactionModel NewTransaction)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //el type tiene que ser topup o payment
                    //tengo que saber el id de la account del user en ARS. Por ahora la asigno a mano mas abajo porque la sé.
                    Transactions transaction = _mapper.Map<Transactions>(NewTransaction);
                    transaction.AccountId = 1; //acá
                    _unitOfWork.Transactions.Insert(transaction);
                    await _unitOfWork.Complete();
                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else { return BadRequest(); }
        }

    }
}
