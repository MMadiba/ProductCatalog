import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly baseUrl: string;

  constructor(private http: HttpClient) {
    this.baseUrl = environment?.apiUrl ?? '/api';
  }

  get<T>(path: string, params?: Record<string, string | number | null | undefined>) {
    const p: Record<string, string> = {};
    if (params) {
      for (const [k, v] of Object.entries(params)) {
        if (v !== undefined && v !== null && v !== '') p[k] = String(v);
      }
    }
    return this.http.get<T>(`${this.baseUrl}${path}`, { params: p });
  }

  post<T>(path: string, body: unknown) {
    return this.http.post<T>(`${this.baseUrl}${path}`, body);
  }

  put<T>(path: string, body: unknown) {
    return this.http.put<T>(`${this.baseUrl}${path}`, body);
  }

  delete(path: string) {
    return this.http.delete(`${this.baseUrl}${path}`);
  }
}
