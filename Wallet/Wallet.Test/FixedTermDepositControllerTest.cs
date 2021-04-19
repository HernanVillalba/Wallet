using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.API.Controllers;
using Wallet.Business;
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
            context.ChangeTracker.Clear();
        }

        [Theory]
        [MemberData(nameof(Data_Get_All))]
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
        }
        public static IEnumerable<object[]> Data_Get_All =>
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

        [Theory]
        [MemberData(nameof(Data_Profit_Ok))]
        public void Calculate_Profit_Ok(double amount, DateTime from, DateTime to, double expected)
        {
            // Arrange at initialization

            // Act
            var result = _controller.calculateProfit("", amount, from, to);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var castResult = (OkObjectResult)result;
            var modelResult = (InterestsCalculationModel)castResult.Value;
            Assert.Equal(expected, Math.Round(modelResult.montoFinal, 4));
        }
        public static IEnumerable<object[]> Data_Profit_Ok =>
        new List<object[]>
        {
            new object[] { 100, DateTime.Now, DateTime.Now.AddDays(1), 101 },
            new object[] { 100, DateTime.Now, DateTime.Now.AddDays(1.99), 101 },
            new object[] { 100, DateTime.Now, DateTime.Now.AddDays(2), 102.01 },
            new object[] { 100, DateTime.Now, DateTime.Now.AddDays(2.01), 102.01 },
            new object[] { 100, DateTime.Now, DateTime.Now.AddDays(3), 103.0301 },
        };

        [Theory]
        [MemberData(nameof(Data_Profit_Error_Day))]
        public void Calculate_Profit_Error_Day(double amount, DateTime from, DateTime to)
        {
            // Arrange at initialization

            // Act
            Func<IActionResult> result = () => _controller.calculateProfit("", amount, from, to);

            // Assert
            var exception = Assert.Throws<CustomException>(result);
            Assert.Equal(400, exception.StatusCode);
            // TODO: Assert for equality of message error
        }
        public static IEnumerable<object[]> Data_Profit_Error_Day =>
        new List<object[]>
        {
            new object[] { 100, DateTime.Now, DateTime.Now },
            new object[] { 100, DateTime.Now, DateTime.Now.AddDays(0.01) },
            new object[] { 100, DateTime.Now, DateTime.Now.AddDays(0.99) },
            new object[] { 100, DateTime.Now, DateTime.Now.AddDays(-0.01) },
            new object[] { 100, DateTime.Now, DateTime.Now.AddDays(-0.99) },
            new object[] { 100, DateTime.Now, DateTime.Now.AddDays(-1) },
            new object[] { 100, DateTime.Now, DateTime.Now.AddDays(-2) },
        };

        [Theory]
        [MemberData(nameof(Data_Profit_Error_Amount))]
        public void Calculate_Profit_Error_Amount(double amount, DateTime from, DateTime to)
        {
            // Arrange at initialization

            // Act
            Func<IActionResult> result = () => _controller.calculateProfit("", amount, from, to);

            // Assert
            var exception = Assert.Throws<CustomException>(result);
            Assert.Equal(400, exception.StatusCode);
            // TODO: Assert for equality of message error
        }
        public static IEnumerable<object[]> Data_Profit_Error_Amount =>
        new List<object[]>
        {
            new object[] { 0, DateTime.Now, DateTime.Now.AddDays(1) },
            new object[] { -double.Epsilon, DateTime.Now, DateTime.Now.AddDays(1) },
            new object[] { -0.1, DateTime.Now, DateTime.Now.AddDays(1) },
            new object[] { -0.99, DateTime.Now, DateTime.Now.AddDays(1) },
            new object[] { -1, DateTime.Now, DateTime.Now.AddDays(1) },
            new object[] { -100, DateTime.Now, DateTime.Now.AddDays(1) },
        };

        [Fact]
        public async void Create_Ok()
        {
            // Arrange
            FixedTermDepositCreateModel fixedTermDeposit = new FixedTermDepositCreateModel
            {
                AccountId = 2,
                Amount = 10
            };

            // Act
            var result = await _controller.CreateFixedTermDeposit(fixedTermDeposit);

            // Assert
            Assert.IsType<OkResult>(result);
            // Get the saved entity from database (asuming there is the first deposit created in the database)
            FixedTermDeposits createdFixedTermDeposit = _unitOfWork.FixedTermDeposits.GetById(1);
            Assert.Equal(fixedTermDeposit.AccountId, createdFixedTermDeposit.AccountId);
            Assert.Equal(fixedTermDeposit.Amount, createdFixedTermDeposit.Amount);
            // Cannot assert anything about .CreationDate because the in-memory database does not save current datetime
            Assert.Null(createdFixedTermDeposit.ClosingDate);
        }
    }
}
