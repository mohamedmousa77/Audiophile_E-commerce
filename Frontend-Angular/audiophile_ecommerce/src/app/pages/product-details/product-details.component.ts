import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Product } from '../../models/product';
import { ProductService } from '../../services/product/product.service';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-product-details',
  imports: [CommonModule],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.css'
})
export class ProductDetailsComponent {

  productId: number = 0;
  product: any;

  constructor(private route: ActivatedRoute, private router: Router, private productS: ProductService) {  }

  ngOnInit() {
    const navigation = this.router.getCurrentNavigation();
    this.product = navigation?.extras?.state?.['product'];
    console.log("Product received via router state:", this.product);
    console.log("Product received via router state:", navigation?.extras?.state?.['product']);
    if (!this.product) {
      // fallback
      this.productId = parseInt(this.route.snapshot.paramMap.get('id')!);
      this.product = this.productS.getProductById(this.productId);
      console.warn("Product not found in navigation state. ID from URL:", this.productId);
    } else {
      console.log("Product received via router state:", this.product);
    }
  }

}
