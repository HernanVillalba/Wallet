using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wallet.Entities;
using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;
using Wallet.Business.Logic;

namespace Wallet.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FixedTermDepositController : ControllerBase
    {
        private readonly IFixedTermDepositBusiness _fixedTermDepositBusiness;

        public FixedTermDepositController(IFixedTermDepositBusiness fixedTermDepositBusiness)
        {
            _fixedTermDepositBusiness = fixedTermDepositBusiness;
        }

        [HttpGet]
        public IActionResult GetAllUserFixedTermDeposits()
        {
            try
            {
                // Get the current user's id logged to the API
                var userId = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);

                // Get all the fixed term deposits of the current user
                var fixedTermDeposits = _fixedTermDepositBusiness.GetAllByUserId(userId);

                return Ok(fixedTermDeposits);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateFixedTermDeposit([FromBody] FixedTermDepositCreateModel fixedTermDeposit)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    // Get the current user's id logged to the API
                    var userId = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);

                    // Delegate the logic to business
                    await _fixedTermDepositBusiness.CreateFixedTermDeposit(fixedTermDeposit, userId);

                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest("Datos de entrada inválidos.");
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> CloseFixedTermDeposit(int? id)
        {
            try
            {
                // Delegate the logic to business
                await _fixedTermDepositBusiness.CloseFixedTermDeposit((int)id);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
