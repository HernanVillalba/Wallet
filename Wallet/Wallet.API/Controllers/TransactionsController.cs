﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Wallet.Business;
using Wallet.Business.Logic;
using Wallet.Entities;

namespace Wallet.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionBusiness _transactionBusiness;
        public TransactionsController(ITransactionBusiness transactionBusiness)
        {
            _transactionBusiness = transactionBusiness;
        }

        /// <summary>
        /// Listar todas las transacciones ordenadas por fecha descendente y paginadas de a 10
        /// </summary>
        /// <param name="page">Página</param>
        /// <param name="transactionFilterModel">Transacción</param>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page, [FromQuery] TransactionFilterModel transactionFilterModel)
        {
            try
            {
                //this is commented until the user can log in
                var user_id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
                var ListDB = await _transactionBusiness.GetAll(transactionFilterModel, user_id, page);
                return StatusCode(200, ListDB);
            }
            catch { throw; }
        }

        /// <summary>
        /// Mostrar los detalles de una transacción en específico por Id
        /// </summary>
        /// <param name="id">Id de la transacción</param>
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null || id <= 0) { throw new CustomException(400, "Id de la transacción no válido"); }
                var user_id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
                var transaction = await _transactionBusiness.Details(id, user_id);
                return StatusCode(200, transaction);
            }
            catch { throw; }
        }

        /// <summary>
        /// Crear una transacción en pesos
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TransactionCreateModel newT)
        {
            //solo transacciones en ARS
            try
            {
                var user_id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
                await _transactionBusiness.Create(newT, user_id);
                return StatusCode(201);
            }
            catch (Exception) { throw; }

        }

        /// <summary>
        /// Comprar divisas
        /// </summary>
        /// <param name="tbc">Transacción</param>
        /// <returns></returns>
        /// <remarks>Ingrese si va a comprar o vender, el tipo de divisa y el monto</remarks>
        [HttpPost]
        [Route("BuyCurrencies")]
        public async Task<IActionResult> BuyCurrencyAsync([FromBody] TransactionBuyCurrency tbc)
        {
            try
            {
                var user_id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
                await _transactionBusiness.BuyCurrency(tbc, user_id);
                return StatusCode(201);
            }
            catch { throw; }
        }

        /// <summary>
        /// Transferir dinero de una cuenta propia a otra cuenta existente de la misma moneda
        /// </summary>
        /// <remarks>Ingrese la cuenta de origen, el monto y por último la cuenta de destino</remarks>
        [HttpPost("Transfer")]
        public async Task<IActionResult> TransferAsync([FromBody] TransferModel newTransfer)
        {
            try
            {
                var userId = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
                await _transactionBusiness.Transfer(newTransfer, userId);
                return StatusCode(201);
            }
            catch { throw; }
        }

        /// <summary>
        /// Editar una transacción
        /// </summary>
        /// <param name="id">Id de la transacción</param>
        /// <param name="NewTransaction">Transacción</param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        public async Task<IActionResult> Edit(int? id, [FromBody] TransactionEditModel NewTransaction)
        {
            try
            {
                if (id == null || id <= 0) { throw new CustomException(400, "Id no válido"); }
                var user_id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
                await _transactionBusiness.Edit(id, NewTransaction, user_id);
                return StatusCode(200);
            }
            catch { throw; }
        }


    }
}

