import { Injectable } from '@angular/core';
import { HttpHeaders } from '@angular/common/http';
import { enviroment } from '../../environments/enviroment';
@Injectable({
  providedIn: 'root'
})
export class ApiService {

  private baseUrl = enviroment.apiUrl;
  getBaseUrl(): string {
    return this.baseUrl;
  }

  getheaders(): HttpHeaders{
    return new HttpHeaders({
      'Content-Type': 'Application/json'
    })
  }
}
