using DTO;
using DAL;
using System.Collections.Generic;

namespace BUS
{
    public class BanBidaBUS
    {
        private readonly BanBidaDAL _dal = new BanBidaDAL();
        public List<BanBidaDTO> GetAll() => _dal.GetAll();
        public BanBidaDTO? GetByMaBan(int maBan) => _dal.GetByMaBan(maBan);
    }
}