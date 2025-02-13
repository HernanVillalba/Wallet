﻿using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;
using Wallet.Entities;

namespace Wallet.Business.Logic
{
    public class RatesBusiness : IRatesBusiness
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RatesBusiness(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Rates> GetRates()
        {
            var rates = _unitOfWork.Rates.GetLastValues();
            //first time, table is empty and the values must be retrieved from the API
            if (rates == null)
            {
                return await SetRates();
            }
            //getting the values after 10 AM requires last values to be from the same day          
            if (DateTime.Now.Hour > 10 && rates.Date.Day == DateTime.Now.Day)
            {
                return rates;
            }
            //getting the values before 10 AM requires last values to be from the previous day
            else if (DateTime.Now.Hour < 10 && rates.Date.Day == DateTime.Now.Day - 1)
            {
                return rates;
            }
            //when any of these conditions is not met the values must be retrieved from the API
            else
            {
                return await SetRates();
            }
        }

        public async Task<Rates> SetRates()
        {
            string dollarApi = "https://www.dolarsi.com/api/api.php?type=valoresprincipales";
            List<Dollar> dollarValues = new List<Dollar>();

            //Call API and deserialize json
            using (var HttpClient = new HttpClient())
            {
                using (var response = await HttpClient.GetAsync(dollarApi))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    dollarValues = JsonConvert.DeserializeObject<List<Dollar>>(apiResponse);
                }
            }
            //Map json response to database model
            Rates rate = new Rates
            {
                BuyingPrice = Convert.ToDouble(dollarValues.First().Casa.Venta),
                SellingPrice = Convert.ToDouble(dollarValues.First().Casa.Compra)
            };
            //try inserting into database
            _unitOfWork.Rates.Insert(rate);
            return rate;
        }

        public IEnumerable<RateModel> GetLatestRates()
        {
            var ratesDB = _unitOfWork.Rates.GetLatestValuesAsync();
            IEnumerable<RateModel> rates = _mapper.Map<IEnumerable<Rates>, IEnumerable<RateModel>>(ratesDB);
            return rates;
            
        }
    }
}
