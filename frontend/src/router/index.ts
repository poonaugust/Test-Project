import { createRouter, createWebHistory } from 'vue-router';
import DashboardView from '@/views/DashboardView.vue';
import ProductsView from '@/views/ProductsView.vue';

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      name: 'dashboard',
      component: DashboardView,
      meta: { title: 'Dashboard' },
    },
    {
      path: '/products',
      name: 'products',
      component: ProductsView,
      meta: { title: 'Products' },
    },
  ],
});

router.afterEach((to) => {
  const title = to.meta?.title ? `${to.meta.title} — TestProject` : 'TestProject';
  document.title = title;
});

export default router;
