using Microsoft.EntityFrameworkCore;
using UserManagement.DbContexts;
using UserManagement.Entity;
using UserManagement.Interfaces;
using UserManagement.Model;

namespace UserManagement.Services
{
    public class LegalContentService : ILegalContentService
    {
        private readonly UserManagementContext _dbContext;
        public LegalContentService(UserManagementContext dbContext) => _dbContext = dbContext;

        public async Task<Result<PagingResult<PagedList<LegalContent>>>> Paginate(PagingParameter pagingParameter)
        {
            var result = new Result<PagingResult<PagedList<LegalContent>>>();
            try
            {
                var query = _dbContext.LegalContents.AsNoTracking().AsQueryable();

                if (pagingParameter.Id.HasValue)
                    query = query.Where(x => x.Id == pagingParameter.Id.Value);

                if (!string.IsNullOrWhiteSpace(pagingParameter.Category))
                {
                    var category = pagingParameter.Category.Trim().ToLower();
                    query = query.Where(x => x.Category.ToLower() == category);
                }

                if (!string.IsNullOrWhiteSpace(pagingParameter.Title))
                {
                    var title = pagingParameter.Title.Trim().ToLower();
                    query = query.Where(x => x.Title.ToLower().Contains(title));
                }

                if (!string.IsNullOrWhiteSpace(pagingParameter.TitleEn))
                {
                    var titleEn = pagingParameter.TitleEn.Trim().ToLower();
                    query = query.Where(x => x.TitleEn.ToLower().Contains(titleEn));
                }

                if (!string.IsNullOrWhiteSpace(pagingParameter.Slug))
                {
                    var slug = pagingParameter.Slug.Trim().ToLower();
                    query = query.Where(x => x.Slug.ToLower().Contains(slug));
                }

                if (pagingParameter.SortOrder.HasValue)
                    query = query.Where(x => x.SortOrder == pagingParameter.SortOrder.Value);

                if (pagingParameter.IsDeleted.HasValue)
                    query = query.Where(x => x.IsDeleted == pagingParameter.IsDeleted.Value);

                var ordered = query.OrderBy(x => x.SortOrder).ThenBy(x => x.Id);
                var pagination = PagedList<LegalContent>.ToPagedList(ordered, pagingParameter.PageNumber, pagingParameter.PageSize);

                result.SetData(new PagingResult<PagedList<LegalContent>>
                {
                    Items = pagination,
                    TotalCount = pagination.TotalCount
                });
                result.SetMessage("İşlem başarı ile gerçekleşti.");
            }
            catch (Exception ex)
            {
                result.SetIsSuccess(false);
                result.SetMessage(ex.Message);
            }

            return result;
        }

        public async Task<Result<List<LegalContent>>> GetAll(bool includeDeleted = false)
        {
            var result = new Result<List<LegalContent>>();
            try
            {
                var query = _dbContext.LegalContents.AsNoTracking().AsQueryable();
                if (!includeDeleted) query = query.Where(x => !x.IsDeleted);
                result.SetData(await query.OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToListAsync());
                result.SetMessage("İşlem başarı ile gerçekleşti.");
            }
            catch (Exception ex) { result.SetIsSuccess(false); result.SetMessage(ex.Message); }
            return result;
        }

        public async Task<Result<LegalContent>> GetById(long id)
        {
            var result = new Result<LegalContent>();
            try
            {
                var item = await _dbContext.LegalContents.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
                if (item == null) { result.SetIsSuccess(false); result.SetMessage("İçerik bulunamadı."); return result; }
                result.SetData(item); result.SetMessage("İşlem başarı ile gerçekleşti.");
            }
            catch (Exception ex) { result.SetIsSuccess(false); result.SetMessage(ex.Message); }
            return result;
        }

        public async Task<Result<LegalContent>> GetBySlug(string slug)
        {
            var result = new Result<LegalContent>();
            try
            {
                slug = NormalizeSlug(slug);
                var item = await _dbContext.LegalContents.AsNoTracking().FirstOrDefaultAsync(x => x.Slug == slug && !x.IsDeleted);
                if (item == null) { result.SetIsSuccess(false); result.SetMessage("İçerik bulunamadı."); return result; }
                result.SetData(item); result.SetMessage("İşlem başarı ile gerçekleşti.");
            }
            catch (Exception ex) { result.SetIsSuccess(false); result.SetMessage(ex.Message); }
            return result;
        }

