import { Injectable } from '@angular/core';
import { Observable, catchError, of } from 'rxjs';
import { ApiService } from './api.service';
import { Category, CategoryTreeNode, CreateCategory } from '../models/category.model';

@Injectable({ providedIn: 'root' })
export class CategoryService {
  constructor(private api: ApiService) {}

  getAll(): Observable<Category[] | null> {
    return this.api.get<Category[]>('/categories').pipe(catchError(() => of(null)));
  }

  getTree(): Observable<CategoryTreeNode[] | null> {
    return this.api.get<CategoryTreeNode[]>('/categories/tree').pipe(catchError(() => of(null)));
  }

  create(dto: CreateCategory): Observable<Category | null> {
    return this.api.post<Category>('/categories', dto).pipe(catchError(() => of(null)));
  }
}
