// <copyright file="ServiceCollectionExtensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using IRepositories;
using VirtualPark.Application;
using VirtualPark.Application.Attractions;
using VirtualPark.Application.Clock;
using VirtualPark.Application.Maintenance;
using VirtualPark.Application.Reports;
using VirtualPark.Application.Scoring;
using VirtualPark.Application.Session;
using VirtualPark.Application.SpecialEvent;
using VirtualPark.Application.Tickets;
using VirtualPark.Application.Users;
using VirtualPark.Application.Visitors;
using VirtualPark.Domain.Interfaces.Repositories;
using VirtualPark.Domain.Interfaces.Security;
using VirtualPark.Infrastructure.Repositories;
using VirtualPark.Infrastructure.Repositories.ScoreLogs;
using VirtualPark.Infrastructure.Repositories.Visitors;
using VirtualPark.Infrastructure.Security;

namespace VirtualPark.WebApi.Configuration;

public static class ServiceCollectionExtensions
{
    [ExcludeFromCodeCoverage]
    public static void AddServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddSingleton(configuration);

        // Application Services
        serviceCollection.AddScoped<IAttractionService, AttractionService>();
        serviceCollection.AddScoped<IAttractionIncidenceService, AttractionIncidenceService>();
        serviceCollection.AddScoped<IAttractionValidationService, AttractionValidationService>();
        serviceCollection.AddScoped<ISpecialEventService, SpecialEventService>();
        serviceCollection.AddScoped<IUserService, UserService>();
        serviceCollection.AddScoped<IClockService, ClockService>();
        serviceCollection.AddScoped<IAttractionAccessService, AttractionAccessService>();
        serviceCollection.AddScoped<IScoringService, ScoringService>();
        serviceCollection.AddScoped<IVisitorService, VisitorService>();
        serviceCollection.AddScoped<ITicketService, TicketService>();
        serviceCollection.AddScoped<ITicketLookupService, TicketLookupService>();
        serviceCollection.AddScoped<ISessionService, SessionService>();
        serviceCollection.AddScoped<IReportsService, ReportsService>();
        serviceCollection.AddScoped<IAuthService, AuthService>();
        serviceCollection.AddScoped<IMaintenanceService, MaintenanceService>();
        serviceCollection.AddScoped<IRewardService, RewardService>();
        serviceCollection.AddScoped<IScoreHistoryService, ScoreHistoryService>();

        // Repositories
        serviceCollection.AddScoped<IAttractionRepository, AttractionRepository>();
        serviceCollection.AddScoped<IIncidenceRepository, IncidenceRepository>();
        serviceCollection.AddScoped<IAttractionAccessRepository, AttractionAccessRepository>();
        serviceCollection.AddScoped<ISpecialEventRepository, SpecialEventRepository>();
        serviceCollection.AddScoped<IUserRepository, UserRepository>();
        serviceCollection.AddScoped<IVisitorRepository, VisitorRepository>();
        serviceCollection.AddScoped<ITicketRepository, TicketRepository>();
        serviceCollection.AddScoped<IScoreLogRepository, ScoreLogRepository>();
        serviceCollection.AddScoped<IClockRepository, ClockRepository>();
        serviceCollection.AddScoped<ISessionRepository, SessionRepository>();
        serviceCollection.AddScoped<IActiveScoringStrategyRepository, ActiveScoringStrategyRepository>();
        serviceCollection.AddScoped<IMaintenanceRepository, MaintenanceRepository>();
        serviceCollection.AddScoped<IRewardRepository, RewardRepository>();
        serviceCollection.AddScoped<IRewardRedemptionRepository, RewardRedemptionRepository>();

        // Infrastructure Services
        serviceCollection.AddScoped<ITokenGenerator, TokenGenerator>();
        serviceCollection.AddScoped<IPasswordHasher, PasswordHasher>();

        // Register strategy loader with plugins path
        // Navigate up from bin/Debug/net8.0 to solution root, then to Plugins folder
        var pluginsPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../Plugins"));
        serviceCollection.AddSingleton<IScoringStrategyLoader>(_ => new ScoringStrategyLoader(pluginsPath));
        serviceCollection.AddSingleton<IStrategyPluginStore>(_ => new StrategyPluginStore(pluginsPath));
    }
}
