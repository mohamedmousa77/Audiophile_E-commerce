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
    // const product = this.promotionsProducts?.find(p => p.id === productId);
    console.log(`Product sent from promo products:  ${product}`);
    
    this.router.navigate(['/product', product.id], {
      state: { product: product }
    });
  }

}
