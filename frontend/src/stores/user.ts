import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useUserStore = defineStore('user', () => {
  const token = ref(localStorage.getItem('token') || '')
  const loginName = ref('')
  const realName = ref('')

  function setToken(t: string) {
    token.value = t
    localStorage.setItem('token', t)
  }

  function setUserInfo(name: string, real: string) {
    loginName.value = name
    realName.value = real
  }

  function logout() {
    token.value = ''
    loginName.value = ''
    realName.value = ''
    localStorage.removeItem('token')
  }

  return { token, loginName, realName, setToken, setUserInfo, logout }
})
