const routes = [
  {
    path: '/',
    component: () => import('layouts/MainLayout.vue'),
    children: [
      // Brand Page (default)
      {
        path: '/',
        name: 'brands',
        component: () => import('pages/BrandPage.vue'),
      },
      {
        path: '/cart',
        name: 'cart',
        component: () => import('pages/CartPage.vue'),
      },
      {
        path: '/orderhistory',
        name: 'orderhistory',
        component: () => import('pages/OrderHistoryPage.vue'),
      },
      {
        path: '/register',
        name: 'register',
        component: () => import('pages/RegisterPage.vue'),
      },
      {
        path: '/login',
        name: 'login',
        component: () => import('pages/LoginPage.vue'),
      },
      {
        path: '/logout',
        name: 'logout',
        component: () => import('pages/LogoutPage.vue'),
      },
      {
        path: '/branches',
        name: 'branch',
        component: () => import('pages/BranchPage.vue'),
      },
    ],
  },

  // Always leave this as last one (404)
  {
    path: '/:catchAll(.*)*',
    component: () => import('pages/ErrorNotFound.vue'),
  },
]

export default routes
