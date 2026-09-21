export function formatDateTime(dt: string | null | undefined): string {
  if (!dt) return '-'
  return new Date(dt).toLocaleString('zh-CN', {
    year: 'numeric', month: '2-digit', day: '2-digit',
    hour: '2-digit', minute: '2-digit', second: '2-digit'
  })
}

export function statusLabel(status: number): string {
  const map: Record<number, string> = { 0: '正常', 1: '禁用', 2: '锁定' }
  return map[status] ?? '未知'
}

export function statusTagType(status: number): 'success' | 'danger' | 'warning' {
  const map: Record<number, 'success' | 'danger' | 'warning'> = { 0: 'success', 1: 'danger', 2: 'warning' }
  return map[status] ?? 'danger'
}

export function resultLabel(result: number): string {
  return result === 1 ? '成功' : '失败'
}

export function resultTagType(result: number): 'success' | 'danger' {
  return result === 1 ? 'success' : 'danger'
}
