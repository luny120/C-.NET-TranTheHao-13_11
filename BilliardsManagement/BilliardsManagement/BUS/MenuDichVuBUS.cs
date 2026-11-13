using DTO;
using DAL;
using System.Collections.Generic;

namespace BUS
{
    public class MenuDichVuBUS
    {
        private readonly MenuDichVuDAL _dal = new MenuDichVuDAL();
        public List<MenuDichVuDTO> GetAll() => _dal.GetAll();
        public MenuDichVuDTO? GetByMaDV(int maDV) => _dal.GetByMaDV(maDV);
    }
}