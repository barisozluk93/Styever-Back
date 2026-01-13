using FAQManagement.DbContexts;
using FAQManagement.Entity;
using FAQManagement.Interfaces;
using FAQManagement.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Reflection;

namespace FAQManagement.Services
{
    public class FAQService : IFAQService
    {
        private readonly FAQManagementContext _dbContext;

        private readonly IConfiguration _configuration;

        public FAQService(FAQManagementContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<Result<List<FAQ>>> GetAll()
        {
            var result = new Result<List<FAQ>>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var queryable = await _dbContext.FAQs
                                        .Where(x => !x.IsDeleted)
                                        .ToListAsync();
                    
                    result.SetData(queryable);
                    result.SetMessage("İşlem başarı ile gerçekleşti.");
                }
                catch (Exception ex)
                {
                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }        
    }
}
