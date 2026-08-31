<template>
    <div class="index-page">
        <div class="header">
            <div>智阅书联ERP管理系统</div>
             <el-dropdown>
                <span class="user-name">
                    {{UserName}}
                    <el-icon>
                        <ArrowDown />
                    </el-icon>
                </span>
                <template #dropdown>
                    <el-dropdown-menu>
                        <el-dropdown-item @click="Logout">
                            退出登录
                        </el-dropdown-item>
                    </el-dropdown-menu>
                </template>
            </el-dropdown>
        </div>
    
     <div class="main-container">
            <!-- 左侧菜单 -->
            <div class="menu">
                <el-menu router class="el-menu">
                    <el-menu-item index="/index">
                        <el-icon><House /></el-icon>
                        <span>首页</span>
                    </el-menu-item>

                    <!-- 资料管理 -->
                    <el-sub-menu index="base">
                        <template #title>
                            <el-icon><Folder /></el-icon>
                            <span>资料管理</span>
                        </template>


                        <el-menu-item index="/index/company">
                            <el-icon><OfficeBuilding /></el-icon>
                            <span>公司资料</span>
                        </el-menu-item>

                        <!-- 站点资料 -->
                        <el-menu-item index="/index/location">
                            <el-icon><Location /></el-icon>
                            <span>站点资料</span>
                        </el-menu-item>

                        <el-menu-item index="/index/goodsCategory">
                            <el-icon><Collection /></el-icon>
                            <span>商品分类资料</span>
                        </el-menu-item>

                        <el-menu-item index="/index/unit">
                            <el-icon><ScaleToOriginal /></el-icon>
                            <span>单位管理</span>
                        </el-menu-item>

                        <el-menu-item index="/index/goods">
                            <el-icon><Goods /></el-icon>
                            <span>商品资料</span>
                        </el-menu-item>


                        <el-menu-item index="/index/supplier">
                            <el-icon><Shop /></el-icon>
                            <span>供应商资料</span>
                        </el-menu-item>


                    </el-sub-menu>

                    <el-menu-item index="/order">
                        <el-icon><ShoppingCart /></el-icon>
                        <span>订单管理</span>
                    </el-menu-item>

                    <el-sub-menu index="system">
                        <template #title>
                            <el-icon><Setting /></el-icon>
                            <span>系统设置</span>
                        </template>
                        <el-menu-item index="/index/user">
                            <el-icon><User /></el-icon>
                            <span>员工管理</span>
                        </el-menu-item>
                    </el-sub-menu>
                </el-menu>
            </div>

            <!-- 内容区域 -->
            <div class="content">
                <router-view />
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { House,User,Goods,ShoppingCart,Setting,ArrowDown,Folder,OfficeBuilding,Collection,Shop,Location,ScaleToOriginal } from '@element-plus/icons-vue'

const router = useRouter()
const UserName = ref(localStorage.getItem('userName') || '')
//退出登陆
function Logout(){
    localStorage.removeItem('token')
    localStorage.removeItem('userName')
    router.push('/login')
}
</script>

<style scoped>
.header{
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 0 20px;
    height: 60px;
    background-color: #409EFF;
    color: white;
    font-size: 18px;
}
.main-container {
    display: flex;
    height: calc(100vh - 60px);
    gap:30px;
}
.menu{
    width:220px;
    background-color:#f4f7fb;
}
.el-menu{
    height:100%;
    border-right:none;
}
.content{
    flex: 1;
    padding: 20px;
    background-color: #ffffff;
}
</style>