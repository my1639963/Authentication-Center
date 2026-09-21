import request from './request'
import type { PageResult } from './user'

// Region
export interface RegionDto {
  id: number
  parentId: number | null
  regionCode: string
  regionName: string
  regionLevel: number
  status: number
  sort: number
}

export function getAllRegions() {
  return request.get<RegionDto[]>('/regions')
}

export function getRegion(id: number) {
  return request.get<RegionDto>(`/regions/${id}`)
}

export function getRegionChildren(parentId: number) {
  return request.get<RegionDto[]>(`/regions/${parentId}/children`)
}

export function createRegion(data: {
  parentId?: number; regionCode: string; regionName: string; regionLevel: number; sort: number
}) {
  return request.post<RegionDto>('/regions', data)
}

export function updateRegion(id: number, data: { regionName?: string; status?: number; sort?: number }) {
  return request.put(`/regions/${id}`, data)
}

// Unit
export interface UnitDto {
  id: number
  parentId: number | null
  unitCode: string
  unitName: string
  unitType: string | null
  unitLevel: number | null
  regionId: number | null
  status: number
  sort: number
  description: string | null
}

export function searchUnits(params: {
  keyword?: string; status?: number; pageIndex?: number; pageSize?: number
}) {
  return request.get<PageResult<UnitDto>>('/units', { params })
}

export function getUnit(id: number) {
  return request.get<UnitDto>(`/units/${id}`)
}

export function getUnitChildren(parentId?: number) {
  return request.get<UnitDto[]>('/units/children', { params: { parentId } })
}

export function createUnit(data: {
  parentId?: number; unitCode: string; unitName: string
  unitType?: string; unitLevel?: number; regionId?: number; sort: number; description?: string
}) {
  return request.post<UnitDto>('/units', data)
}

export function updateUnit(id: number, data: {
  unitName?: string; unitType?: string; unitLevel?: number
  regionId?: number; status?: number; sort?: number; description?: string
}) {
  return request.put(`/units/${id}`, data)
}

export function enableUnit(id: number) {
  return request.put(`/units/${id}/enable`)
}

export function disableUnit(id: number) {
  return request.put(`/units/${id}/disable`)
}

// Department
export interface DepartmentDto {
  id: number
  unitId: number
  parentId: number | null
  deptCode: string
  deptName: string
  leaderUserId: number | null
  status: number
  sort: number
  description: string | null
}

export function getDepartmentsByUnit(unitId: number) {
  return request.get<DepartmentDto[]>(`/units/${unitId}/departments`)
}

export function getDepartment(id: number) {
  return request.get<DepartmentDto>(`/departments/${id}`)
}

export function getDepartmentChildren(unitId: number, parentId?: number) {
  return request.get<DepartmentDto[]>('/departments/children', { params: { unitId, parentId } })
}

export function createDepartment(data: {
  unitId: number; parentId?: number; deptCode: string; deptName: string
  leaderUserId?: number; sort: number; description?: string
}) {
  return request.post<DepartmentDto>('/departments', data)
}

export function updateDepartment(id: number, data: {
  deptName?: string; leaderUserId?: number; status?: number; sort?: number; description?: string
}) {
  return request.put(`/departments/${id}`, data)
}

export function enableDepartment(id: number) {
  return request.put(`/departments/${id}/enable`)
}

export function disableDepartment(id: number) {
  return request.put(`/departments/${id}/disable`)
}

// Position
export interface PositionDto {
  id: number
  positionCode: string
  positionName: string
  positionType: string | null
  positionLevel: number | null
  status: number
  sort: number
  description: string | null
}

export function searchPositions(params: {
  keyword?: string; status?: number; pageIndex?: number; pageSize?: number
}) {
  return request.get<PageResult<PositionDto>>('/positions', { params })
}

export function getPosition(id: number) {
  return request.get<PositionDto>(`/positions/${id}`)
}

export function createPosition(data: {
  positionCode: string; positionName: string; positionType?: string
  positionLevel?: number; sort: number; description?: string
}) {
  return request.post<PositionDto>('/positions', data)
}

export function updatePosition(id: number, data: {
  positionName?: string; positionType?: string; positionLevel?: number
  status?: number; sort?: number; description?: string
}) {
  return request.put(`/positions/${id}`, data)
}

export function enablePosition(id: number) {
  return request.put(`/positions/${id}/enable`)
}

export function disablePosition(id: number) {
  return request.put(`/positions/${id}/disable`)
}
