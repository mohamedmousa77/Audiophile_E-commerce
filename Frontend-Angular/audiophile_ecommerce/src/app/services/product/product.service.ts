import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product } from '../../models/product';
import { ApiService } from '../api/api.service';
@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private apiUrl = '';

  constructor(private http: HttpClient, private apiServices: ApiService) { 
    this.apiUrl = this.apiServices.getBaseUrl() + '/products';
  }

  getAll(): Observable<Product[]> {
    return this.http.get<Product[]>(this.apiUrl);
  }

  getProductById(id: number): Observable<Product>{
    let product = this.http.get<Product>(`${this.apiUrl}/${id}`);
    console.log(`The product received from the api is: ${product}`);
    
    return product;
  }

  getProductsByCategory(category: string): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.apiUrl}/category/${category}`,
      {headers: this.apiServices.getheaders()}
    );
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

  getFilteredProducts(isPromotion: boolean, isNew: boolean): Observable<Product[]> {   
    console.log('filterd products called: isPromotion product = '+isPromotion+`is new = ${isNew}`);
    let params = new HttpParams();

      if (isNew) {
        params = params.set('isNew', isNew);
        return this.http.get<Product[]>(`${this.apiUrl}/filter`, { params })
    }

    if (isPromotion) {
      params = params.set('isPromotion', isPromotion);
      return this.http.get<Product[]>(`${this.apiUrl}/filter`,{ params });
      
    }
    return this.http.get<Product[]>(`${this.apiUrl}/filter`)
  }
}
