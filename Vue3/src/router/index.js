import { createRouter, createWebHistory } from 'vue-router'

const routes = [
  { path: '/', redirect: '/login' },
  { path: '/register', name: 'AccountRegister', component: () => import('../views/Register.vue') },
  { path: '/login', name: 'AccountLogin', component: () => import('../views/Login.vue') },
  { path: '/index', name: 'Index', component: () => import('../views/Index.vue'),
    children: 
    [
      { path: 'user', name: 'UserList', component: () => import('../views/user/UserList.vue') },
      { path: 'goods', name:'GoodsList', component: () => import('../views/goods/GoodsList.vue')},
      { path: 'supplier', name:'SupplierList', component: () => import('../views/supplier/SupplierList.vue')},
      { path: 'company', name:'CompanyList', component: () => import('../views/company/CompanyList.vue')},
      { path:'goodscategory', name:'GoodsCategoryList', component:() => import('../views/goodscategory/GoodsCategoryList.vue')},
      { path:'location', name:'Location', component:() => import('../views/location/LocationList.vue')}
    ]
  }
]

// 【第一步】先创建 router 实例
const router = createRouter({
  history: createWebHistory(),
  routes
})

// 【第二步】然后再设置导航守卫 (此时 router 已经存在了)
router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('token')
  if(to.path == "/login") {
    next()
    return
  }
  if(!token) {
    next("/login")
    return
  }
  next()
})

// 【第三步】最后再导出
export default router