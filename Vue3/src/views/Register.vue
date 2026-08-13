<template>
  <div class="auth-container">
    <h2 class="title">智阅书联 ERP</h2>
    <p class="subtitle">新账户注册中心</p>
    <hr />

    <!-- 🏗️ Element Plus 高级表单组件 -->
    <el-form :model="registerModel" label-position="top">
      
      <!-- 1. 用户名输入框 -->
      <el-form-item label="账 号">
        <el-input v-model="registerModel.userName" placeholder="请输入用于登录的账号" clearable/>
      </el-form-item>

      <!-- 2. 密码输入框 -->
      <el-form-item label="密 码">
        <el-input v-model="registerModel.password" type="password" placeholder="请输入密码" show-password/>
      </el-form-item>

      <!-- 3. 昵称输入框 -->
      <el-form-item label="昵 称">
        <el-input v-model="registerModel.nickName" placeholder="请输入你的工作花名/昵称" clearable/>
      </el-form-item>

      <!-- 4. 提交按钮 -->
      <el-form-item style="margin-top: 30px;">
        <el-button type="primary" style="width: 100%;" size="large" @click="handleRegister">
          立即注册账户
        </el-button>
      </el-form-item>

      <div class="go-login">
          已有账号？<span @click="router.push('/login')">立即登录</span>
      </div>
    </el-form>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { ElMessage } from 'element-plus'
import request from '../utils/request'
import { useRouter } from 'vue-router'

const router = useRouter()

// 💡 前端收集表单数据的“大沙盒”
const registerModel = ref({
  userName: '',
  password: '',
  nickName: ''
})

// 💡 当用户点击“立即注册”时触发的方法
const handleRegister = async () => {
    try{
        const res = await request.post('/Auth/Register', registerModel.value)
      if (res.success) {
      ElMessage.success('账户注册成功！')
      router.push('/login')
    } else {
      ElMessage.error('注册失败：' + res.message)
    }

  } catch (error) {
    console.error('页面捕捉到错误:', error)
  }
}
</script>

<style scoped>
.auth-container {
  max-width: 400px;
  margin: 60px auto;
  padding: 35px;
  border-radius: 16px;
  background: #ffffff;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.08);
  border: 1px solid #f0f0f0;
}
.title {
  text-align: center;
  margin-bottom: 5px;
  color: #409eff;
  font-size: 24px;
  font-weight: 600;
}
.subtitle {
  text-align: center;
  color: #909399;
  font-size: 14px;
  margin-top: 0;
}
hr {
  border: 0;
  border-top: 1px solid #f2f6fc;
  margin: 25px 0;
}
.go-login {
  width: 100%;
  text-align: center;
  margin-top: 15px;
  font-size: 13px;
  color: #606266;
}

.go-login span {
  color: #409eff;
  cursor: pointer;
  font-weight: 500;
}

.go-login span:hover {
  text-decoration: underline;
}
</style>