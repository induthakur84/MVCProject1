﻿using MVCProject1.DataAccess.Data;
using MVCProject1.DataAccess.Repository.Repository.IRepository;
using MVCProject1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCProject1.DataAccess.Repository.Repository
{
    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        private readonly ApplicationDbContext _context;
        public CoverTypeRepository(ApplicationDbContext context):base(context) 
        {
            _context = context;
        }
        public void Update(CoverType coverType)
        {
            _context.Update(coverType);
        }
    }
}
