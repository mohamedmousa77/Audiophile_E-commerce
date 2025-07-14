import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Cart } from '../../models/cart';
import { CartItemDTO } from '../../models/cartItemDTO';
import { ApiService } from '../api/api.service';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private apiUrl = '';

  constructor(private http: HttpClient, private api: ApiService) { 
    this.apiUrl = this.api.getBaseUrl + '/cart';
  }

  getCart(customerId: number): Observable<Cart> {
    return this.http.get<Cart>(`${this.apiUrl}/${customerId}`);
  }

  addToCart(customerId: number, item: CartItemDTO): Observable<boolean> {
    return this.http.post<boolean>(`${this.apiUrl}/${customerId}/add`, item);
  }

  clearCart(customerId: number) : Observable<boolean> {
    return this.http.delete<boolean> (`${this.apiUrl}/${customerId}/clear`);
  }

  removeItemFromCart(customerId: number, productId: number): Observable<boolean> {
    return this.http.delete<boolean>(`${this.apiUrl}/${customerId}/remove/${productId}`);
  }

  updateCartItem(customerId: number, productId: number, quantity: number) : Observable<boolean> {
    return this.http.put<boolean> (`${this.apiUrl}/${customerId}/update/${productId}`, quantity);
  }
}
