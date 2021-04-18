using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.API.Controllers;
using Wallet.Business.Logic;
using Wallet.Data.Models;
using Wallet.Entities;
using Xunit;

namespace Wallet.Test
{
    public class FixedTermDepositControllerTest : TestBase
    {
        private readonly FixedTermDepositsController _controller;

        public FixedTermDepositControllerTest()
        {
            var business = new FixedTermDepositBusiness(_unitOfWork, _mapper, _emailSender, _accountBusiness);
            _controller = new FixedTermDepositsController(business)
            {
                ControllerContext = _controllerContext
            };
        }

        [Theory]
        [MemberData(nameof(DataDeposits))]
        public void Get_All(List<FixedTermDeposits> newDeposits, int expectedDeposits)
        {
            // Arrange
            context.FixedTermDeposits.AddRange(newDeposits);
            context.SaveChanges();

            // Act
            var result = _controller.GetAllUserFixedTermDeposits();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var resultCast = (OkObjectResult)result;
            var currentDeposits = (IEnumerable<FixedTermDepositModel>)resultCast.Value;
            Assert.Equal(expectedDeposits, currentDeposits.Count());

            // Rollback
            context.FixedTermDeposits.RemoveRange(newDeposits);
            context.SaveChanges();
        }
        public static IEnumerable<object[]> DataDeposits =>
        new List<object[]>
        {
            new object[] { new List<FixedTermDeposits>(), 0 },
            new object[] { new List<FixedTermDeposits> {
                               new FixedTermDeposits { Id = 1, CreationDate = DateTime.Now, AccountId = 2, Amount = 10 }
                           }, 1},
            new object[] { new List<FixedTermDeposits> {
                               new FixedTermDeposits { Id = 1, CreationDate = DateTime.Now, AccountId = 2, Amount = 10 },
                               new FixedTermDeposits { Id = 2, CreationDate = DateTime.Now, AccountId = 2, Amount = 20 }
                           }, 2},
            new object[] { new List<FixedTermDeposits> {
                               new FixedTermDeposits { Id = 1, CreationDate = DateTime.Now, AccountId = 2, Amount = 10 },
                               new FixedTermDeposits { Id = 2, CreationDate = DateTime.Now, AccountId = 2, Amount = 20 },
                               new FixedTermDeposits { Id = 3, CreationDate = DateTime.Now, AccountId = 1, Amount = 5 }
                           }, 3},
        };


    }
}
