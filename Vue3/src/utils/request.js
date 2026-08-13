import axios from 'axios'
import { ElMessage } from 'element-plus'

// 1. 🌟 创建一个专属于咱们 ERP 系统的 Axios 实例
const request = axios.create({
  // 让组件自动去读取刚才我们在 .env.development 里面配好的那个本地后端端口
  baseURL: import.meta.env.VITE_API_BASE_URL, 
  timeout: 5000 // 如果网络卡死，超过 5 秒钟就自动报错断开，不让网页无限转圈
})

// 2. 拦截器
request.interceptors.request.use(
  config => {
    const token = localStorage.getItem('token') // 读取本地存储的 token

    if (token) {
      config.headers.token = token
    }

    return config
  },
  error => {

  console.error(error)

  if (error.response?.status === 401) {
    ElMessage.error("登录已过期，请重新登录")
  } else {
    ElMessage.error("网络异常，请稍后重试")
  }

  return Promise.reject(error)
})

// 3. 响应拦截器
request.interceptors.response.use(
  response => {
    return response.data
  },
  error => {
    console.error('统一网络拦截抓到错误啦:', error)
    ElMessage.error('网络连接异常，请检查后端服务！')
    return Promise.reject(error)
  }
)

// 4. 导出
export default request