using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Data;
using UserManagement.DbContexts;
using UserManagement.Model;

namespace UserManagement.BackgroundServices
{
    public class UserDailyWorker : BackgroundService
    {
        private readonly ILogger<UserDailyWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;

        public UserDailyWorker(ILogger<UserDailyWorker> logger, IServiceScopeFactory scopeFactory, IConfiguration configuration) 
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                var nextRun = now.Date.AddHours(1);

                if (now > nextRun)
                    nextRun = nextRun.AddDays(1);

                var delay = nextRun - now;

                _logger.LogInformation($"Bir sonraki çalışma zamanı: {nextRun}");

                try
                {
                    await Task.Delay(delay, stoppingToken);
                    await DoWork(stoppingToken);
                }
                catch (TaskCanceledException)
                {
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "UserDailyWorker hata aldı");
                }
            }
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {

            using var scope = _scopeFactory.CreateScope();

            var _dbContext = scope.ServiceProvider
                .GetRequiredService<UserManagementContext>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var users = await _dbContext.Users.ToListAsync();

                    foreach (var user in users)
                    {
                        if(user.Id != 1 && !user.IsDeleted)
                        {
                            if(user.IsActive && user.IsTrial && user.TrialExpirationDate < DateTime.UtcNow)
                            {
                                user.IsActive = false;

                                await DeactivateUserMemories(user.Id);
                            }

                            if (user.IsActive && !user.IsTrial && user.ExpirationDate < DateTime.UtcNow)
                            {
                                user.IsActive = false;
                            }
                        }

                        await _dbContext.SaveChangesAsync();
                    }

                    transaction.Commit();
                }
                catch (Exception ex) {

                    _logger.LogInformation(ex.Message);
                }
            }

            _logger.LogInformation("🕐 Günlük servis 01:00'da çalıştı");
        }

        private async Task<bool> DeactivateUserMemories(long id)
        {
            HttpClient client = new HttpClient();

            var response = await client.GetAsync(_configuration["AppSettings:ApiUrl"] + "/api/Memory/DeactivateUserMemories/" + id);

            if (response.IsSuccessStatusCode)
            {
                var responseStr = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(responseStr))
                {
                    try
                    {
                        Result<bool> result = JsonConvert.DeserializeObject<Result<bool>>(responseStr);

                        if (result != null)
                        {
                            return result.GetData();
                        }
                        else
                        {
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            return false;
        }

    }
}
