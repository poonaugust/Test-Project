<template>
  <div>
    <div class="d-flex flex-wrap align-center justify-space-between mb-5 ga-3">
      <div>
        <h1 class="text-h5 font-weight-bold">Products</h1>
        <p class="text-body-2 text-medium-emphasis mt-1">
          Full CRUD powered by CQRS Commands &amp; Queries over PostgreSQL.
        </p>
      </div>
      <v-btn color="primary" prepend-icon="mdi-plus" @click="openCreate">
        New Product
      </v-btn>
    </div>

    <v-card class="pa-4 mb-4">
      <v-row dense align="center">
        <v-col cols="12" md="5">
          <v-text-field
            v-model="search"
            label="Search by SKU, name, or category"
            prepend-inner-icon="mdi-magnify"
            clearable
            hide-details
            @update:model-value="onFilterChanged"
          />
        </v-col>
        <v-col cols="12" sm="6" md="3">
          <v-select
            v-model="selectedStatus"
            label="Status"
            :items="statusFilterOptions"
            clearable
            hide-details
            @update:model-value="onFilterChanged"
          />
        </v-col>
        <v-col cols="12" sm="6" md="4" class="d-flex justify-end ga-2">
          <v-btn
            variant="outlined"
            prepend-icon="mdi-refresh"
            :loading="store.loading"
            @click="reload"
          >
            Refresh
          </v-btn>
        </v-col>
      </v-row>
    </v-card>

    <v-card>
      <v-data-table-server
        v-model:items-per-page="itemsPerPage"
        v-model:page="page"
        :headers="headers"
        :items="store.items"
        :items-length="store.totalCount"
        :loading="store.loading"
        @update:options="onTableOptionsUpdate"
      >
        <template #item.sku="{ item }">
          <span class="font-weight-medium text-primary">{{ item.sku }}</span>
        </template>

        <template #item.price="{ item }">
          {{ formatPrice(item.price) }}
        </template>

        <template #item.stockQuantity="{ item }">
          <v-chip
            size="small"
            :color="item.stockQuantity <= 10 ? 'warning' : 'default'"
            variant="tonal"
          >
            {{ item.stockQuantity }}
          </v-chip>
        </template>

        <template #item.status="{ item }">
          <v-chip size="small" :color="statusColor(item.status)" variant="flat">
            {{ item.status }}
          </v-chip>
        </template>

        <template #item.actions="{ item }">
          <div class="d-flex ga-1 justify-end">
            <v-btn
              icon="mdi-pencil-outline"
              size="small"
              variant="text"
              color="primary"
              title="Edit"
              @click="openEdit(item)"
            />
            <v-btn
              icon="mdi-delete-outline"
              size="small"
              variant="text"
              color="error"
              title="Delete"
              @click="openDelete(item)"
            />
          </div>
        </template>
      </v-data-table-server>
    </v-card>

    <ProductFormDialog
      ref="formDialogRef"
      v-model="formDialogOpen"
      :product="editingProduct"
      :saving="store.saving"
      @save="handleSave"
    />

    <ConfirmDeleteDialog
      v-model="deleteDialogOpen"
      :product="deletingProduct"
      :loading="store.saving"
      @confirm="handleDelete"
    />

    <v-snackbar v-model="snackbar.open" :color="snackbar.color" timeout="3500">
      {{ snackbar.message }}
    </v-snackbar>
  </div>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue';
import ConfirmDeleteDialog from '@/components/ConfirmDeleteDialog.vue';
import ProductFormDialog from '@/components/ProductFormDialog.vue';
import { extractApiErrorMessage } from '@/services/httpClient';
import { useProductStore } from '@/stores/productStore';
import type { Product, ProductStatus, UpsertProductPayload } from '@/types/product';

const store = useProductStore();

const search = ref('');
const selectedStatus = ref<ProductStatus | null>(null);
const page = ref(1);
const itemsPerPage = ref(10);
const sortBy = ref('createdAtUtc');
const sortDescending = ref(true);

const formDialogOpen = ref(false);
const editingProduct = ref<Product | null>(null);
const formDialogRef = ref<InstanceType<typeof ProductFormDialog> | null>(null);

const deleteDialogOpen = ref(false);
const deletingProduct = ref<Product | null>(null);

const snackbar = reactive({
  open: false,
  message: '',
  color: 'success',
});

let searchDebounceTimer: ReturnType<typeof setTimeout> | null = null;

const statusFilterOptions: ProductStatus[] = ['Active', 'Draft', 'Archived'];

const headers = [
  { title: 'SKU', key: 'sku', sortable: true },
  { title: 'Name', key: 'name', sortable: true },
  { title: 'Category', key: 'category', sortable: true },
  { title: 'Price', key: 'price', sortable: true, align: 'end' as const },
  { title: 'Stock', key: 'stockQuantity', sortable: true, align: 'center' as const },
  { title: 'Status', key: 'status', sortable: true, align: 'center' as const },
  { title: 'Actions', key: 'actions', sortable: false, align: 'end' as const },
];

function statusColor(status: ProductStatus): string {
  switch (status) {
    case 'Active':
      return 'success';
    case 'Draft':
      return 'info';
    case 'Archived':
      return 'grey';
  }
}

function formatPrice(value: number): string {
  return new Intl.NumberFormat('en-US', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(value);
}

async function reload() {
  try {
    await store.fetchProducts({
      search: search.value,
      status: selectedStatus.value,
      sortBy: sortBy.value,
      sortDescending: sortDescending.value,
      page: page.value,
      pageSize: itemsPerPage.value,
    });
  } catch (err) {
    showSnackbar(extractApiErrorMessage(err), 'error');
  }
}

function onFilterChanged() {
  page.value = 1;
  if (searchDebounceTimer) clearTimeout(searchDebounceTimer);
  searchDebounceTimer = setTimeout(() => {
    reload();
  }, 300);
}

function onTableOptionsUpdate(options: {
  page: number;
  itemsPerPage: number;
  sortBy: Array<{ key: string; order?: 'asc' | 'desc' }>;
}) {
  page.value = options.page;
  itemsPerPage.value = options.itemsPerPage;
  if (options.sortBy.length > 0) {
    sortBy.value = options.sortBy[0].key;
    sortDescending.value = options.sortBy[0].order === 'desc';
  } else {
    sortBy.value = 'createdAtUtc';
    sortDescending.value = true;
  }
  reload();
}

function openCreate() {
  editingProduct.value = null;
  formDialogOpen.value = true;
}

function openEdit(product: Product) {
  editingProduct.value = product;
  formDialogOpen.value = true;
}

function openDelete(product: Product) {
  deletingProduct.value = product;
  deleteDialogOpen.value = true;
}

async function handleSave(payload: UpsertProductPayload) {
  try {
    if (editingProduct.value) {
      await store.updateProduct(editingProduct.value.id, payload);
      showSnackbar('Product updated successfully.', 'success');
    } else {
      await store.createProduct(payload);
      showSnackbar('Product created successfully.', 'success');
    }
    formDialogOpen.value = false;
  } catch (err) {
    formDialogRef.value?.setError(extractApiErrorMessage(err));
  }
}

async function handleDelete() {
  if (!deletingProduct.value) return;
  try {
    await store.deleteProduct(deletingProduct.value.id);
    deleteDialogOpen.value = false;
    showSnackbar('Product deleted.', 'success');
  } catch (err) {
    showSnackbar(extractApiErrorMessage(err), 'error');
  }
}

function showSnackbar(message: string, color: string) {
  snackbar.message = message;
  snackbar.color = color;
  snackbar.open = true;
}
</script>
