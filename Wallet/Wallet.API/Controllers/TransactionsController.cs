using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wallet.Entities;
using Wallet.Data.Repositories.Interfaces;
using Wallet.Data.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using X.PagedList;
using Wallet.Business.Logic;
using System.Net.Http;
using Wallet.Business.Operations;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Wallet.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly TransactionBusiness tb;
        public TransactionsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            tb = new TransactionBusiness(_unitOfWork, mapper);
        }

        /// <summary>
        /// Listar todas las transacciones ordenadas por fecha descendente y paginadas de a 10
        /// </summary>
        /// <param name="page">Página</param>
        [HttpGet("{page}")]
        public async Task<IActionResult> GetAll(int? page)
        {
            try
            {
                if (page == null || page <= 0) { page = 1; } //asigna la primer página
                int pageNumber = (int)page, pageSize = 10; //10 registros por página

                var user_id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
                var ListDB = await tb.GetAll(user_id);
                IEnumerable<Transactions> transactionsPagined = await ListDB.ToPagedList(pageNumber, pageSize).ToListAsync();

                if (transactionsPagined != null) { return Ok(transactionsPagined); }
                else { return BadRequest("No hya transacciones para mostrar"); }
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }


        /// <summary>
        /// Crear una transacción en pesos
        /// </summary>
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] TransactionCreateModel NewTransaction)
        {
            //por ahora solo se realalizan transacciones en ARS
            if (ModelState.IsValid)
            {
                try
                {
                    var user_id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
                    int ARS_account_id = _unitOfWork.Accounts.GetAccountId(user_id, "ARS");
                    NewTransaction.AccountId = ARS_account_id;

                    await tb.Create(NewTransaction);
                    return Ok();
                }
                catch (Exception ex) { return BadRequest(ex.Message); }
            }
            else { return BadRequest(); }
        }

        /// <summary>
        /// Editar una transacción
        /// </summary>
        /// <param name="id">Id de la transacción</param>
        /// <param name="NewTransaction">Transacción</param>
        /// <returns></returns>
        [HttpPatch("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id, [FromBody] TransactionEditModel NewTransaction)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    if (id == null || id <= 0) { return BadRequest(); }
                    var user_id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
                    string message = await tb.EditTransaction(id, NewTransaction, user_id);
                    return Ok(message);
                }
                catch (Exception ex) { return BadRequest(ex.Message); }
            }
            else { return BadRequest(); }
        }

        /// <summary>
        /// Mostrar los detalles de una transacción en específico por Id
        /// </summary>
        /// <param name="id">Id de la transacción</param>
        /// <returns></returns>
        [HttpGet("Details/{id}")]
        public IActionResult Details(int? id)
        {
            try
            {
                if (id == null || id <= 0) { return BadRequest("Id no válido"); }

                var user_id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
                var transaction = tb.GetDetails(id, user_id);

                if (transaction != null) { return Ok(transaction); }
                else { return BadRequest("No se encontró la transacción"); }

            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        /// <summary>
        /// Filtrar transacción por campos
        /// </summary>
        /// <param name="transaction">Transacción</param>
        /// <returns></returns>
        [HttpPost("Filter")]
        public IActionResult Filter([FromBody] TransactionFilterModel transaction)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user_id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);

                    IEnumerable<Transactions> List = tb.Filter(transaction, user_id);

                    if (List != null) { return Ok(List); }
                    else { return BadRequest("No se encontró la transacción"); }
                }
                catch (Exception ex) { return BadRequest(ex.Message); }
            }
            else { return BadRequest(); }
        }

        /// <summary>
        /// Comprar divisas
        /// </summary>
        /// <param name="tbc">Transacción</param>
        /// <returns></returns>
        /// <remarks>Ingrese si va a comprar o vender, el tipo de divisa y el monto</remarks>
        [HttpPost("BuyCurrency")]
        public async Task<IActionResult> BuyCurrencyAsync([FromBody] TransactionBuyCurrency tbc)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user_id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
                    string message = await tb.BuyCurrency(tbc, user_id);
                    return Ok(message);
                }
                catch (Exception ex) { return BadRequest(ex.Message); }
            }
            else { return BadRequest(); }
        }


        /// <summary>
        /// Transferir dinero de una cuenta propia a otra cuenta existente de la misma moneda
        /// </summary>
        /// <remarks>Ingrese la cuenta de origen, el monto y por último la cuenta de destino</remarks>
        [HttpPost("Transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferModel newTransfer)
        {
            var userId = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
            try
            {
               await tb.Transfer(newTransfer, userId);
               return Ok("Transferencia realizada");
               
            }
            catch 
            {
                throw;
            }
        }
    }
}