        public async Task<Result<LegalContent>> Save(LegalContent item)
        {
            var result = new Result<LegalContent>();
            try
            {
                Normalize(item);
                if (!Validate(item, result)) return result;
                if (await _dbContext.LegalContents.AnyAsync(x => !x.IsDeleted && x.Slug == item.Slug))
                { result.SetIsSuccess(false); result.SetMessage("Aynı sayfa anahtarıyla kayıt bulunmaktadır."); return result; }
                item.Id = 0; item.IsDeleted = false;
                _dbContext.LegalContents.Add(item); await _dbContext.SaveChangesAsync();
                result.SetData(item); result.SetMessage("İçerik başarıyla eklendi.");
            }
            catch (Exception ex) { result.SetIsSuccess(false); result.SetMessage(ex.Message); }
            return result;
        }

        public async Task<Result<LegalContent>> Update(LegalContent item)
        {
            var result = new Result<LegalContent>();
            try
            {
                var existing = await _dbContext.LegalContents.FirstOrDefaultAsync(x => x.Id == item.Id && !x.IsDeleted);
                if (existing == null) { result.SetIsSuccess(false); result.SetMessage("İçerik bulunamadı."); return result; }
                Normalize(item); if (!Validate(item, result)) return result;
                if (await _dbContext.LegalContents.AnyAsync(x => x.Id != item.Id && !x.IsDeleted && x.Slug == item.Slug))
                { result.SetIsSuccess(false); result.SetMessage("Aynı sayfa anahtarıyla başka bir kayıt bulunmaktadır."); return result; }
                existing.Slug = item.Slug; existing.Category = item.Category; existing.Title = item.Title; existing.TitleEn = item.TitleEn;
                existing.Content = item.Content; existing.ContentEn = item.ContentEn; existing.SortOrder = item.SortOrder;
                await _dbContext.SaveChangesAsync(); result.SetData(existing); result.SetMessage("İçerik başarıyla güncellendi.");
            }
            catch (Exception ex) { result.SetIsSuccess(false); result.SetMessage(ex.Message); }
            return result;
        }

        public async Task<Result<LegalContent>> Delete(long id)
        {
            var result = new Result<LegalContent>();
            try
            {
                var existing = await _dbContext.LegalContents.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
                if (existing == null) { result.SetIsSuccess(false); result.SetMessage("İçerik bulunamadı."); return result; }
                existing.IsDeleted = true; await _dbContext.SaveChangesAsync();
                result.SetData(existing); result.SetMessage("İçerik başarıyla silindi.");
            }
            catch (Exception ex) { result.SetIsSuccess(false); result.SetMessage(ex.Message); }
            return result;
        }

        private static bool Validate(LegalContent item, Result<LegalContent> result)
        {
            if (string.IsNullOrWhiteSpace(item.Slug) || string.IsNullOrWhiteSpace(item.Title) || string.IsNullOrWhiteSpace(item.TitleEn) || string.IsNullOrWhiteSpace(item.Content) || string.IsNullOrWhiteSpace(item.ContentEn))
            { result.SetIsSuccess(false); result.SetMessage("Sayfa anahtarı, başlık ve içerikler zorunludur."); return false; }
            if (item.Category != "Legal" && item.Category != "Community")
            { result.SetIsSuccess(false); result.SetMessage("Kategori Legal veya Community olmalıdır."); return false; }
            return true;
        }

        private static void Normalize(LegalContent item)
        {
            item.Slug = NormalizeSlug(item.Slug); item.Category = item.Category?.Trim() ?? string.Empty;
            item.Title = item.Title?.Trim() ?? string.Empty; item.TitleEn = item.TitleEn?.Trim() ?? string.Empty;
            item.Content = item.Content?.Trim() ?? string.Empty; item.ContentEn = item.ContentEn?.Trim() ?? string.Empty;
        }
        private static string NormalizeSlug(string? slug) => (slug ?? string.Empty).Trim().Trim('/').ToLowerInvariant();
    }
}
