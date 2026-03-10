import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { ReactiveFormsModule, FormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ProductService } from '../services/product.service';
import { CategoryService } from '../services/category.service';
import { Category } from '../models/category.model';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, RouterLink],
  templateUrl: './product-form.component.html',
  styleUrl: './product-form.component.css'
})
export class ProductFormComponent implements OnInit {
  form!: FormGroup;
  categories = signal<Category[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);
  isEdit = signal(false);
  id = signal<number | null>(null);

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private categoryService: CategoryService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      description: [''],
      sku: ['', [Validators.required, Validators.maxLength(50)]],
      price: [0, [Validators.required, Validators.min(0)]],
      quantity: [0, [Validators.required, Validators.min(0)]],
      categoryId: [null as number | null]
    });
  }

  ngOnInit(): void {
    this.categoryService.getAll().subscribe((list) => {
      if (list) this.categories.set(list);
    });
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      const id = +idParam;
      this.id.set(id);
      this.isEdit.set(true);
      this.loading.set(true);
      this.productService.getById(id).subscribe((p) => {
        this.loading.set(false);
        if (p) {
          this.form.patchValue({
            name: p.name,
            description: p.description ?? '',
            sku: p.sku,
            price: p.price,
            quantity: p.quantity,
            categoryId: p.categoryId
          });
        } else {
          this.error.set('Product not found');
        }
      });
    }
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const raw = this.form.getRawValue();
    const dto = {
      name: raw.name,
      description: raw.description || null,
      sku: raw.sku,
      price: +raw.price,
      quantity: +raw.quantity,
      categoryId: raw.categoryId === '' || raw.categoryId == null ? null : +raw.categoryId
    };
    this.loading.set(true);
    this.error.set(null);
    const id = this.id();
    if (id != null) {
      this.productService.update(id, dto).subscribe((result) => {
        this.loading.set(false);
        if (result) this.router.navigate(['/products']);
        else this.error.set('Failed to update product');
      });
    } else {
      this.productService.create(dto).subscribe((result) => {
        this.loading.set(false);
        if (result) this.router.navigate(['/products']);
        else this.error.set('Failed to create product');
      });
    }
  }
}
