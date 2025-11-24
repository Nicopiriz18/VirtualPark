# Backend implementation of the new requirements

## 1. Reward catalog and redemption rules

The API exposes `RewardController` as the entry point for the Angular UI; administrators call `POST /api/Reward` to create catalog entries and visitors call `POST /api/Reward/{rewardId}/redeem` to trigger a redemption flow that is guarded by `AuthorizationFilter` and resolved to the session owner (`VirtualPark.WebApi/Controllers/RewardController.cs:16-59`).

The business rules live in `RewardService`: it validates the visitor, checks the current `ScoreLog.TotalPointsByVisitor`, enforces membership-level restrictions, decrements stock, and finally creates a `RewardRedemption` plus a negative `ScoreLog` entry in one transaction (`VirtualPark.Application.Scoring/RewardService.cs:13-64`). That `ScoreLog` entry is what keeps the ranking in sync, since the UI relies on the negative points recorded here to immediately reflect the updated totals (`VirtualPark.Domain/ScoreLog.cs:7-88`).

## 2. Preventive maintenance and attraction availability

Scheduling a maintenance event calls `MaintenanceService.ScheduleMaintenance`, which tier-validates the attraction, persists a `PreventiveMaintenance` entity, creates a linked incidence through `IAttractionIncidenceService`, and stores the incidence‑id for future cancellation (`VirtualPark.Application.Maintenance/MaintenanceService.cs:12-85`). Canceling maintenance closes the associated incidence before removing the record, so the incident lifecycle always mirrors the preventive window.

Attraction availability is computed by `AttractionValidationService`: before admitting visitors or ticket purchases the system verifies that no active incidence exists (`VirtualPark.Application.Attractions/AttractionValidationService.cs:9-36`). Because maintenance slots surface as incidences (`IAttractionIncidenceService` contract in `VirtualPark.Application.Attractions/IAttractionIncidenceService.cs:9-22`), the same guard denies access/reservations when a preventive maintenance is open.

## 3. Visitor score history

`VisitorsController` includes an endpoint `GET /api/Visitors/{visitorId}/score-history` that delegates to `ScoreHistoryService` (`VirtualPark.WebApi/Controllers/VisitorsController.cs:54-67`). The service reads `ScoreLog` rows for the visitor and maps them into DTOs that preserve the timestamp, `StrategyUsed`, and `PointsAwarded`, producing a human-readable origin/description pair that lets the UI show whether the log came from a ride or from redeeming a reward (`VirtualPark.Application.Scoring/ScoreHistoryService.cs:10-35`). Because `ScoreLog` stores `StrategyUsed` and `AwardedAt`, each entry has the date/time, source, and active strategy metadata demanded by the requirement (`VirtualPark.Domain/ScoreLog.cs:7-88`).

## 4. Dynamic scoring strategy extensibility

Strategy implementations must satisfy `IScoringStrategy` (name + `CalculatePoints` contract) so they can be discovered by DI (`VirtualPark.Application.Scoring/IScoringStrategy.cs:9-17`). `ScoringService` consumes every registered `IScoringStrategy` via constructor injection, builds a `Dictionary<string, IScoringStrategy>`, and uses `ActiveScoringStrategyRepository` (which persists a single row) to determine which strategy is active when awarding points (`VirtualPark.Application.Scoring/ScoringService.cs:10-117` and `VirtualPark.Infrastructure/Repositories/ActiveScoringStrategy/ActiveScoringStrategyRepository.cs:11-34`). The service also exposes `GetAvailableStrategies()` and `SetActiveStrategy(string)` so administrators can switch the ranking at runtime via the `ScoringController` API.

To plug in a new strategy without recompiling:

1. Build an assembly that implements `IScoringStrategy`, exposing a unique `Name` and full implementation of `CalculatePoints`.
2. Register that implementation with the DI container (or a plugin loader) so that it appears in the `IEnumerable<IScoringStrategy>` that `ScoringService` consumes.
3. Update the `ActiveScoringStrategy` row if a newly loaded strategy should become the default; otherwise it will be available for manual activation through the existing `ScoringController` endpoints.

Documenting `IScoringStrategy` plus the activation endpoint gives third parties the stable contract they need to ship new point-calculation plugins without deploying the core solution.
