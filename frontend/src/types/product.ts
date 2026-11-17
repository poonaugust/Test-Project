export type ProductStatus = 'Draft' | 'Active' | 'Archived';

export interface Product {
  id: string;
  sku: string;
  name: string;
  description: string | null;
  category: string;
  price: number;
  stockQuantity: number;
  status: ProductStatus;
  createdAtUtc: string;
  updatedAtUtc: string | null;
}

export interface ProductSummary {
  totalProducts: number;
  activeProducts: number;
  lowStockProducts: number;
  totalInventoryValue: number;
}

export interface ProductQueryParams {
  search?: string;
  category?: string;
  status?: ProductStatus | null;
  sortBy?: string;
  sortDescending?: boolean;
  page?: number;
  pageSize?: number;
}

export interface UpsertProductPayload {
  sku: string;
  name: string;
  description: string | null;
  category: string;
  price: number;
  stockQuantity: number;
  status: ProductStatus;
}
