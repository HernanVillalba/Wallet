using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wallet.API.Controllers;
using Wallet.Business;
using Wallet.Business.Logic;
using Wallet.Data.Models;
using Wallet.Entities;
using Xunit;

namespace Wallet.Test
{
    public class RefundRequestTest : TestBase
    {
        private readonly RefundRequestsController controller;

        public RefundRequestTest() : base()
        {
            RefundsBusiness refundsBusiness = new(_unitOfWork, _mapper, _accountBusiness, _emailSender);
            context.ChangeTracker.Clear();
            controller = new RefundRequestsController(refundsBusiness)
            {
                ControllerContext = _controllerContext
            };
        }

        [Fact]
        public async void Create_Ok()
        {

            RefundRequestCreateModel model = new() { TransactionId = 10 };

            // Act
            var result = await controller.Create(model);

            // Check if type is status code result
            Assert.IsType<StatusCodeResult>(result);
            var statusCodeResult = (StatusCodeResult)result;

            // Check status code created
            Assert.Equal(201, statusCodeResult.StatusCode);

            // Check if created correctly
            var refund = _unitOfWork.RefundRequest.GetById(1);
            Assert.NotNull(refund);

        }

        [Theory]
        [InlineData(1, 1, "Pending", 1, 3)] // the request cannot be created because it already pending 
        [InlineData(2, 1, "Accepted", 1, 3)] // the request cannot be created because it already accepted
        [InlineData(3, 93, "Canceled", 2, 4)] // transaction not found, (I pretend that a refund request is created so that it throws me the error that I want)
        [InlineData(4, 1, "", 2, 4)] // the transaction is non-refundable because its common

        public async void Create_Error(int id, int transactionId, string status, int sourceAccountId, int TargetAccountId)
        {
            RefundRequest request = new()
            {
                Id = id,
                TransactionId = transactionId,
                Status = status,
                SourceAccountId = sourceAccountId,
                TargetAccountId = TargetAccountId
            };
            Transfers transfer = new()
            {
                Id = 99,
                OriginTransactionId = 1,
                DestinationTransactionId = 2,
            };
            context.AddRange(transfer, request);
            await _unitOfWork.Complete();

            RefundRequestCreateModel model = new() { TransactionId = transactionId };

            async Task result() => await controller.Create(model);

            var ex = await Assert.ThrowsAsync<CustomException>(result);
            Assert.Equal(400, ex.StatusCode);

            //Check messages (THIS IS CORRECT???)
            switch (id)
            {
                case 1: Assert.Equal("No se puede crear la solicitud", ex.Error); break;
                case 2: Assert.Equal("No se puede crear la solicitud", ex.Error); break;
                case 3: Assert.Equal("No se encontró la transacción", ex.Error); break;
                case 4: Assert.Equal("No se puede pedir reembolso de la transacción", ex.Error); break;
            }
            context.RemoveRange(transfer,request);
            await _unitOfWork.Complete();
        }

        [Theory]
        [MemberData(nameof(Data_Get_All))]
        public void Get_All_Ok(List<RefundRequest> newRefunds, int expectedRefunds)
        {
            // Arrange
            context.RefundRequest.AddRange(newRefunds);
            context.SaveChanges();

            // Act
            var result = controller.GetAll();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var resultCast = (OkObjectResult)result;
            var refundList = (IEnumerable<RefundRequestModel>)resultCast.Value;
            Assert.Equal(expectedRefunds, refundList.Count());
        }
        public static IEnumerable<object[]> Data_Get_All =>
        new List<object[]>
        {
            new object[] { new List<RefundRequest>(), 0 },
            new object[] { new List<RefundRequest> {
                               new RefundRequest { TransactionId = 1, Status = "Pending", SourceAccountId = 1, TargetAccountId = 3 }
                           }, 1},
            new object[] { new List<RefundRequest> {
                               new RefundRequest { TransactionId = 1, Status = "Pending", SourceAccountId = 1, TargetAccountId = 3 },
                               new RefundRequest { TransactionId = 1, Status = "Pending", SourceAccountId = 1, TargetAccountId = 3 }
                           }, 2},
            new object[] { new List<RefundRequest> {
                               new RefundRequest { TransactionId = 1, Status = "Pending", SourceAccountId = 1, TargetAccountId = 3 },
                               new RefundRequest { TransactionId = 1, Status = "Pending", SourceAccountId = 1, TargetAccountId = 3 },
                               new RefundRequest { TransactionId = 1, Status = "Pending", SourceAccountId = 1, TargetAccountId = 3 }
                           }, 3},
        };
    }
}
