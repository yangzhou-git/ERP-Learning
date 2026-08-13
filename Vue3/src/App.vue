<template>
  <router-view />
</template>

<script setup>
// 💡 这里现在干净得像镜子一样，什么都不需要写
</script>

<style>
/* 💡 注意：这里去掉 scoped，作为全系统的全局背景底色 */
body {
  margin: 0;
  padding: 0;
  background-color: #fafafa; /* 给整个 ERP 系统配一个高档的浅灰色背景 */
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;
}
</style>