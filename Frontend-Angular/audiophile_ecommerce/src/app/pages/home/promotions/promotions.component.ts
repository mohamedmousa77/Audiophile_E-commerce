import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ProductService } from '../../../services/product/product.service';
import { Product } from '../../../models/product';

@Component({
  selector: 'app-promotions',
  imports: [CommonModule],
  templateUrl: './promotions.component.html',
  styleUrl: './promotions.component.css'
})
export class PromotionsComponent {

  promotionsProducts?: Product[];

  constructor(private router: Router, private productService: ProductService) {  }

  ngOnInit() {
    this.productService.getFilteredProducts(true, false)
    .subscribe(products => {
      this.promotionsProducts = products;
    });
  }
  
  viewProduct(product: Product) {
    sessionStorage.setItem('selectedProduct', JSON.stringify(product));    
    this.router.navigate(['/product', product.id]);
  }

}
