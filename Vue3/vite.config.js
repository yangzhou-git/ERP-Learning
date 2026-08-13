import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import path from 'path' // 引入 path 模块

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      // 告诉 Vite：看到 @ 符号，就把它替换成 src 目录的绝对路径
      '@': path.resolve(__dirname, './src')
    }
  }
})