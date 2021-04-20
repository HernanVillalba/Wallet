using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
    public class RefundRequestsController : ControllerBase
    {
        private readonly IRefundsBusiness _refundsBusiness;
        private int user_id;
        public RefundRequestsController(IRefundsBusiness refundsBusiness)
        {
            _refundsBusiness = refundsBusiness;
        }

        /// <summary>
        /// Mostrar todos los pedidos de reembolso vinculados a este usuario
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                user_id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
                return Ok(_refundsBusiness.GetAll(user_id));
            }
            catch { throw; }
        }

        /// <summary>
        /// Mostrar los detalles de un pedido de reembolso específico
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult Details(int id)
        {
            try
            {
                return Ok(_refundsBusiness.Details(id));
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Crear un pedido de reembolso
        /// </summary>
        /// <param name="refund"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RefundRequestCreateModel refund)
        {
            try
            {
                user_id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
                await _refundsBusiness.Create(refund, user_id);
                return StatusCode(201);
            }
            catch { throw; }
        }

        /// <summary>
        /// Modificar el estado de un pedido de reembolso
        /// </summary>
        /// <param name="id">Id del pedido de reembolso</param>
        /// <param name="action">Acción a tomar: accept/cancel/reject</param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody]RefundRequestActionModel action)
        {
            int userId = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
            try
            {
                switch (action.Action.ToLower())
                {
                    case "accept":
                        await _refundsBusiness.Accept(userId, id);
                        return Ok();
                    case "cancel":
                        await _refundsBusiness.Cancel(userId, id);
                        return Ok();
                    case "reject":
                        await _refundsBusiness.Reject(userId, id);
                        return Ok();
                    default:
                        throw new CustomException(400, "Ingrese una acción válida");
                }
            }
            catch { throw; }
        }
    }
}
