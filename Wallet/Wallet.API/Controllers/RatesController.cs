using Microsoft.AspNetCore.Mvc;
using Wallet.Business.Logic;

namespace Wallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatesController : ControllerBase
    {
        private readonly IRatesBusiness _ratesBusiness;
        public RatesController(IRatesBusiness ratesBusiness)
        {
            _ratesBusiness = ratesBusiness;
        }

        /// <summary>
        /// Mostrar los últimos 10 valores del dolar guardados
        /// </summary>
        [HttpGet]
        public IActionResult GetLatest()
        {
            return Ok(_ratesBusiness.GetLatestRates());
        }

    }
}
