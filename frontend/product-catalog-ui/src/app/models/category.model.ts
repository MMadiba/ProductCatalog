export interface Category {
  id: number;
  name: string;
  description: string | null;
  parentCategoryId: number | null;
}

export interface CategoryTreeNode extends Category {
  children: CategoryTreeNode[];
}

export interface CreateCategory {
  name: string;
  description: string | null;
  parentCategoryId: number | null;
}
