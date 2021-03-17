using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;

namespace Wallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FixedTermDepositController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public FixedTermDepositController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        
    }
}
