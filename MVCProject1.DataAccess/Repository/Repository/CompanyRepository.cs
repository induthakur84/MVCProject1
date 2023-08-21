using MVCProject1.DataAccess.Data;
using MVCProject1.DataAccess.Repository.Repository.IRepository;
using MVCProject1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCProject1.DataAccess.Repository.Repository
{
    public class CompanyRepository : Repository<Company>,
       ICompanyRepository
    {
        private readonly ApplicationDbContext _context;
        public CompanyRepository(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }
        public void Update(Company company)
        {
            _context.Update(company);
        }
    }
}


