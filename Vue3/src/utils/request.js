import axios from 'axios'
import { ElMessage } from 'element-plus'
import router from '@/router'

const request = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  timeout: 5000
})

// 请求拦截器
request.interceptors.request.use(
  config => {
    const token = localStorage.getItem('token')

    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }

    return config
  },
  error => {
    console.error(error)
    return Promise.reject(error)
  }
)

// 响应拦截器
request.interceptors.response.use(
  response => {
    return response.data
  },
  async error => {
    console.error('统一网络拦截抓到错误啦:', error)

    if (error.response?.status === 401) {
      localStorage.removeItem('token')
      localStorage.removeItem('userName')

      ElMessage.error('登录已过期，请重新登录')

      await router.replace('/login')
    }
    else if (error.response?.status === 403) {
      ElMessage.warning('您没有权限执行此操作')
    }
    else {
      ElMessage.error('网络连接异常，请检查后端服务！')
    }

    return Promise.reject(error)
  }
)

export default request