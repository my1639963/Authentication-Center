import request from './request'

export interface AdminRoleDto {
  id: number
  roleCode: string
  roleName: string
  description: string | null
  status: number
}

export interface AdminPermissionDto {
  id: number
  permissionCode: string
  permissionName: string
  permissionGroup: string | null
  description: string | null
  status: number
}

export function getAllRoles() {
  return request.get<AdminRoleDto[]>('/admin/roles')
}

export function getRole(id: number) {
  return request.get<AdminRoleDto>(`/admin/roles/${id}`)
}

export function createRole(data: { roleCode: string; roleName: string; description?: string }) {
  return request.post<AdminRoleDto>('/admin/roles', data)
}

export function updateRole(id: number, data: { roleName?: string; status?: number; description?: string }) {
  return request.put(`/admin/roles/${id}`, data)
}

export function assignPermissions(roleId: number, permissionIds: number[]) {
  return request.put(`/admin/roles/${roleId}/permissions`, { permissionIds })
}

export function getRolePermissions(roleId: number) {
  return request.get<AdminPermissionDto[]>(`/admin/roles/${roleId}/permissions`)
}

export function getAllPermissions() {
  return request.get<AdminPermissionDto[]>('/admin/permissions')
}

export function getPermissionsByGroup(group: string) {
  return request.get<AdminPermissionDto[]>(`/admin/permissions/groups/${group}`)
}

export function getUserRoles(userId: number) {
  return request.get<AdminRoleDto[]>(`/admin/users/${userId}/roles`)
}

export function assignRoles(userId: number, roleIds: number[]) {
  return request.put(`/admin/users/${userId}/roles`, { roleIds })
}

export function getUserPermissions(userId: number) {
  return request.get<AdminPermissionDto[]>(`/admin/users/${userId}/permissions`)
}
