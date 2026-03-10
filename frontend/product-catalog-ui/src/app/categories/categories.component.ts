import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CategoryService } from '../services/category.service';
import { CategoryTreeNode } from '../models/category.model';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './categories.component.html',
  styleUrl: './categories.component.css'
})
export class CategoriesComponent implements OnInit {
  tree = signal<CategoryTreeNode[]>([]);
  form!: FormGroup;
  loading = signal(false);
  error = signal<string | null>(null);
  success = signal<string | null>(null);

  constructor(
    private fb: FormBuilder,
    private categoryService: CategoryService
  ) {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      description: [''],
      parentCategoryId: [null as number | null]
    });
  }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.categoryService.getTree().subscribe((result) => {
      if (result) this.tree.set(result);
      else this.error.set('Failed to load categories');
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const raw = this.form.getRawValue();
    this.loading.set(true);
    this.error.set(null);
    this.success.set(null);
    this.categoryService
      .create({
        name: raw.name,
        description: raw.description || null,
        parentCategoryId: raw.parentCategoryId === '' || raw.parentCategoryId == null ? null : +raw.parentCategoryId
      })
      .subscribe((result) => {
        this.loading.set(false);
        if (result) {
          this.success.set('Category created.');
          this.form.reset({ name: '', description: '', parentCategoryId: null });
          this.load();
        } else {
          this.error.set('Failed to create category');
        }
      });
  }

  flatNodes(nodes: CategoryTreeNode[], level = 0): { node: CategoryTreeNode; level: number }[] {
    let out: { node: CategoryTreeNode; level: number }[] = [];
    for (const node of nodes) {
      out.push({ node, level });
      if (node.children?.length) out = out.concat(this.flatNodes(node.children, level + 1));
    }
    return out;
  }
}
