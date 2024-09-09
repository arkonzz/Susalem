﻿using Microsoft.AspNetCore.Authorization;

using Susalem.Abp.Auditing.Features;
using Susalem.Abp.Auditing.Permissions;
using Susalem.Abp.AuditLogging;

using System.ComponentModel.DataAnnotations;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Features;

namespace Susalem.Abp.Auditing.AuditLogs;

[Authorize(AuditingPermissionNames.AuditLog.Default)]
[RequiresFeature(AuditingFeatureNames.Logging.AuditLog)]
public class AuditLogAppService : AuditingApplicationServiceBase, IAuditLogAppService
{
    protected IAuditLogManager AuditLogManager { get; }

    public AuditLogAppService(IAuditLogManager auditLogManager)
    {
        AuditLogManager = auditLogManager;
    }

    public async virtual Task<AuditLogDto> GetAsync(Guid id)
    {
        var auditLog = await AuditLogManager.GetAsync(id, includeDetails: true);

        return ObjectMapper.Map<AuditLog, AuditLogDto>(auditLog);
    }

    public async virtual Task<PagedResultDto<AuditLogDto>> GetListAsync(AuditLogGetByPagedDto input)
    {
        var auditLogCount = await AuditLogManager
            .GetCountAsync(input.StartTime, input.EndTime,
                input.HttpMethod, input.Url, 
                input.UserId, input.UserName, 
                input.ApplicationName, input.CorrelationId,
                input.ClientId, input.ClientIpAddress,
                input.MaxExecutionDuration, input.MinExecutionDuration,
                input.HasException, input.HttpStatusCode);

        var auditLogs = await AuditLogManager
            .GetListAsync(input.Sorting, input.MaxResultCount, input.SkipCount,
            input.StartTime, input.EndTime,
                input.HttpMethod, input.Url, 
                input.UserId, input.UserName, 
                input.ApplicationName, input.CorrelationId,
                input.ClientId, input.ClientIpAddress,
                input.MaxExecutionDuration, input.MinExecutionDuration,
                input.HasException, input.HttpStatusCode, includeDetails: false);

        return new PagedResultDto<AuditLogDto>(auditLogCount,
            ObjectMapper.Map<List<AuditLog>, List<AuditLogDto>>(auditLogs));
    }

    [Authorize(AuditingPermissionNames.AuditLog.Delete)]
    public async virtual Task DeleteAsync([Required] Guid id)
    {
        await AuditLogManager.DeleteAsync(id);
    }
}
