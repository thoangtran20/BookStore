﻿using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DataAccess.Repository
{
    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CoverTypeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(CoverType coverType)
        {
            _dbContext.CoverTypes.Update(coverType);
        }
    }
}