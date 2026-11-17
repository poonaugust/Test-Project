import { defineStore } from 'pinia';
import { ref } from 'vue';
import { extractApiErrorMessage } from '@/services/httpClient';
import { productService } from '@/services/productService';
import type {
  Product,
  ProductQueryParams,
  ProductSummary,
  UpsertProductPayload,
} from '@/types/product';

export const useProductStore = defineStore('products', () => {
  const items = ref<Product[]>([]);
  const totalCount = ref(0);
  const summary = ref<ProductSummary>({
    totalProducts: 0,
    activeProducts: 0,
    lowStockProducts: 0,
    totalInventoryValue: 0,
  });

  const loading = ref(false);
  const saving = ref(false);
  const error = ref<string | null>(null);

  const lastQuery = ref<ProductQueryParams>({
    page: 1,
    pageSize: 10,
    sortBy: 'createdAtUtc',
    sortDescending: true,
  });

  async function fetchProducts(params?: ProductQueryParams) {
    loading.value = true;
    error.value = null;
    const merged: ProductQueryParams = { ...lastQuery.value, ...params };
    lastQuery.value = merged;

    try {
      const result = await productService.getAll(merged);
      items.value = result.items;
      totalCount.value = result.totalCount;
    } catch (err) {
      error.value = extractApiErrorMessage(err);
      throw err;
    } finally {
      loading.value = false;
    }
  }

  async function fetchSummary() {
    try {
      summary.value = await productService.getSummary();
    } catch (err) {
      error.value = extractApiErrorMessage(err);
    }
  }

  async function createProduct(payload: UpsertProductPayload): Promise<Product> {
    saving.value = true;
    error.value = null;
    try {
      const created = await productService.create(payload);
      await Promise.all([fetchProducts(), fetchSummary()]);
      return created;
    } catch (err) {
      error.value = extractApiErrorMessage(err);
      throw err;
    } finally {
      saving.value = false;
    }
  }

  async function updateProduct(id: string, payload: UpsertProductPayload): Promise<Product> {
    saving.value = true;
    error.value = null;
    try {
      const updated = await productService.update(id, payload);
      await Promise.all([fetchProducts(), fetchSummary()]);
      return updated;
    } catch (err) {
      error.value = extractApiErrorMessage(err);
      throw err;
    } finally {
      saving.value = false;
    }
  }

  async function deleteProduct(id: string): Promise<void> {
    saving.value = true;
    error.value = null;
    try {
      await productService.remove(id);
      await Promise.all([fetchProducts(), fetchSummary()]);
    } catch (err) {
      error.value = extractApiErrorMessage(err);
      throw err;
    } finally {
      saving.value = false;
    }
  }

  return {
    items,
    totalCount,
    summary,
    loading,
    saving,
    error,
    lastQuery,
    fetchProducts,
    fetchSummary,
    createProduct,
    updateProduct,
    deleteProduct,
  };
});
