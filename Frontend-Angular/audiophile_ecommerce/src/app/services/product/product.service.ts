import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product } from '../../models/product';
import { ApiService } from '../api/api.service';
@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private apiUrl = '';

  constructor(private http: HttpClient, private apiServices: ApiService) { 
    this.apiUrl = this.apiServices.getBaseUrl + '/products';
  }

  getAll(): Observable<Product[]> {
    return this.http.get<Product[]>(this.apiUrl);
  }

  getProductById(id: number): Observable<Product>{
    return this.http.get<Product>(`${this.apiUrl}/${id}`);
  }

  getProductsByCategory(category: string): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.apiUrl}/category/${category}`);
  }

  createNewProduct(product: Product): Observable<Product>{
    return this.http.post<Product>(this.apiUrl, product);
  }

  updateProduct (product:Product): Observable<boolean> {
    return this.http.put<boolean> (`${this.apiUrl}/${product.id}`, product)
  }

  deleteProduct (id: number): Observable<boolean>{
    return this.http.delete<boolean>(`${this.apiUrl}/${id}`)
  }

}
