using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wallet.API.Models;
using Wallet.Data.Repositories.Interfaces;
using Wallet.Data.Models;
using System;
using System.Threading.Tasks;
using System.Linq;

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

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                //tengo que saber el id del user traido desde el login, por ahora lista todas las transacciones
                //asignando su id a mano.
                var user_id = 1;
                string SP = "SP_GetTransactionsUser " + user_id;
                var transactions = _unitOfWork.Transactions.SP_GetTransactionsUser(SP, user_id);
                if (transactions != null) { return Ok(transactions); }
                else { return BadRequest(); }
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
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
                    var id_user = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
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

        [HttpPatch("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id, [FromBody] TransactionModel NewTransaction)
        {
            if (id == null || id <= 0)
            {
                return BadRequest();
            }
            //falta obtener los id de la cuenta en usd y ars del usuario con su id
            int USD_account_id = 1, ARS_account_id = 2;
            var transaction_buscada = _unitOfWork.Transactions.FindTransaction((int)id, USD_account_id, ARS_account_id);
            if (!ModelState.IsValid || transaction_buscada == null) { return BadRequest(); }
            else
            {
                try
                {
                    transaction_buscada.Concept = NewTransaction.Concept;
                    _unitOfWork.Transactions.Update(transaction_buscada);
                    await _unitOfWork.Complete();
                    return Ok();
                }
                catch (Exception ex) { return BadRequest(ex.Message); }
            }
        }
    }
}
