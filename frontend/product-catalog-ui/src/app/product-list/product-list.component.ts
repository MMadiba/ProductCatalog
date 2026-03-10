import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../services/product.service';
import { CategoryService } from '../services/category.service';
import { Product } from '../models/product.model';
import { Category } from '../models/category.model';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.css'
})
export class ProductListComponent implements OnInit {
  products = signal<Product[]>([]);
  categories = signal<Category[]>([]);
  totalCount = signal(0);
  page = signal(1);
  pageSize = signal(10);
  searchTerm = signal('');
  categoryId = signal<number | null>(null);
  loading = signal(false);
  error = signal<string | null>(null);
  private searchDebounceHandle: number | null = null;

  totalPages = computed(() => Math.max(1, Math.ceil(this.totalCount() / this.pageSize())));

  constructor(
    private productService: ProductService,
    private categoryService: CategoryService
  ) {}

  ngOnInit(): void {
    this.categoryService.getAll().subscribe((list) => {
      if (list) this.categories.set(list);
    });
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.error.set(null);
    this.productService
      .getPage({
        page: this.page(),
        pageSize: this.pageSize(),
        categoryId: this.categoryId(),
        search: this.searchTerm() || undefined
      })
      .subscribe((result) => {
        this.loading.set(false);
        if (result) {
          this.products.set(result.items);
          this.totalCount.set(result.totalCount);
        } else {
          this.error.set('Failed to load products');
        }
      });
  }

  onSearchChange(term: string): void {
    this.searchTerm.set(term);
    this.page.set(1);
    if (this.searchDebounceHandle !== null) {
      window.clearTimeout(this.searchDebounceHandle);
    }
    this.searchDebounceHandle = window.setTimeout(() => {
      this.load();
    }, 300);
  }

  onSearch(): void {
    this.page.set(1);
    this.load();
  }

  onCategoryChange(): void {
    this.page.set(1);
    this.load();
  }

  goToPage(p: number): void {
    this.page.set(Math.max(1, Math.min(p, this.totalPages())));
    this.load();
  }

  getCategoryName(categoryId: number | null): string {
    if (categoryId == null) return '-';
    const c = this.categories().find((cat) => cat.id === categoryId);
    return c?.name ?? '-';
  }

  confirmDelete(product: Product): void {
    if (confirm(`Delete "${product.name}"?`)) {
      this.productService.delete(product.id).subscribe((ok) => {
          if (ok) {
          // Optimistically refresh local list so the UI updates immediately
          this.products.update((items) => items.filter((p) => p.id !== product.id));
          this.totalCount.update((c) => Math.max(0, c - 1));

          // If current page becomes empty and we're not on the first page, go back a page
          if (this.products().length === 0 && this.page() > 1) {
            this.goToPage(this.page() - 1);
          }
        } else {
          this.error.set('Failed to delete product');
        }
      });
    }
  }
}
