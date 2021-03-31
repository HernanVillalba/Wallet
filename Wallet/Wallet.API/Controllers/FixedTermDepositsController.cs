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
using Wallet.Business;

namespace Wallet.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FixedTermDepositsController : ControllerBase
    {
        private readonly IFixedTermDepositBusiness _fixedTermDepositBusiness;

        public FixedTermDepositsController(IFixedTermDepositBusiness fixedTermDepositBusiness)
        {
            _fixedTermDepositBusiness = fixedTermDepositBusiness;
        }

        /// <summary>
        /// Listar todos los plazos fijos abiertos por el usuario logueado
        /// </summary>
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
            catch
            {
                throw;
            }
        }

        // Is it okay to have the calculateProfit() function inside FixedTermDeposit's Controller?
        /// <summary>
        /// Calcular intereses de un potencial plazo fijo
        /// </summary>
        /// <param name="currency">Moneda del plazo fijo</param>
        /// <param name="amount">Monto inicial</param>
        /// <param name="from">Fecha de inicio (AAAA-MM-DD)</param>
        /// <param name="to">Fecha de fin (AAAA-MM-DD)</param>
        [AllowAnonymous]
        [HttpGet]
        [Route("/api/calculateProfit")]
        public IActionResult calculateProfit(string currency, double amount, DateTime from, DateTime to)
        {
            try
            {
                var profit = _fixedTermDepositBusiness.calculateProfit(currency, amount, from, to);

                return Ok(profit);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Crear un nuevo plazo fijo
        /// </summary>
        /// <response code="200">Plazo fijo creado correctamente</response>
        [HttpPost]
        public async Task<IActionResult> CreateFixedTermDeposit([FromBody] FixedTermDepositCreateModel fixedTermDeposit)
        {
            try
            {
                // Get the current user's id logged to the API
                var userId = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);

                // Delegate the logic to business
                await _fixedTermDepositBusiness.CreateFixedTermDeposit(fixedTermDeposit, userId);

                return Ok();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Cerrar un plazo fijo en específico por su Id
        /// </summary>
        /// <param name="id">Id del plazo fijo</param>
        /// <response code="200">Plazo fijo cerrado correctamente</response>
        [HttpPost("{id}/Close")]
        public async Task<IActionResult> CloseFixedTermDeposit(int id)
        {
            try
            {
                // Delegate the logic to business
                await _fixedTermDepositBusiness.CloseFixedTermDeposit(id);

                return Ok();
            }
            catch
            {
                throw;
            }
        }
    }
}
