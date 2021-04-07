using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;

namespace Wallet.Business.Logic
{
    public class RefundsBusiness : IRefundsBusiness
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public RefundsBusiness(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RefundRequest>> algo()
        {
            IEnumerable<RefundRequest> lista = await _unitOfWork.RefundRequest.GetAllAsync();
            return lista;
        }
    }
}
