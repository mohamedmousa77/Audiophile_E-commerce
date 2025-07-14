import { Injectable } from '@angular/core';
import { ApiService } from '../api/api.service';
import { HttpClient } from '@angular/common/http';
import { Order } from '../../models/order';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private baseUrl = '';

  constructor(private api: ApiService, private http: HttpClient) { 
    this.baseUrl = this.api.getBaseUrl() + '/orders';
  }

  getAllOrders(): Observable<Order[]> {
    return this.http.get<Order[]> (this.baseUrl);
  }

  createOrder(order: Order): Observable<Order> {
    return this.http.post<Order>(this.baseUrl, order);
  }

  getOrderById(id: number): Observable<Order> {
    return this.http.get<Order>(`${this.baseUrl}/${id}`);
  }

  updateOrder(order: Order): Observable<boolean> {
    return this.http.put<boolean> (`${this.baseUrl}/${order.id}`, order);
  }

  deleteOrder(id: number): Observable<boolean>{
    return this.http.delete<boolean>(`${this.baseUrl}/${id}`);
  }
}
