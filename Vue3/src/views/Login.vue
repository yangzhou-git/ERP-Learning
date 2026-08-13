<template>
  <div class="login-page">
    <div class="login-card">
      <div class="login-left">
        <div class="brand-wrapper">
          <div class="brand-text-box">
            <h2 class="brand-title">智阅书联 ERP</h2>
            <p class="brand-desc">智能管理 · 高效协同 · 数据驱动</p>
            <div class="brand-line"></div>
          </div>
        </div>
      </div>
      <div class="login-right">
        <div class="form-wrapper">
          <div class="form-header">
            <div class="logo-area">
              <el-icon class="logo-icon" color="#409eff"><Reading /></el-icon>
              <span class="logo-title">智阅书联 ERP</span>
            </div>
            <p class="form-subtitle">欢迎回来，请登录系统</p>
          </div>
          <el-form :model="loginModel" label-position="top" size="large">
            <el-form-item label="账号" class="custom-form-item">
              <el-input v-model="loginModel.userName" placeholder="请输入账号" :prefix-icon="User" clearable/>
            </el-form-item>
            <el-form-item label="密码" class="custom-form-item">
              <el-input v-model="loginModel.password" type="password" placeholder="请输入密码" :prefix-icon="Lock" show-password/>
            </el-form-item>
            <div class="form-actions">
              <el-checkbox v-model="rememberMe" label="记住我" />
              <span class="forget-pwd">忘记密码？</span>
            </div>
            <el-form-item style="margin-top: 24px;">
              <el-button type="primary" class="submit-btn" @click="handleLogin">
                立即登录
              </el-button>
            </el-form-item>
            <div class="go-register">
              还没有账号？<span @click="router.push('/register')">立即注册</span>
            </div>
          </el-form>
        </div>
      </div>
    </div>
    <div class="footer-copyright">
      © 2026 智阅书联 ERP. 保留所有权利。
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { ElMessage } from 'element-plus'
import { useRouter } from 'vue-router'
import { User, Lock, Reading } from '@element-plus/icons-vue'
import { LoginApi } from '@/api/auth/auth.js'

const router = useRouter()
const rememberMe = ref(false)

const loginModel = ref({
  userName: '',
  password: ''
})

//  点击登录
const handleLogin = async () => {
  try {
    const res = await LoginApi(loginModel.value)
    console.log('登录接口返回的数据：', res)

    if (res.code === 200) {
      // 保存token
      localStorage.setItem('token', res.data.token)

      // 保存当前登录用户名
      localStorage.setItem('userName', res.data.userName)
      ElMessage.success('登录成功！')
      router.push('/index') 
    } else {
      ElMessage.error('登录失败：' + res.message)
    }
  } catch (error) {
    console.error('登录请求出错：', error)
    ElMessage.error('登录请求失败，请稍后再试！')
  }
}
</script>

<style scoped>
.login-page {
  height: 100vh;
  width: 100vw;
  background: #f4f7fb;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  overflow: hidden;
  font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Helvetica, Arial, sans-serif;
}

/* 💳 核心大卡片（左右双拼） */
.login-card {
  width: 1100px;
  height: 600px;
  background: #ffffff;
  border-radius: 24px; /* 极其圆润的高级边角 */
  box-shadow: 0 20px 50px rgba(0, 0, 0, 0.04);
  display: flex;
  overflow: hidden;
}

/* 💙 左侧科技感渐变区 */
.login-left {
  width: 45%;
  background-image: url('../assets/images/login_bg_3d.png');
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
  display: flex;
  align-items: flex-end;
  justify-content: center;
  padding: 40px;
  position: relative;
}
.brand-wrapper {
  text-align: left;
  width: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
}
.illustration-img {
  width: 85%;
  transform: translateY(-10px);
  filter: drop-shadow(0 15px 25px rgba(64, 159, 255, 0.2));
}
.brand-text-box {
  width: 85%;
  margin-top: 20px;
}
.brand-title {
  color: #1e293b;
  font-size: 24px;
  font-weight: 700;
  margin: 0 0 8px 0;
}
.brand-desc {
  color: #64748b;
  font-size: 14px;
  margin: 0;
}
.brand-line {
  width: 30px;
  height: 4px;
  background: #409eff;
  border-radius: 2px;
  margin-top: 12px;
}

/* ⚪ 右侧表单白卡区 */
.login-right {
  width: 55%;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 60px;
}
.form-wrapper {
  width: 100%;
  max-width: 380px;
}

/* Logo 头部排版 */
.form-header {
  margin-bottom: 35px;
}
.logo-area {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 6px;
}
.logo-icon {
  font-size: 28px;
}
.logo-title {
  font-size: 26px;
  font-weight: 700;
  color: #1e293b;
  letter-spacing: -0.5px;
}
.form-subtitle {
  font-size: 14px;
  color: #94a3b8;
  margin: 0;
}

/* 表单细节美化 */
:deep(.el-form-item__label) {
  font-weight: 500 !important;
  color: #334155 !important;
  padding-bottom: 6px !important;
}
:deep(.el-input__wrapper) {
  border-radius: 8px !important; /* 彻底告别直角，输入框圆润化 */
  background-color: #f8fafc !important;
  box-shadow: 0 0 0 1px #e2e8f0 inset !important;
}
:deep(.el-input__wrapper.is-focus) {
  box-shadow: 0 0 0 1px #409eff inset !important;
  background-color: #ffffff !important;
}

/* 记住我 与 忘记密码 布局 */
.form-actions {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-top: -8px;
}
.forget-pwd {
  font-size: 14px;
  color: #409eff;
  cursor: pointer;
}
.forget-pwd:hover {
  text-decoration: underline;
}

/* 蓝色主按钮 */
.submit-btn {
  width: 100%;
  height: 44px;
  border-radius: 8px !important;
  font-size: 15px;
  font-weight: 600;
  background: #409eff !important;
  border: none !important;
  box-shadow: 0 4px 12px rgba(64, 159, 255, 0.3);
  transition: all 0.2s ease;
}
.submit-btn:hover {
  transform: translateY(-1px);
  box-shadow: 0 6px 16px rgba(64, 159, 255, 0.4);
}

/* 底部跳转 */
.go-register {
  text-align: center;
  margin-top: 25px;
  font-size: 13px;
  color: #64748b;
  position: relative;
}
/* 设计稿里两侧的淡淡装饰横线 */
.go-register::before, .go-register::after {
  content: "";
  position: absolute;
  top: 50%;
  width: 20%;
  height: 1px;
  background: #e2e8f0;
}
.go-register::before { left: 0; }
.go-register::after { right: 0; }

.go-register span {
  color: #409eff;
  cursor: pointer;
  font-weight: 600;
}

/* 版权脚标 */
.footer-copyright {
  margin-top: 24px;
  font-size: 12px;
  color: #94a3b8;
  letter-spacing: 0.5px;
}
</style>