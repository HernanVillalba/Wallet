using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> GetAllAsync([FromQuery] int page, [FromQuery] TransactionFilterModel transactionFilterModel)
        {
            //this is commented until the user can log in
            var user_id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);

            var ListDB = await _transactionBusiness.GetAll(transactionFilterModel, user_id, page);
            return StatusCode(200, ListDB);
        }

        /// <summary>
        /// Mostrar los detalles de una transacción en específico por Id
        /// </summary>
        /// <param name="id">Id de la transacción</param>
        [HttpGet("{id}")]
        public async Task<IActionResult> DetailsAsync(int? id)
        {
            var user_id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
            var transaction = await _transactionBusiness.Details(id, user_id);
            return StatusCode(200, transaction);
        }

        /// <summary>
        /// Crear una transacción en pesos
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] TransactionCreateModel newT)
        {
            //solo transacciones en ARS
            var user_id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
            await _transactionBusiness.Create(newT, user_id);

            return StatusCode(201);

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
            var user_id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);

            await _transactionBusiness.BuyCurrency(tbc, user_id);
            return StatusCode(201);
        }

        /// <summary>
        /// Transferir dinero de una cuenta propia a otra cuenta existente de la misma moneda
        /// </summary>
        /// <remarks>Ingrese la cuenta de origen, el monto y por último la cuenta de destino</remarks>
        [HttpPost("Transfer")]
        public async Task<IActionResult> TransferAsync([FromBody] TransferModel newTransfer)
        {

            var userId = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);

            await _transactionBusiness.Transfer(newTransfer, userId);
            return StatusCode(201);
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
            var user_id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);

            await _transactionBusiness.Edit(id, NewTransaction, user_id);
            return StatusCode(200);
        }


    }
}

