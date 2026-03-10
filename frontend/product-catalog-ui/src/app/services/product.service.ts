import { Injectable } from '@angular/core';
import { Observable, catchError, map, of } from 'rxjs';
import { ApiService } from './api.service';
import {
  Product,
  CreateUpdateProduct,
  PagedResult,
  ProductQueryParams
} from '../models/product.model';

@Injectable({ providedIn: 'root' })
export class ProductService {
  constructor(private api: ApiService) {}

  getPage(params: ProductQueryParams): Observable<PagedResult<Product> | null> {
    return this.api
      .get<PagedResult<Product>>('/products', {
        page: params.page ?? 1,
        pageSize: params.pageSize ?? 10,
        categoryId: params.categoryId ?? undefined,
        search: params.search ?? undefined
      })
      .pipe(catchError(() => of(null)));
  }

  getById(id: number): Observable<Product | null> {
    return this.api
      .get<Product>(`/products/${id}`)
      .pipe(catchError(() => of(null)));
  }

  create(dto: CreateUpdateProduct): Observable<Product | null> {
    return this.api
      .post<Product>('/products', dto)
      .pipe(catchError(() => of(null)));
  }

  update(id: number, dto: CreateUpdateProduct): Observable<Product | null> {
    return this.api
      .put<Product>(`/products/${id}`, dto)
      .pipe(catchError(() => of(null)));
  }

  delete(id: number): Observable<boolean> {
    return this.api.delete(`/products/${id}`).pipe(
      map(() => true),
      catchError(() => of(false))
    );
  }
}
