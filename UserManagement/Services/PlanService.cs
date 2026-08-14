using Microsoft.EntityFrameworkCore;
using UserManagement.DbContexts;
using UserManagement.Entity;
using UserManagement.Interfaces;
using UserManagement.Model;

namespace UserManagement.Services
{
    public class PlanService : IPlanService
    {
        private readonly UserManagementContext _dbContext;

        public PlanService(UserManagementContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<List<Plan>>> GetAll(bool includeDeleted = false)
        {
            var result = new Result<List<Plan>>();
            try
            {
                var query = _dbContext.Plans.AsNoTracking().AsQueryable();
                if (!includeDeleted)
                    query = query.Where(x => !x.IsDeleted);

                result.SetData(await query.OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync());
                result.SetMessage("İşlem başarı ile gerçekleşti.");
            }
            catch (Exception ex)
            {
                result.SetIsSuccess(false);
                result.SetMessage(ex.Message);
            }
            return result;
        }

        public async Task<Result<Plan>> GetById(long id)
        {
            var result = new Result<Plan>();
            try
            {
                var plan = await _dbContext.Plans.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
                if (plan == null)
                {
                    result.SetIsSuccess(false);
                    result.SetMessage("Böyle bir paket bulunmamaktadır.");
                    return result;
                }

                result.SetData(plan);
                result.SetMessage("İşlem başarı ile gerçekleşti.");
            }
            catch (Exception ex)
            {
                result.SetIsSuccess(false);
                result.SetMessage(ex.Message);
            }
            return result;
        }

        public async Task<Result<Plan>> Save(Plan plan)
        {
            var result = new Result<Plan>();
            try
            {
                Normalize(plan);
                if (string.IsNullOrWhiteSpace(plan.Name) || string.IsNullOrWhiteSpace(plan.NameEn))
                {
                    result.SetIsSuccess(false);
                    result.SetMessage("Paket adı zorunludur.");
                    return result;
                }
                if (plan.Price < 0)
                {
                    result.SetIsSuccess(false);
                    result.SetMessage("Paket fiyatı 0'dan küçük olamaz.");
                    return result;
                }
                if (await _dbContext.Plans.AnyAsync(x => !x.IsDeleted && (x.Name == plan.Name || x.NameEn == plan.NameEn)))
                {
                    result.SetIsSuccess(false);
                    result.SetMessage("Aynı isimle tanımlı bir paket bulunmaktadır.");
                    return result;
                }

                plan.Id = 0;
                plan.IsDeleted = false;
                _dbContext.Plans.Add(plan);
                await _dbContext.SaveChangesAsync();
                result.SetData(plan);
                result.SetMessage("Paket başarıyla eklendi.");
            }
            catch (Exception ex)
            {
                result.SetIsSuccess(false);
                result.SetMessage(ex.Message);
            }
            return result;
        }

        public async Task<Result<Plan>> Update(Plan plan)
        {
            var result = new Result<Plan>();
            try
            {
                var existing = await _dbContext.Plans.FirstOrDefaultAsync(x => x.Id == plan.Id && !x.IsDeleted);
                if (existing == null)
                {
                    result.SetIsSuccess(false);
                    result.SetMessage("Böyle bir paket bulunmamaktadır.");
                    return result;
                }

                Normalize(plan);
                if (await _dbContext.Plans.AnyAsync(x => x.Id != plan.Id && !x.IsDeleted && (x.Name == plan.Name || x.NameEn == plan.NameEn)))
                {
                    result.SetIsSuccess(false);
                    result.SetMessage("Aynı isimle tanımlı başka bir paket bulunmaktadır.");
                    return result;
                }

                existing.Name = plan.Name;
                existing.NameEn = plan.NameEn;
                existing.Price = plan.Price;
                existing.Currency = plan.Currency;
                existing.Period = plan.Period;
                existing.PeriodEn = plan.PeriodEn;
                existing.Properties = plan.Properties;
                existing.PropertiesEn = plan.PropertiesEn;
                existing.SortOrder = plan.SortOrder;
                existing.IsPopular = plan.IsPopular;

                await _dbContext.SaveChangesAsync();
                result.SetData(existing);
                result.SetMessage("Paket başarıyla güncellendi.");
            }
            catch (Exception ex)
            {
                result.SetIsSuccess(false);
                result.SetMessage(ex.Message);
            }
            return result;
        }

        public async Task<Result<Plan>> Delete(long id)
        {
            var result = new Result<Plan>();
            try
            {
                var existing = await _dbContext.Plans.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
                if (existing == null)
                {
                    result.SetIsSuccess(false);
                    result.SetMessage("Böyle bir paket bulunmamaktadır.");
                    return result;
                }

                existing.IsDeleted = true;
                await _dbContext.SaveChangesAsync();
                result.SetData(existing);
                result.SetMessage("Paket başarıyla silindi.");
            }
            catch (Exception ex)
            {
                result.SetIsSuccess(false);
                result.SetMessage(ex.Message);
            }
            return result;
        }

        private static void Normalize(Plan plan)
        {
            plan.Name = plan.Name?.Trim() ?? string.Empty;
            plan.NameEn = plan.NameEn?.Trim() ?? string.Empty;
            plan.Currency = string.IsNullOrWhiteSpace(plan.Currency) ? "₺" : plan.Currency.Trim();
            plan.Period = string.IsNullOrWhiteSpace(plan.Period) ? "Yıl" : plan.Period.Trim();
            plan.PeriodEn = string.IsNullOrWhiteSpace(plan.PeriodEn) ? "Year" : plan.PeriodEn.Trim();
            plan.Properties = plan.Properties?.Trim() ?? string.Empty;
            plan.PropertiesEn = plan.PropertiesEn?.Trim() ?? string.Empty;
        }
    }
}
