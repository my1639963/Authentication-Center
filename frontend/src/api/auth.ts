import request from './request'

export function login(loginName: string, password: string) {
  const params = new URLSearchParams()
  params.append('grant_type', 'password')
  params.append('username', loginName)
  params.append('password', password)

  return request.post('/oauth2/token', params, {
    baseURL: '',
    headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
  })
}
