export interface Product {
  id: number;
  name: string;
  description: string | null;
  sku: string;
  price: number;
  quantity: number;
  categoryId: number | null;
  createdAt: string;
  updatedAt: string;
}

export interface CreateUpdateProduct {
  name: string;
  description: string | null;
  sku: string;
  price: number;
  quantity: number;
  categoryId: number | null;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface ProductQueryParams {
  page?: number;
  pageSize?: number;
  categoryId?: number | null;
  search?: string | null;
}
