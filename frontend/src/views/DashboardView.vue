<template>
  <div>
    <div class="d-flex flex-wrap align-center justify-space-between mb-6 ga-3">
      <div>
        <h1 class="text-h5 font-weight-bold">System Overview</h1>
        <p class="text-body-2 text-medium-emphasis mt-1">
          Clean Architecture + CQRS + Mediator (.NET 10, EF Core, PostgreSQL &amp; Vue 3 / Vuetify)
        </p>
      </div>
      <v-btn color="primary" prepend-icon="mdi-package-variant-closed" to="/products">
        Manage Products
      </v-btn>
    </div>

    <v-row>
      <v-col cols="12" sm="6" lg="3">
        <v-card class="pa-4">
          <div class="d-flex align-center justify-space-between">
            <div>
              <div class="text-caption text-medium-emphasis">Total Products</div>
              <div class="text-h4 font-weight-bold mt-1">{{ store.summary.totalProducts }}</div>
            </div>
            <v-avatar color="primary" variant="tonal" size="48">
              <v-icon>mdi-package-variant</v-icon>
            </v-avatar>
          </div>
        </v-card>
      </v-col>

      <v-col cols="12" sm="6" lg="3">
        <v-card class="pa-4">
          <div class="d-flex align-center justify-space-between">
            <div>
              <div class="text-caption text-medium-emphasis">Active Catalog</div>
              <div class="text-h4 font-weight-bold mt-1">{{ store.summary.activeProducts }}</div>
            </div>
            <v-avatar color="success" variant="tonal" size="48">
              <v-icon>mdi-check-circle-outline</v-icon>
            </v-avatar>
          </div>
        </v-card>
      </v-col>

      <v-col cols="12" sm="6" lg="3">
        <v-card class="pa-4">
          <div class="d-flex align-center justify-space-between">
            <div>
              <div class="text-caption text-medium-emphasis">Low Stock (&le; 10)</div>
              <div class="text-h4 font-weight-bold mt-1">{{ store.summary.lowStockProducts }}</div>
            </div>
            <v-avatar color="warning" variant="tonal" size="48">
              <v-icon>mdi-alert-outline</v-icon>
            </v-avatar>
          </div>
        </v-card>
      </v-col>

      <v-col cols="12" sm="6" lg="3">
        <v-card class="pa-4">
          <div class="d-flex align-center justify-space-between">
            <div>
              <div class="text-caption text-medium-emphasis">Inventory Value</div>
              <div class="text-h4 font-weight-bold mt-1">
                {{ formatCurrency(store.summary.totalInventoryValue) }}
              </div>
            </div>
            <v-avatar color="secondary" variant="tonal" size="48">
              <v-icon>mdi-cash-multiple</v-icon>
            </v-avatar>
          </div>
        </v-card>
      </v-col>
    </v-row>

    <v-card class="mt-6 pa-5">
      <v-card-title class="px-0 pt-0 text-subtitle-1 font-weight-bold">
        Architecture Highlights
      </v-card-title>
      <v-row class="mt-1">
        <v-col v-for="item in layers" :key="item.title" cols="12" md="6" lg="3">
          <v-sheet border rounded="lg" class="pa-4 h-100">
            <div class="d-flex align-center ga-2 mb-2">
              <v-icon color="primary">{{ item.icon }}</v-icon>
              <span class="font-weight-bold">{{ item.title }}</span>
            </div>
            <div class="text-body-2 text-medium-emphasis">{{ item.description }}</div>
          </v-sheet>
        </v-col>
      </v-row>
    </v-card>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue';
import { useProductStore } from '@/stores/productStore';

const store = useProductStore();

const layers = [
  {
    icon: 'mdi-domain',
    title: 'Domain Layer',
    description: 'Pure entities, invariants, domain events, and Result<T> error pattern with zero dependencies.',
  },
  {
    icon: 'mdi-swap-horizontal',
    title: 'Application (CQRS + Mediator)',
    description: 'Commands & Queries dispatched via IMediator with FluentValidation and logging pipeline behaviors.',
  },
  {
    icon: 'mdi-database-outline',
    title: 'Infrastructure (EF Core + PostgreSQL)',
    description: 'Separate Command repository & AsNoTracking Query read store, audit interceptor, and auto-seeding.',
  },
  {
    icon: 'mdi-vuejs',
    title: 'Presentation (ASP.NET Core + Vuetify)',
    description: 'Swagger OpenAPI, RFC 9457 ProblemDetails, health checks, and modular Vue 3 / Vuetify 3 UI.',
  },
];

function formatCurrency(value: number): string {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'THB',
    maximumFractionDigits: 0,
  }).format(value);
}

onMounted(() => {
  store.fetchSummary();
});
</script>
