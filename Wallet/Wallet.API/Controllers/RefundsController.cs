using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wallet.Business.Logic;

namespace Wallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefundsController : ControllerBase
    {
        private readonly IRefundsBusiness _refundsBusiness;
        public RefundsController(IRefundsBusiness refundsBusiness)
        {
            _refundsBusiness = refundsBusiness;
        }
        [HttpGet]
        public async Task<IActionResult> algoAsync()
        {
            var lista = await _refundsBusiness.algo();
            return Ok(lista);
        }
    }
}
