using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    }
}
