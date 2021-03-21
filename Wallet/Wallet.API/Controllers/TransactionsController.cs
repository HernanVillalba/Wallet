using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wallet.API.Models;
using Wallet.Data.Repositories.Interfaces;
using Wallet.Data.ModelsAPI;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using X.PagedList;

namespace Wallet.API.Controllers
{
    [Authorize]
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

        [HttpGet("{page}")]
        public async Task<IActionResult> GetAll(int? page)
        {
            try
            {
                if (page == null || page <= 0) { page = 1; }
                int pageNumber = (int)page, pageSize = 10;
                var user_id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
                int ARS_id = _unitOfWork.Accounts.GetAccountId(user_id, "ARS"), USD_id = _unitOfWork.Accounts.GetAccountId(user_id, "USD");
                IEnumerable<Transactions> listDB = await _unitOfWork.Transactions.GetTransactionsUser(ARS_id, USD_id);
                var TransactionsPagined = listDB.ToPagedList(pageNumber, pageSize);

                if (TransactionsPagined != null) { return Ok(TransactionsPagined); }
                else { return BadRequest("No hay transacciones para mostrar"); }
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] TransactionModel NewTransaction)
        {
            //por ahora solo se realalizan transacciones en ARS
            if (ModelState.IsValid)
            {
                try
                {
                    var user_id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
                    int ARS_account_id = _unitOfWork.Accounts.GetAccountId(user_id, "ARS");

                    Transactions transaction = _mapper.Map<Transactions>(NewTransaction);
                    transaction.AccountId = ARS_account_id;

                    _unitOfWork.Transactions.Insert(transaction);
                    await _unitOfWork.Complete();
                    return Ok();
                }
                catch (Exception ex){return BadRequest(ex.Message);}
            }
            else { return BadRequest(); }
        }

        [HttpPatch("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id, [FromBody] TransactionEditModel NewTransaction)
        {
            if (id == null || id <= 0) { return BadRequest(); }

            var user_id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
            int USD_account_id = _unitOfWork.Accounts.GetAccountId(user_id, "USD");
            int ARS_account_id = _unitOfWork.Accounts.GetAccountId(user_id, "ARS");
            var transaction_buscada = _unitOfWork.Transactions.FindTransaction((int)id, USD_account_id, ARS_account_id);

            if (ModelState.IsValid && transaction_buscada != null)
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
            else { return BadRequest(); }
        }

        [HttpGet("Details/{id}")]
        public IActionResult Details(int? id)
        {
            if (id == null || id <= 0) { return BadRequest("Id no válido"); }

            var user_id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);

            int? ARS_account_id = _unitOfWork.Accounts.GetAccountId(user_id, "ARS");
            int? USD_account_id = _unitOfWork.Accounts.GetAccountId(user_id, "USD");

            if (ARS_account_id != null && USD_account_id != null)
            {
                var transaction = _unitOfWork.Transactions
                    .FindTransaction((int)id, (int)USD_account_id, (int)ARS_account_id);

                if (transaction != null) { return Ok(transaction); }
                else { return BadRequest("No se encontró la transacción"); }

            }
            else { return BadRequest("No se encontraron las cuentas del usuario"); }
        }

        [HttpPost("Filter")]
        public IActionResult Filter([FromBody] TransactionSearchModel transaction)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user_id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);

                    int ARS_account_id = _unitOfWork.Accounts.GetAccountId(user_id, "ARS");
                    int USD_account_id = _unitOfWork.Accounts.GetAccountId(user_id, "USD");

                    //si el id de account es null o menor a 0 se asume que busca en pesos
                    if (transaction.AccountId == null || transaction.AccountId <= 0 ||
                        (transaction.AccountId != ARS_account_id && transaction.AccountId != USD_account_id)) //si el id de la account ingresado es distinta a alguna de la suyas, se asume que busca en pesos
                    { transaction.AccountId = ARS_account_id; }

                    Transactions transactionDB = _mapper.Map<Transactions>(transaction);
                    IEnumerable<Transactions> List = _unitOfWork.Transactions.FilterTransaction(transactionDB);

                    if (List != null) { return Ok(List); }
                    else { return BadRequest("No se encontró la transacción"); }
                }
                catch (Exception ex) { return BadRequest(ex.Message); }
            }
            else { return BadRequest(); }
        }
    }
}
