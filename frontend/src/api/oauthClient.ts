import request from './request'

export interface OAuthClientDto {
  clientId: string
  displayName: string | null
  clientType: string | null
  redirectUris: string[]
  permissions: string[]
}

export function getAllOAuthClients() {
  return request.get<OAuthClientDto[]>('/oauth-clients')
}

export function getOAuthClient(clientId: string) {
  return request.get<OAuthClientDto>(`/oauth-clients/${clientId}`)
}

export function createOAuthClient(data: {
  clientId: string; clientSecret?: string; displayName?: string
  clientType?: string; redirectUris?: string[]; permissions?: string[]
}) {
  return request.post<OAuthClientDto>('/oauth-clients', data)
}

export function updateOAuthClient(clientId: string, data: {
  displayName?: string; redirectUris?: string[]; permissions?: string[]
}) {
  return request.put(`/oauth-clients/${clientId}`, data)
}

export function deleteOAuthClient(clientId: string) {
  return request.delete(`/oauth-clients/${clientId}`)
}
