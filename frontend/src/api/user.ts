import request from './request'

export interface UserDto {
  id: number
  loginName: string
  realName: string
  mobile: string | null
  email: string | null
  status: number
  mustModifyPwd: number
  lastLoginTime: string | null
  createTime: string
}

export interface UserDetailDto {
  id: number
  loginName: string
  realName: string
  mobile: string | null
  email: string | null
  status: number
  mustModifyPwd: number
  passwordExpireTime: string | null
  lastLoginTime: string | null
  lastLoginIp: string | null
  createTime: string
  organizations: UserOrganizationDto[]
  mainOrganization: UserMainOrganizationDto | null
}

export interface UserOrganizationDto {
  id: number
  unitId: number
  unitName: string | null
  departmentId: number | null
  departmentName: string | null
  positionId: number | null
  positionName: string | null
  regionId: number | null
  startTime: string | null
  endTime: string | null
  status: number
}

export interface UserMainOrganizationDto {
  userOrganizationId: number
  unitId: number
  unitName: string | null
  departmentId: number | null
  departmentName: string | null
}

export interface PageResult<T> {
  items: T[]
  total: number
}

export function searchUsers(params: {
  keyword?: string; status?: number; pageIndex?: number; pageSize?: number
}) {
  return request.get<PageResult<UserDto>>('/user', { params })
}

export function getUserById(id: number) {
  return request.get<UserDetailDto>(`/user/${id}`)
}

export function createUser(data: {
  loginName: string; password: string; realName: string
  mobile?: string; email?: string; idCardNo?: string
  unitId: number; departmentId?: number; positionId?: number; regionId?: number
}) {
  return request.post<UserDetailDto>('/user', data)
}

export function updateUser(id: number, data: { realName?: string; mobile?: string; email?: string }) {
  return request.put(`/user/${id}`, data)
}

export function enableUser(id: number) {
  return request.put(`/user/${id}/enable`)
}

export function disableUser(id: number) {
  return request.put(`/user/${id}/disable`)
}

export function deleteUser(id: number) {
  return request.delete(`/user/${id}`)
}

export function resetPassword(id: number, newPassword: string) {
  return request.post(`/user/${id}/reset-password`, { newPassword })
}

export function changeMainOrganization(userId: number, userOrganizationId: number) {
  return request.put(`/user/${userId}/main-organization`, { userOrganizationId })
}
