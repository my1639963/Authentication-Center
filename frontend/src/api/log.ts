import request from './request'
import type { PageResult } from './user'

export interface LoginLogDto {
  id: number
  userId: number | null
  loginName: string | null
  clientId: string | null
  loginType: string
  loginResult: number
  failReason: string | null
  ipAddress: string | null
  createTime: string
}

export interface AuditLogDto {
  id: number
  eventType: string
  operationType: string
  operateUserId: number | null
  targetUserId: number | null
  clientId: string | null
  eventResult: number
  failReason: string | null
  content: string | null
  createTime: string
}

export function searchLoginLogs(params: {
  userId?: number; clientId?: string; loginType?: string
  result?: number; startTime?: string; endTime?: string
  pageIndex?: number; pageSize?: number
}) {
  return request.get<PageResult<LoginLogDto>>('/logs/login', { params })
}

export function searchAuditLogs(params: {
  eventType?: string; operationType?: string
  operateUserId?: number; targetUserId?: number; result?: number
  startTime?: string; endTime?: string
  pageIndex?: number; pageSize?: number
}) {
  return request.get<PageResult<AuditLogDto>>('/logs/audit', { params })
}
