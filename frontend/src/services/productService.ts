import { httpClient } from '@/services/httpClient';
import type { PagedResult } from '@/types/api';
import type {
  Product,
  ProductQueryParams,
  ProductSummary,
  UpsertProductPayload,
} from '@/types/product';

const BASE_PATH = '/api/v1/products';

export const productService = {
  async getAll(params: ProductQueryParams): Promise<PagedResult<Product>> {
    const { data } = await httpClient.get<PagedResult<Product>>(BASE_PATH, {
      params: {
        search: params.search || undefined,
        category: params.category || undefined,
        status: params.status || undefined,
        sortBy: params.sortBy || 'createdAtUtc',
        sortDescending: params.sortDescending ?? true,
        page: params.page ?? 1,
        pageSize: params.pageSize ?? 10,
      },
    });
    return data;
  },

  async getSummary(): Promise<ProductSummary> {
    const { data } = await httpClient.get<ProductSummary>(`${BASE_PATH}/summary`);
    return data;
  },

  async getById(id: string): Promise<Product> {
    const { data } = await httpClient.get<Product>(`${BASE_PATH}/${id}`);
    return data;
  },

  async create(payload: UpsertProductPayload): Promise<Product> {
    const { data } = await httpClient.post<Product>(BASE_PATH, payload);
    return data;
  },

  async update(id: string, payload: UpsertProductPayload): Promise<Product> {
    const { data } = await httpClient.put<Product>(`${BASE_PATH}/${id}`, payload);
    return data;
  },

  async remove(id: string): Promise<void> {
    await httpClient.delete(`${BASE_PATH}/${id}`);
  },
};
