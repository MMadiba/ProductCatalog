import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'products', pathMatch: 'full' },
  { path: 'products', loadComponent: () => import('./product-list/product-list.component').then(m => m.ProductListComponent) },
  { path: 'products/new', loadComponent: () => import('./product-form/product-form.component').then(m => m.ProductFormComponent) },
  { path: 'products/edit/:id', loadComponent: () => import('./product-form/product-form.component').then(m => m.ProductFormComponent) },
  { path: 'categories', loadComponent: () => import('./categories/categories.component').then(m => m.CategoriesComponent) }
];
