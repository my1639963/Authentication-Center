import request from './request'

export interface DashboardStats {
  userCount: number
  roleCount: number
  permissionCount: number
  unitCount: number
  departmentCount: number
  positionCount: number
  todayLoginSuccess: number
  todayLoginFail: number
}

export interface LoginLogItem {
  id: number
  userId: number | null
  loginName: string | null
  loginType: string
  loginResult: number
  failReason: string | null
  ipAddress: string | null
  createTime: string
}

export interface AuditLogItem {
  id: number
  eventType: string
  operationType: string
  operateUserId: number | null
  eventResult: number
  failReason: string | null
  createTime: string
}

export function getDashboardStats() {
  return request.get<DashboardStats>('/dashboard/stats')
}

export function getRecentLogins(count = 5) {
  return request.get<LoginLogItem[]>('/dashboard/recent-logins', { params: { count } })
}

export function getRecentAudits(count = 5) {
  return request.get<AuditLogItem[]>('/dashboard/recent-audits', { params: { count } })
}
