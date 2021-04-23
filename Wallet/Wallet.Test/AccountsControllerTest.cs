using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.API.Controllers;
using Wallet.Business.Logic;
using Wallet.Entities;
using Xunit;

namespace Wallet.Test
{
    public class AccountsControllerTest : TestBase
    {
        private readonly AccountsController accountsController;

        public AccountsControllerTest()
        {
            var accountBusiness = new AccountBusiness(_unitOfWork);
            context.ChangeTracker.Clear();
            accountsController = new AccountsController(accountBusiness)
            {
                ControllerContext = _controllerContext
            };
        }

        [Fact] //User needs to be logged in to access accounts, no need to check if user exists
        public void ListAccounts_Default_Ok()
        {
            var result = accountsController.ListAccounts();
            Assert.IsType<OkObjectResult>(result);
            var resultOk = (OkObjectResult)result;
            Assert.Equal(200, resultOk.StatusCode);
            Assert.IsType<List<AccountModel>>(resultOk.Value);            
        }

        [Fact]
        public void ListAccounts_Default_Values()
        {
            var result = (OkObjectResult)accountsController.ListAccounts();
            var accounts = (List<AccountModel>)result.Value;
            Assert.Equal(100, accounts[0].Balance);   //Usd account(initialized with a topup transaction)     
            Assert.Equal(100, accounts[1].Balance); //Ars account (initialized with a topup transaction)
        }

        [Fact]
        public void Balance_Default_Ok()
        {
            var result = accountsController.ListBalance();
            Assert.IsType<OkObjectResult>(result);
            var resultOk = (OkObjectResult)result;
            Assert.Equal(200, resultOk.StatusCode);
            Assert.IsType<BalanceModel>(resultOk.Value);
        }        

        [Fact]
        public void Balance_Default_Values()
        {
            var result = (OkObjectResult)accountsController.ListBalance();
            var balances = (BalanceModel)result.Value;
            Assert.Equal(100, balances.UsdBalance);
            Assert.Equal(100, balances.ArgBalance);
        }
    }
}
