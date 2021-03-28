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
    public class FixedTermDepositController : ControllerBase
    {
        private readonly IFixedTermDepositBusiness _fixedTermDepositBusiness;

        public FixedTermDepositController(IFixedTermDepositBusiness fixedTermDepositBusiness)
        {
            _fixedTermDepositBusiness = fixedTermDepositBusiness;
        }

        /// <summary>
        /// Listar todos los plazos fijos abiertos por el usuario logueado
        /// </summary>
        [HttpGet]
        public IActionResult GetAllUserFixedTermDeposits()
        {
            // Get the current user's id logged to the API
            var userId = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);

            // Get all the fixed term deposits of the current user
            var fixedTermDeposits = _fixedTermDepositBusiness.GetAllByUserId(userId);

            return Ok(fixedTermDeposits);
        }

        /// <summary>
        /// Crea un nuevo plazo fijo
        /// </summary>
        /// <response code="200">Plazo fijo creado correctamente</response>
        [HttpPost]
        public async Task<IActionResult> CreateFixedTermDeposit([FromBody] FixedTermDepositCreateModel fixedTermDeposit)
        {
            // Get the current user's id logged to the API
            var userId = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);

            // Delegate the logic to business
            await _fixedTermDepositBusiness.CreateFixedTermDeposit(fixedTermDeposit, userId);

            return Ok(new { message = "Plazo fijo creado correctamente" });
        }

        /// <summary>
        /// Cierra un plazo fijo en específico por su Id
        /// </summary>
        /// <param name="id">Id del plazo fijo</param>
        /// <response code="200">Plazo fijo cerrado correctamente</response>
        [HttpPatch("{id}")]
        public async Task<IActionResult> CloseFixedTermDeposit(int? id)
        {
            // Delegate the logic to business
            await _fixedTermDepositBusiness.CloseFixedTermDeposit((int)id);

            return Ok(new { message = "Plazo fijo cerrado correctamente" });
        }
    }
}
