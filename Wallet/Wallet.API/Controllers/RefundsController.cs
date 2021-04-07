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
    public class RefundsController : ControllerBase
    {
        private readonly IRefundsBusiness _refundsBusiness;
        private int user_id ;
        public RefundsController(IRefundsBusiness refundsBusiness)
        {
            _refundsBusiness = refundsBusiness;
        }
        [HttpPost]
        public IActionResult Create([FromBody]RefundRequestCreateModel refund)
        {
            try
            {
                user_id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
                _refundsBusiness.Create(refund, user_id);
                return StatusCode(201);
            }
            catch { throw; }
        }
    }
}
