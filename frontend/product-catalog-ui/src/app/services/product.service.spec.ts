import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ProductService } from './product.service';
import { ApiService } from './api.service';

describe('ProductService', () => {
  let service: ProductService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ProductService, ApiService]
    });
    service = TestBed.inject(ProductService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('getPage should request GET /products with query params', () => {
    service.getPage({ page: 1, pageSize: 10 }).subscribe((result) => {
      expect(result).toBeTruthy();
      expect(result!.items).toEqual([]);
      expect(result!.totalCount).toBe(0);
    });
    const req = httpMock.expectOne((r) => r.url.includes('/products') && r.method === 'GET');
    expect(req.request.params.get('page')).toBe('1');
    expect(req.request.params.get('pageSize')).toBe('10');
    req.flush({ items: [], totalCount: 0, page: 1, pageSize: 10 });
  });
});
