using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Wallet.Business;
using Wallet.Business.Logic;
using Wallet.Data.Repositories.Interfaces;
using Wallet.Entities;
using X.PagedList;

namespace Wallet.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {

        ////////////////////////////////////////////////////////////////////////////falta pasar el business como interfaz////////////////////////////////////////////////////////////////////////////

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
        /// <param name="transactionFilterModel">Transacción</param>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page, [FromQuery]TransactionFilterModel transactionFilterModel)
        {
            try
            {
                if (page <= 0) { page = 1; } //asigna la primer página
                int pageNumber = (int)page, pageSize = 10; //10 registros por página
                var user_id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);

                var ListDB = await tb.GetAll(transactionFilterModel, user_id);
                ListDB = await ListDB.ToPagedList(pageNumber, pageSize).ToListAsync();

                return Ok(ListDB);
            }
            catch { throw; }
        }

        /// <summary>
        /// Mostrar los detalles de una transacción en específico por Id
        /// </summary>
        /// <param name="id">Id de la transacción</param>
        [HttpGet("{id}")]
        public IActionResult Details(int? id)
        {
            try
            {
                if (id == null || id <= 0) { throw new CustomException(400, "Id de la transacción no válido"); }
                var user_id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
                var transaction = tb.Details(id, user_id);
                return Ok(transaction);
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
                int ARS_account_id = _unitOfWork.Accounts.GetAccountId(user_id, "ARS");
                newT.AccountId = ARS_account_id;
                await tb.Create(newT);
                return Ok();
            }
            catch (Exception) { throw new CustomException(404, "No se pudo crear la transacción"); }

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
                await tb.BuyCurrency(tbc, user_id);
                return Ok();
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
                if (id == null || id <= 0) { return BadRequest(); }
                var user_id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
                await tb.Edit(id, NewTransaction, user_id);
                return Ok();
            }
            catch { throw; }
        }

      


        /// <summary>
        /// Transferir dinero de una cuenta propia a otra cuenta existente de la misma moneda
        /// </summary>
        /// <remarks>Ingrese la cuenta de origen, el monto y por último la cuenta de destino</remarks>
        [HttpPost("Transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferModel newTransfer)
        {
            try
            {
                var userId = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
                await tb.Transfer(newTransfer, userId);
                return Ok();
            }
            catch { throw; }
        }
    }
}

